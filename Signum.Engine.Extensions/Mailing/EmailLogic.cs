﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Signum.Engine.Basics;
using Signum.Engine.Maps;
using Signum.Entities.Authorization;
using Signum.Utilities;
using Signum.Entities.Mailing;
using Signum.Engine.Processes;
using Signum.Entities.Processes;
using Signum.Entities;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Operations;
using System.Net;
using Signum.Engine.Authorization;
using Signum.Utilities.Reflection;
using System.ComponentModel;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Linq.Expressions;
using Signum.Engine.Exceptions;
using Signum.Entities.Basics;
using Signum.Entities.DynamicQuery;
using System.Text.RegularExpressions;
using System.Globalization;
using Signum.Engine.Translation;

namespace Signum.Engine.Mailing
{
    public class EmailContent
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        bool IsPlainText { get; set; }
    }

    public interface IEmailModel
    {
        IdentifiableEntity UntypedEntity { get; }
    }

    public interface IEmailModelWithRecipient : IEmailModel
    {
        Lite<IEmailOwnerDN> Recipient { get; set; }
    }

    public interface IEmailModelWithCC : IEmailModel
    {
        List<string> CCRecipients { get; set; }
    }

    public class EmailModel<T> : IEmailModel
        where T : IdentifiableEntity
    {
        public T Entity { get; set; }

        IdentifiableEntity IEmailModel.UntypedEntity
        {
            get { return Entity; }
        }
    }

    public static class EmailLogic
    {
        public static string DoNotSend = "null@null.com";

        public static Func<string> OverrideEmailAddress = () => null;

        public static Func<EmailMessageDN, SmtpClient> SmtpClientBuilder;

        internal static void AssertStarted(SchemaBuilder sb)
        {
            sb.AssertDefined(ReflectionTools.GetMethodInfo(() => EmailLogic.Start(null, null)));
        }

        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                CultureInfoLogic.AssertStarted(sb);

                sb.Include<EmailMessageDN>();
                sb.Include<EmailTemplateDN>();
                sb.Include<EmailMasterTemplateDN>();

                dqm.RegisterQuery(typeof(EmailMasterTemplateDN), () =>
                    from t in Database.Query<EmailMasterTemplateDN>()
                    select new
                    {
                        Entity = t,
                        t.Id,
                        t.Name,
                        t.State
                    });

                dqm.RegisterQuery(typeof(EmailTemplateDN), () => 
                    from t in Database.Query<EmailTemplateDN>()
                    select new
                    {
                        Entity = t,
                        t.Id,
                        t.Name,
                        t.From,
                        t.DisplayFrom,
                        t.Bcc,
                        t.Cc,
                        Active = t.IsActiveNow(),
                        t.IsBodyHtml
                    });

                dqm.RegisterQuery(typeof(EmailMessageDN), () => 
                    from e in Database.Query<EmailMessageDN>()
                    select new
                    {
                        Entity = e,
                        e.Id,
                        e.Recipient,
                        e.State,
                        e.Subject,
                        Body = e.Text,
                        e.Template,
                        e.Sent,
                        e.Received,
                        e.Package,
                        e.Exception,
                    });

                sb.Schema.Initializing[InitLevel.Level2NormalEntities] += Schema_Initializing;
                sb.Schema.Generating += Schema_Generating;
                sb.Schema.Synchronizing += Schema_Synchronizing;

                sb.Schema.EntityEvents<EmailTemplateDN>().PreSaving += new PreSavingEventHandler<EmailTemplateDN>(EmailTemplate_PreSaving);

                Validator.OverridePropertyValidator((EmailTemplateMessageDN m) => m.Text).StaticPropertyValidation += 
                    EmailTemplateMessageText_StaticPropertyValidation;

                Validator.OverridePropertyValidator((EmailTemplateMessageDN m) => m.Subject).StaticPropertyValidation +=
                    EmailTemplateMessageSubject_StaticPropertyValidation;

                EmailTemplateGraph.Register();
                EmailMasterTemplateGraph.Register();

                EmailTemplateDN.AssociatedTypeIsEmailOwner = t =>
                    typeof(IEmailOwnerDN).IsAssignableFrom(t.ToType());

                new Graph<EmailTemplateDN>.Execute(EmailTemplateOperation.Save)
                {
                    AllowsNew = true,
                    Lite = false,
                    Execute = (et, _) => { },
                }.Register();
            }
        }

        public static void Configure(IEmailLogicConfiguration config)
        {
            EmailTemplateDN.DefaultCulture = config.DefaultCulture;
            EmailTemplateParser.GlobalVariables.Add("UrlPrefix", _ => config.UrlPrefix);
            SenderManager = new EmailSenderManager(config.DefaultFrom, config.DefaultDisplayFrom, config.DefaultBCC);
        }

        static string EmailTemplateMessageText_StaticPropertyValidation(EmailTemplateMessageDN message, PropertyInfo pi)
        {
            EmailTemplateParser.BlockNode parsedNode = message.TextParsedNode as EmailTemplateParser.BlockNode;

            if (parsedNode == null)
            {
                try
                {
                    parsedNode = ParseTemplate(message, message.Text);
                    message.TextParsedNode = parsedNode;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            return null;
        }
        static string EmailTemplateMessageSubject_StaticPropertyValidation(EmailTemplateMessageDN message, PropertyInfo pi)
        {
            EmailTemplateParser.BlockNode parsedNode = message.TextParsedNode as EmailTemplateParser.BlockNode;

            if (parsedNode == null)
            {
                try
                {
                    parsedNode = ParseTemplate(message, message.Subject);
                    message.SubjectParsedNode = parsedNode;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            return null;
        }

        private static EmailTemplateParser.BlockNode ParseTemplate(EmailTemplateMessageDN message, string text)
        {
            Type query = message.Template.AssociatedType.ToType();
            QueryDescription qd = DynamicQueryManager.Current.QueryDescription(query);

            List<QueryToken> list = new List<QueryToken>();
            return EmailTemplateParser.Parse(text, s => QueryUtils.Parse("Entity." + s, qd, false), message.Template.Model.ToType());
        }

        static Dictionary<Type, Func<EmailTemplateDN>> emailModels =
            new Dictionary<Type, Func<EmailTemplateDN>>();
        static Dictionary<Type, EmailModelDN> emailModelToDN;
        static Dictionary<EmailModelDN, Type> emailModelToType;

        public static void RegisterEmailModel<T>(Func<EmailTemplateDN> defaultTemplateConstructor = null)
            where T : IEmailModel
        {
            RegisterEmailModel(typeof(T), defaultTemplateConstructor);
        }

        public static void RegisterEmailModel(Type model, Func<EmailTemplateDN> defaultTemplateConstructor = null)
        {
            emailModels[model] = defaultTemplateConstructor;
        }

        static void EmailTemplate_PreSaving(EmailTemplateDN template, ref bool graphModified)
        {
            Type query = template.AssociatedType.ToType();
            QueryDescription qd = DynamicQueryManager.Current.QueryDescription(query);

            List<QueryToken> list = new List<QueryToken>();

            if (!(typeof(IEmailModelWithRecipient).IsAssignableFrom(template.Model.ToType())))
            {
                list.Add(QueryUtils.Parse(".".Combine("Entity", template.Recipient.TokenString), qd, false));
            }

            foreach (var message in template.Messages)
            {
                EmailTemplateParser.Parse(message.Text, s => QueryUtils.Parse("Entity." + s, qd, false), template.Model.ToType()).FillQueryTokens(list);
                EmailTemplateParser.Parse(message.Subject, s => QueryUtils.Parse("Entity." + s, qd, false), template.Model.ToType()).FillQueryTokens(list);
            }

            var tokens = list.Distinct();

            var tokensRemoved = template.Tokens.TryCC(tt => tt.Extract(t => !tokens.Contains(t.Token))) ?? new List<TemplateQueryTokenDN>();

            var tokensToAdd = tokens.Where(t =>
                !template.Tokens.Any(tt => tt.Token == t))
                .Select(t => new TemplateQueryTokenDN { Token = t });

            if (tokensRemoved.Any() || tokensToAdd.Any())
            {
                if (template.Tokens == null)
                    template.Tokens = new MList<TemplateQueryTokenDN>();
                template.Tokens.AddRange(tokensToAdd);
                graphModified = true;
            }
        }

        #region database management
        static void Schema_Initializing()
        {
            var dbTemplates = Database.RetrieveAll<EmailModelDN>();

            emailModelToDN = EnumerableExtensions.JoinStrict(
                dbTemplates, emailModels.Keys, typeDN => typeDN.FullClassName, type => type.FullName,
                (typeDN, type) => KVP.Create(type, typeDN), "caching EmailTemplates").ToDictionary();

            emailModelToType = emailModelToDN.Inverse();
        }

        static readonly string systemTemplatesReplacementKey = "EmailTemplates";

        static SqlPreCommand Schema_Synchronizing(Replacements replacements)
        {
            Table table = Schema.Current.Table<EmailModelDN>();

            Dictionary<string, EmailModelDN> should = GenerateTemplates().ToDictionary(s => s.FullClassName);
            Dictionary<string, EmailModelDN> old = Administrator.TryRetrieveAll<EmailModelDN>(replacements).ToDictionary(c =>
                c.FullClassName);

            replacements.AskForReplacements(
                old.Keys.ToHashSet(),
                should.Keys.ToHashSet(), systemTemplatesReplacementKey);

            Dictionary<string, EmailModelDN> current = replacements.ApplyReplacementsToOld(old, systemTemplatesReplacementKey);

            return Synchronizer.SynchronizeScript(should, current,
                (tn, s) => table.InsertSqlSync(s),
                (tn, c) => table.DeleteSqlSync(c),
                (tn, s, c) =>
                {
                    c.FullClassName = s.FullClassName;
                    return table.UpdateSqlSync(c);
                },
                Spacing.Double);
        }

        internal static List<EmailModelDN> GenerateTemplates()
        {
            var lista = (from type in emailModels.Keys
                         select new EmailModelDN
                         {
                             FullClassName = type.FullName,
                             FriendlyName = type.NiceName()
                         }).ToList();
            return lista;
        }

        static SqlPreCommand Schema_Generating()
        {
            Table table = Schema.Current.Table<EmailTemplateDN>();

            return (from ei in GenerateTemplates()
                    select table.InsertSqlSync(ei)).Combine(Spacing.Simple);
        }

        #endregion

        #region Old

        //public static void StarProcesses(SchemaBuilder sb, DynamicQueryManager dqm)
        //{
        //    if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
        //    {
        //        sb.Include<EmailPackageDN>();

        //        ProcessLogic.AssertStarted(sb);
        //        ProcessLogic.Register(EmailProcesses.SendEmails, new SendEmailProcessAlgorithm());

        //        new BasicConstructFromMany<EmailMessageDN, ProcessExecutionDN>(EmailOperations.ReSendEmails)
        //        {
        //            Construct = (messages, args) => ProcessLogic.Create(EmailProcesses.SendEmails, messages)
        //        }.Register();

        //        dqm[typeof(EmailPackageDN)] = (from e in Database.Query<EmailPackageDN>()
        //                                       select new
        //                                       {
        //                                           Entity = e,
        //                                           e.Id,
        //                                           e.Name,
        //                                           e.NumLines,
        //                                           e.NumErrors,
        //                                       }).ToDynamic();
        //    }
        //}

        ////public static EmailMessageDN CreateEmailMessage(IEmailModel model, Lite<EmailPackageDN> package)
        ////{

        ////    if (model == null)
        ////        throw new ArgumentNullException("model");

        ////    if (model.To == null)
        ////        throw new ArgumentNullException("model.To");


        ////    using (Sync.ChangeBothCultures(model.To.CultureInfo))
        ////    {
        ////        //EmailContent content = GetTemplate(model.GetType())(model);

        ////        var result = new EmailMessageDN
        ////        {
        ////            State = EmailState.Created,
        ////            Recipient = model.To.ToLite(),
        ////            Bcc = model.Bcc,
        ////            Cc = model.Cc,
        ////            //TemplateOld = GetTemplateDN(model.GetType()),
        ////            //Subject = content.Subject,
        ////            //Text = content.Body,
        ////            Package = package
        ////        };
        ////        return result;
        ////    }
        ////}

        ////public static EmailMessageDN Send(this IEmailModel model)
        ////{
        ////    EmailMessageDN result = CreateEmailMessage(model, null);

        ////    SendMail(result);

        ////    return result;
        ////}


        //    return exceptions;
        //}

        #endregion

        public static EmailSenderManager SenderManager;

        public static EmailModelDN ToEmailModel(Type type)
        {
            return emailModelToDN.GetOrThrow(type, "The email model {0} was not registered");
        }

        public static Type ToType(this EmailModelDN model)
        {
            if (model == null)
                return null;

            return emailModelToType.GetOrThrow(model, "The email model {0} was not registered");
        }

        public static EmailMessageDN CreateEmailMessage(this IEmailModel model)
        {
            var systemTemplate = ToEmailModel(model.GetType());

            var template = Database.Query<EmailTemplateDN>().SingleOrDefaultEx(t =>
                t.IsActiveNow() == true &&
                t.Model == systemTemplate);
            if (template == null)
                template = emailModels[systemTemplate.GetType()]();
            return CreateEmailMessage(template, model.UntypedEntity, model);
        }

        public static EmailMessageDN CreateEmailMessage(this EmailTemplateDN template, IIdentifiable entity)
        {
            return CreateEmailMessage(template, entity, null);
        }

        public static EmailMessageDN CreateEmailMessage(this EmailTemplateDN template, IIdentifiable entity, IEmailModel model)
        {
            Type query = template.AssociatedType.ToType();
            QueryDescription qd = DynamicQueryManager.Current.QueryDescription(query);

            var columns = template.Tokens.Select(qt => new Column(QueryUtils.Parse("Entity." + qt.TokenString, qd, false), null)).ToList();

            var entityToken = QueryUtils.Parse("Entity", qd, false);

            var table = DynamicQueryManager.Current.ExecuteQuery(new QueryRequest
            {
                QueryName = query,
                Columns = columns,
                ElementsPerPage = QueryRequest.AllElements,
                Filters = new List<Filter>
                {
                    new Filter(entityToken, FilterOperation.EqualTo, Lite.Create(entityToken.Type.CleanType(), entity.Id))
                }
            });

            var dicTokenColumn = table.Columns.ToDictionary(rc => rc.Column.Token);

            Lite<IEmailOwnerDN> recipient = null;

            if (model != null && model is IEmailModelWithRecipient)
                recipient = ((IEmailModelWithRecipient)model).Recipient;
            else
            {
                recipient = EmailTemplateParser.GetRecipient(table,
                    dicTokenColumn[QueryUtils.Parse("Entity." + template.Recipient.TokenString, qd, false)]);
            }

            var email = new EmailMessageDN
            {
                Recipient = recipient,
                From = template.From,
                DisplayFrom = template.DisplayFrom,
                Cc = template.Cc,
                Bcc = template.Bcc,
                IsBodyHtml = template.IsBodyHtml,
                EditableMessage = template.EditableMessage,
                Template = template.ToLite(),
            };

            if (template.Model != null && template.Model is IEmailModelWithCC)
            {
                email.Cc = ",".Combine(email.Cc, ((IEmailModelWithCC)template.Model).CCRecipients.ToString(","));
            }

            var recipientCI = recipient.InDB(io => io.CultureInfo);
            var cultureInfo = recipientCI ?? EmailTemplateDN.DefaultCulture.CultureInfo;

            var message = template.Messages.SingleOrDefault(tm => tm.CultureInfo.CultureInfo == cultureInfo) ??
                template.Messages.SingleOrDefault(tm => tm.CultureInfo.CultureInfo == cultureInfo.Parent) ??
                template.Messages.SingleOrDefault(tm => tm.CultureInfo.CultureInfo == EmailTemplateDN.DefaultCulture.CultureInfo);

            Func<string, QueryToken> parseToken = str => QueryUtils.Parse("Entity." + str, qd, false);

            if (message.SubjectParsedNode == null)
                message.SubjectParsedNode = EmailTemplateParser.Parse(message.Subject, parseToken, template.Model.ToType());

            email.Subject = ((EmailTemplateParser.BlockNode)message.SubjectParsedNode).Print(
                new EmailTemplateParameters
                {
                    Columns = dicTokenColumn,
                    IsHtml = false,
                    CultureInfo = cultureInfo,
                    Entity = entity,
                    Model = model
                },
                table.Rows);

            if (message.TextParsedNode == null)
                message.TextParsedNode = EmailTemplateParser.Parse(message.Text, parseToken, template.Model.ToType());

            var body = ((EmailTemplateParser.BlockNode)message.TextParsedNode).Print(
                new EmailTemplateParameters
                {
                    Columns = dicTokenColumn,
                    IsHtml = template.IsBodyHtml,
                    CultureInfo = cultureInfo,
                    Entity = entity,
                    Model = model
                },
                table.Rows);

            if (template.MasterTemplate != null)
                body = EmailMasterTemplateDN.MasterTemplateContentRegex.Replace(template.MasterTemplate.Retrieve().Text, m => body);

            email.Text = body;

            return email;
        }


        public static void SendMail(this IEmailModel model)
        {
            var email = model.CreateEmailMessage();
            SenderManager.Send(email);
        }

        public static void SendMail(this EmailTemplateDN template, IIdentifiable entity)
        {
            var email = template.CreateEmailMessage(entity);
            SenderManager.Send(email);
        }

        public static void SendMailAsync(this IEmailModel model)
        {
            var email = model.CreateEmailMessage();
            SenderManager.SendAsync(email);
        }

        public static void SendMailAsync(this IIdentifiable entity, EmailTemplateDN template)
        {
            var email = template.CreateEmailMessage(entity);
            SenderManager.SendAsync(email);
        }


        public static void SafeSendMailAsync(this SmtpClient client, MailMessage message, Action<AsyncCompletedEventArgs> onComplete)
        {
            client.SendCompleted += (object sender, AsyncCompletedEventArgs e) =>
            {
                //client.Dispose(); -> the client can be used later by other messages
                message.Dispose();
                using (AuthLogic.Disable())
                {
                    try
                    {
                        onComplete(e);
                    }
                    catch (Exception ex)
                    {
                        ex.LogException();
                    }
                }
            };
            client.SendAsync(message, null);
        }

        public static SmtpClient SafeSmtpClient()
        {
            //http://weblogs.asp.net/stanleygu/archive/2010/03/31/tip-14-solve-smtpclient-issues-of-delayed-email-and-high-cpu-usage.aspx
            return new SmtpClient()
            {
                ServicePoint = { MaxIdleTime = 2 }
            };
        }

        internal static SmtpClient SafeSmtpClient(string host, int port)
        {
            //http://weblogs.asp.net/stanleygu/archive/2010/03/31/tip-14-solve-smtpclient-issues-of-delayed-email-and-high-cpu-usage.aspx
            return new SmtpClient(host, port)
            {
                ServicePoint = { MaxIdleTime = 2 }
            };
        }

        public static MList<EmailTemplateMessageDN> CreateMessages(Func<EmailTemplateMessageDN> func)
        {
            var list = new MList<EmailTemplateMessageDN>();
            foreach (var ci in CultureInfoLogic.ApplicationCultures)
            {
                using (Sync.ChangeBothCultures(ci))
                {
                    list.Add(func());
                }
            }
            return list;
        }
    }


    public class EmailSenderManager
    {
        public EmailSenderManager(string defaultFrom, string defaultDisplayFrom, string defaultBcc)
        {
            DefaultFrom = defaultFrom;
            DefaultDisplayFrom = defaultDisplayFrom;
            DefaultBcc = defaultBcc;
        }

        public string DefaultFrom;
        public string DefaultDisplayFrom;
        public string DefaultBcc;


        protected MailMessage CreateMailMessage(EmailMessageDN email)
        {
            email.To = GetRecipient(email, null);

            MailMessage message = new MailMessage()
            {
                To = { new MailAddress(email.To) },
                From = email.DisplayFrom.HasText() ? new MailAddress(email.From, email.DisplayFrom) : new MailAddress(email.From),
                Subject = email.Subject,
                Body = email.Text,
                IsBodyHtml = email.IsBodyHtml,
            };

            if (email.Bcc.HasText())
                message.Bcc.AddRange(email.Bcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(a => new MailAddress(a)).ToList());
            if (email.Cc.HasText())
                message.CC.AddRange(email.Cc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(a => new MailAddress(a)).ToList());
            return message;
        }

        protected virtual string GetRecipient(EmailMessageDN email, object[] args)
        {
            return email.Recipient.InDB().Select(r => r.Email).SingleEx();
        }

        public virtual void Send(EmailMessageDN email)
        {
            try
            {
                SmtpClient client = CreateSmtpClient(email);

                MailMessage message = CreateMailMessage(email);

                client.Send(message);

                email.State = EmailMessageState.Sent;
                email.Sent = TimeZoneManager.Now;
                email.Received = null;
                email.Save();
            }
            catch (Exception ex)
            {
                if (Transaction.InTestTransaction) //Transaction.IsTestTransaction
                    throw;

                var exLog = ex.LogException().ToLite();

                using (Transaction tr = Transaction.ForceNew())
                {
                    email.Exception = exLog;
                    email.State = EmailMessageState.Exception;
                    email.Save();
                    tr.Commit();
                }

                throw;
            }
        }

        public Lite<SMTPConfigurationDN> DefaultSMTPConfiguration;

        SmtpClient CreateSmtpClient(EmailMessageDN email)
        {
            return email.Template != null
                ? email.Template.InDB().Select(t => t.SMTPConfiguration).SingleOrDefault().TryCC(c => c.GenerateSmtpClient(true))
                : null
                ?? DefaultSMTPConfiguration.TryCC(c => c.GenerateSmtpClient(true))
                ?? EmailLogic.SafeSmtpClient();
        }

        public virtual void SendAsync(EmailMessageDN email)
        {
            try
            {
                SmtpClient client = CreateSmtpClient(email);

                MailMessage message = CreateMailMessage(email);

                email.Sent = null;
                email.Received = null;
                email.Save();

                client.SafeSendMailAsync(message, args =>
                {
                    Expression<Func<EmailMessageDN, EmailMessageDN>> updater;
                    if (args.Error != null)
                    {
                        var exLog = args.Error.LogException().ToLite();
                        updater = em => new EmailMessageDN
                {
                    Exception = exLog,
                    State = EmailMessageState.Exception
                };
                    }
                    else
                        updater = em => new EmailMessageDN
                        {
                            State = EmailMessageState.Sent,
                            Sent = TimeZoneManager.Now
                        };

                    for (int i = 0; i < 4; i++) //to allow main thread to save email asynchronously
                    {
                        if (email.InDB().UnsafeUpdate(updater) > 0)
                            return;

                        if (i != 3)
                            Thread.Sleep(3000);
                    }
                });
            }
            catch (Exception ex)
            {
                if (Transaction.InTestTransaction) //Transaction.InTestTransaction
                    throw;

                var exLog = ex.LogException().ToLite();

                using (Transaction tr = Transaction.ForceNew())
                {
                    email.Exception = exLog;
                    email.State = EmailMessageState.Exception;
                    email.Save();
                    tr.Commit();
                }

                throw;
            }
        }

    }


}
