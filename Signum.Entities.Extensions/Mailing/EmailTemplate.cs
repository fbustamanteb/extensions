﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.Basics;
using System.Linq.Expressions;
using Signum.Utilities;
using Signum.Entities.UserQueries;
using Signum.Entities.DynamicQuery;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.ComponentModel;
using Signum.Entities.Translation;

namespace Signum.Entities.Mailing
{
    public enum EmailTemplateState
    {
        Created,
        Modified
    }

    public enum EmailTemplateOperation
    {
        Create,
        Save,
        Enable,
        Disable
    }

    [Serializable, EntityKind(EntityKind.Main)]
    public class EmailTemplateDN : Entity
    {
        [NotNullable, SqlDbType(Size = 100), UniqueIndex]
        string name;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string Name
        {
            get { return name; }
            set { SetToStr(ref name, value, () => Name); }
        }

        bool editableMessage = true;
        public bool EditableMessage
        {
            get { return editableMessage; }
            set { Set(ref editableMessage, value, () => EditableMessage); }
        }

        [NotNullable]
        TypeDN associatedType;
        [NotNullValidator]
        public TypeDN AssociatedType
        {
            get { return associatedType; }
            set { Set(ref associatedType, value, () => AssociatedType); }
        }

        TemplateQueryTokenDN recipient;
        public TemplateQueryTokenDN Recipient
        {
            get { return recipient; }
            set { Set(ref recipient, value, () => Recipient); }
        }

        bool isBodyHtml = true;
        public bool IsBodyHtml
        {
            get { return isBodyHtml; }
            set { Set(ref isBodyHtml, value, () => IsBodyHtml); }
        }

        MList<TemplateQueryTokenDN> tokens;
        public MList<TemplateQueryTokenDN> Tokens
        {
            get { return tokens; }
            set { Set(ref tokens, value, () => Tokens); }
        }

        [NotifyCollectionChanged]
        MList<EmailTemplateMessageDN> messages;
        public MList<EmailTemplateMessageDN> Messages
        {
            get { return messages; }
            set { Set(ref messages, value, () => Messages); }
        }

        protected override void ChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (sender == messages)
            {
                foreach (var item in args.OldItems.Cast<EmailTemplateMessageDN>())
                    item.Template = null;

                foreach (var item in args.NewItems.Cast<EmailTemplateMessageDN>())
                    item.Template = this;
            }
        }

        protected override void PreSaving(ref bool graphModified)
        {
            base.PreSaving(ref graphModified);

            messages.ForEach(e => e.Template = this);
        }

        string from;
        [StringLengthValidator(AllowNulls = false)]
        public string From
        {
            get { return from; }
            set { Set(ref from, value, () => From); }
        }

        string displayFrom;
        public string DisplayFrom
        {
            get { return displayFrom; }
            set { Set(ref displayFrom, value, () => DisplayFrom); }
        }

        string bcc;
        public string Bcc
        {
            get { return bcc; }
            set { Set(ref bcc, value, () => Bcc); }
        }

        string cc;
        public string Cc
        {
            get { return cc; }
            set { Set(ref cc, value, () => Cc); }
        }

        Lite<EmailMasterTemplateDN> masterTemplate;
        public Lite<EmailMasterTemplateDN> MasterTemplate
        {
            get { return masterTemplate; }
            set { Set(ref masterTemplate, value, () => MasterTemplate); }
        }

        Lite<SMTPConfigurationDN> smtpConfiguration;
        public Lite<SMTPConfigurationDN> SMTPConfiguration
        {
            get { return smtpConfiguration; }
            set { Set(ref smtpConfiguration, value, () => SMTPConfiguration); }
        }

        EmailModelDN model;
        public EmailModelDN Model
        {
            get { return model; }
            set { Set(ref model, value, () => Model); }
        }

        EmailTemplateState state = EmailTemplateState.Created;
        public EmailTemplateState State
        {
            get { return state; }
            set { Set(ref state, value, () => State); }
        }

        bool active;
        public bool Active
        {
            get { return active; }
            set { Set(ref active, value, () => Active); }
        }

        DateTime startDate = TimeZoneManager.Now.TrimToMinutes();
        [MinutesPrecissionValidator]
        public DateTime StartDate
        {
            get { return startDate; }
            set { Set(ref startDate, value, () => StartDate); }
        }

        DateTime? endDate;
        [MinutesPrecissionValidator]
        public DateTime? EndDate
        {
            get { return endDate; }
            set { Set(ref endDate, value, () => EndDate); }
        }

        static Expression<Func<EmailTemplateDN, bool>> IsActiveNowExpression =
            (mt) => mt.active && TimeZoneManager.Now.IsInInterval(mt.StartDate, mt.EndDate);
        public bool IsActiveNow()
        {
            return IsActiveNowExpression.Evaluate(this);
        }

        public static Func<TypeDN, bool> AssociatedTypeIsEmailOwner;
        public static CultureInfoDN DefaultCulture;

        protected override string PropertyValidation(System.Reflection.PropertyInfo pi)
        {
            if (pi.Is(() => StartDate) || pi.Is(() => EndDate))
            {
                if (EndDate != null && EndDate >= StartDate)
                    return EmailTemplateMessage.EndDateMustBeHigherThanStartDate.NiceToString();
            }

            if (pi.Is(() => Recipient) && AssociatedType != null && !AssociatedTypeIsEmailOwner(AssociatedType) 
                && Recipient == null && Model == null)
            {
                return EmailTemplateMessage.RouteToGetRecipientNotSet.NiceToString();
            }

            if (pi.Is(() => Messages))
            {
                if (Messages == null || !Messages.Any())
                    return EmailTemplateMessage.ThereAreNoMessagesForTheTemplate.NiceToString();
                if (!Messages.Any(m => m.CultureInfo.Is(DefaultCulture)))
                {
                    return EmailTemplateMessage.ThereMustBeAMessageFor0.NiceToString().Formato(DefaultCulture.DisplayName);
                }
                if (Messages.GroupCount(m => m.CultureInfo).Any(c => c.Value > 1))
                {
                    return EmailTemplateMessage.TheresMoreThanOneMessageForTheSameLanguage.NiceToString();
                }
            }

            if (pi.Is(() => Model) || pi.Is(() => AssociatedType))
            {
                if (Model != null && AssociatedType != null && Model.FullClassName != AssociatedType.FullClassName)
                    return EmailTemplateMessage.TheAssociatedTypeAndTheModelDoNotMatch.NiceToString();
            }

            return base.PropertyValidation(pi);
        }

        static readonly Expression<Func<EmailTemplateDN, string>> ToStringExpression = e => e.Name;
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    [Serializable]
    public class EmailModelDN : IdentifiableEntity
    {
        [NotNullable, UniqueIndex]
        string fullClassName;
        public string FullClassName
        {
            get { return fullClassName; }
            set { Set(ref fullClassName, value, () => FullClassName); }
        }

        [NotNullable]
        string friendlyName;
        [StringLengthValidator(Min = 1)]
        public string FriendlyName
        {
            get { return friendlyName; }
            set { Set(ref friendlyName, value, () => FriendlyName); }
        }

        static readonly Expression<Func<EmailModelDN, string>> ToStringExpression = e => e.FriendlyName;
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    [Serializable]
    public class EmailTemplateMessageDN : EmbeddedEntity
    {
        [Ignore]
        internal EmailTemplateDN Template;

        [NotNullable]
        CultureInfoDN cultureInfo;
        [NotNullValidator]
        public CultureInfoDN CultureInfo
        {
            get { return cultureInfo; }
            set { Set(ref cultureInfo, value, () => CultureInfo); }
        }

        [NotNullable, SqlDbType(Size = int.MaxValue)]
        string text;
        [StringLengthValidator(AllowNulls = false, Max = int.MaxValue)]
        public string Text
        {
            get { return text; }
            set
            {
                if (Set(ref text, value, () => Text))
                    TextParsedNode = null;
            }
        }

        [Ignore]
        internal object TextParsedNode;

        [SqlDbType(Size = 200)]
        string subject;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 200)]
        public string Subject
        {
            get { return subject; }
            set
            {
                if (Set(ref subject, value, () => Subject))
                    SubjectParsedNode = null;
            }
        }

        [Ignore]
        internal object SubjectParsedNode;
    }

    [Serializable]
    public class TemplateQueryTokenDN : QueryTokenDN
    {
        public override void ParseData(IdentifiableEntity context, QueryDescription description, bool canAggregate)
        {
            throw new NotImplementedException("ParseData is ambiguous on {0}".Formato(GetType().NiceName()));
        }
    }

    public enum EmailMasterTemplateOperation
    {
        Create,
        Save
	}

    [Serializable, EntityKind(EntityKind.Main)]
    public class EmailMasterTemplateDN : Entity
    {
        [NotNullable, SqlDbType(Size = 100), UniqueIndex]
        string name;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string Name
        {
            get { return name; }
            set { SetToStr(ref name, value, () => Name); }
        }

        [NotNullable, SqlDbType(Size = int.MaxValue)]
        string text;
        [StringLengthValidator(AllowNulls = false, Max = int.MaxValue)]
        public string Text
        {
            get { return text; }
            set { Set(ref text, value, () => Text); }
        }

        EmailTemplateState state = EmailTemplateState.Created;
        public EmailTemplateState State
        {
            get { return state; }
            set { Set(ref state, value, () => State); }
        }

        static Expression<Func<EmailMasterTemplateDN, string>> ToStringExpression = e => e.name;
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }

        [Ignore]
        public static readonly Regex MasterTemplateContentRegex = new Regex(@"\@\[content\]");

        protected override string PropertyValidation(System.Reflection.PropertyInfo pi)
        {
            if (pi.Is(() => Text) && !MasterTemplateContentRegex.IsMatch(Text))
            {
                throw new ApplicationException(EmailTemplateMessage.TheTextMustContain0IndicatingReplacementPoint.NiceToString().Formato("@[content]"));
            }

            return base.PropertyValidation(pi);
        }
    }

    public enum EmailTemplateMessage
    {
        [Description("End date must be higher than start date")]
        EndDateMustBeHigherThanStartDate,
        [Description("Route to get recipient not set")]
        RouteToGetRecipientNotSet,
        [Description("There are no messages for the template")]
        ThereAreNoMessagesForTheTemplate,
        [Description("There must be a message for {0}")]
        ThereMustBeAMessageFor0,
        [Description("There's more than one message for the same language")]
        TheresMoreThanOneMessageForTheSameLanguage,
        [Description("The associated type and the model do not match")]
        TheAssociatedTypeAndTheModelDoNotMatch,
        [Description("The text must contain {0} indicating replacement point")]
        TheTextMustContain0IndicatingReplacementPoint,
        [Description("The template is already active")]
        TheTemplateIsAlreadyActive,
        [Description("The template is already inactive")]
        TheTemplateIsAlreadyInactive,
        [Description("The model {0} does not have any field with the token {1}")]
        TheModel0DoesNotHaveAnyFieldWithTheToken1
    }

    public enum EmailTemplateViewMessage
    { 
        [Description("Insert message content")]
        InsertMessageContent,
        [Description("Insert in text")]
        InsertInText,
        [Description("Insert iteration in text")]
        InsertIterationInText,
        [Description("Insert in subject")]
        InsertInSubject,
        [Description("Insert iteration in subject")]
        InsertIterationInSubject,
        [Description("Preview content")]
        PreviewContent,
        [Description("Edit content")]
        EditContent,
        [Description("Language")]
        Language
    }
}
