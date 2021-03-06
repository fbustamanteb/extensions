﻿using Signum.Entities.Workflow;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Maps;
using Signum.Engine.Operations;
using Signum.Entities;
using Signum.Entities.Authorization;
using Signum.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Signum.Entities.Basics;
using Signum.Engine.Authorization;
using Signum.Entities.Scheduler;

namespace Signum.Engine.Workflow
{

    public static class WorkflowLogic
    {
        public static Action<ICaseMainEntity, WorkflowTransitionContext> OnTransition;

        static Expression<Func<WorkflowEntity, IQueryable<WorkflowPoolEntity>>> WorkflowPoolsExpression =
            e => Database.Query<WorkflowPoolEntity>().Where(a => a.Workflow == e);
        [ExpressionField]
        public static IQueryable<WorkflowPoolEntity> WorkflowPools(this WorkflowEntity e)
        {
            return WorkflowPoolsExpression.Evaluate(e);
        }

        static Expression<Func<WorkflowEntity, IQueryable<WorkflowActivityEntity>>> WorkflowActivitiesExpression =
            e => Database.Query<WorkflowActivityEntity>().Where(a => a.Lane.Pool.Workflow == e);
        [ExpressionField]
        public static IQueryable<WorkflowActivityEntity> WorkflowActivities(this WorkflowEntity e)
        {
            return WorkflowActivitiesExpression.Evaluate(e);
        }

        public static IEnumerable<WorkflowActivityEntity> WorkflowActivitiesFromCache(this WorkflowEntity e)
        {
            return GetWorkflowNodeGraph(e.ToLite()).NextGraph.OfType<WorkflowActivityEntity>();
        }

        static Expression<Func<WorkflowEntity, IQueryable<WorkflowEventEntity>>> WorkflowEventsExpression =
            e => Database.Query<WorkflowEventEntity>().Where(a => a.Lane.Pool.Workflow == e);
        [ExpressionField]
        public static IQueryable<WorkflowEventEntity> WorkflowEvents(this WorkflowEntity e)
        {
            return WorkflowEventsExpression.Evaluate(e);
        }

        public static IEnumerable<WorkflowEventEntity> WorkflowEventsFromCache(this WorkflowEntity e)
        {
            return GetWorkflowNodeGraph(e.ToLite()).NextGraph.OfType<WorkflowEventEntity>();
        }

        static Expression<Func<WorkflowEntity, IQueryable<WorkflowGatewayEntity>>> WorkflowGatewaysExpression =
            e => Database.Query<WorkflowGatewayEntity>().Where(a => a.Lane.Pool.Workflow == e);
        [ExpressionField]
        public static IQueryable<WorkflowGatewayEntity> WorkflowGateways(this WorkflowEntity e)
        {
            return WorkflowGatewaysExpression.Evaluate(e);
        }

        public static IEnumerable<WorkflowGatewayEntity> WorkflowGatewaysFromCache(this WorkflowEntity e)
        {
            return GetWorkflowNodeGraph(e.ToLite()).NextGraph.OfType<WorkflowGatewayEntity>();
        }

        static Expression<Func<WorkflowEntity, IQueryable<WorkflowConnectionEntity>>> WorkflowConnectionsExpression =
          e => Database.Query<WorkflowConnectionEntity>().Where(a => a.From.Lane.Pool.Workflow == e && a.To.Lane.Pool.Workflow == e);
        [ExpressionField]
        public static IQueryable<WorkflowConnectionEntity> WorkflowConnections(this WorkflowEntity e)
        {
            return WorkflowConnectionsExpression.Evaluate(e);
        }

        public static IEnumerable<WorkflowConnectionEntity> WorkflowConnectionsFromCache(this WorkflowEntity e)
        {
            return GetWorkflowNodeGraph(e.ToLite()).NextGraph.EdgesWithValue.Select(edge => edge.Value);
        }

        static Expression<Func<WorkflowEntity, IQueryable<WorkflowConnectionEntity>>> WorkflowMessageConnectionsExpression =
         e => e.WorkflowConnections().Where(a => a.From.Lane.Pool != a.To.Lane.Pool);
        [ExpressionField]
        public static IQueryable<WorkflowConnectionEntity> WorkflowMessageConnections(this WorkflowEntity e)
        {
            return WorkflowMessageConnectionsExpression.Evaluate(e);
        }

        static Expression<Func<WorkflowPoolEntity, IQueryable<WorkflowLaneEntity>>> PoolLanesExpression =
            e => Database.Query<WorkflowLaneEntity>().Where(a => a.Pool == e);
        [ExpressionField]
        public static IQueryable<WorkflowLaneEntity> WorkflowLanes(this WorkflowPoolEntity e)
        {
            return PoolLanesExpression.Evaluate(e);
        }

        static Expression<Func<WorkflowPoolEntity, IQueryable<WorkflowConnectionEntity>>> PoolConnectionsExpression =
            e => Database.Query<WorkflowConnectionEntity>().Where(a => a.From.Lane.Pool == e && a.To.Lane.Pool == e);
        [ExpressionField]
        public static IQueryable<WorkflowConnectionEntity> WorkflowConnections(this WorkflowPoolEntity e)
        {
            return PoolConnectionsExpression.Evaluate(e);
        }

        static Expression<Func<WorkflowLaneEntity, IQueryable<WorkflowGatewayEntity>>> LaneGatewaysExpression =
            e => Database.Query<WorkflowGatewayEntity>().Where(a => a.Lane == e);
        [ExpressionField]
        public static IQueryable<WorkflowGatewayEntity> WorkflowGateways(this WorkflowLaneEntity e)
        {
            return LaneGatewaysExpression.Evaluate(e);
        }

        static Expression<Func<WorkflowLaneEntity, IQueryable<WorkflowEventEntity>>> LaneEventsExpression =
            e => Database.Query<WorkflowEventEntity>().Where(a => a.Lane == e);
        [ExpressionField]
        public static IQueryable<WorkflowEventEntity> WorkflowEvents(this WorkflowLaneEntity e)
        {
            return LaneEventsExpression.Evaluate(e);
        }

        static Expression<Func<WorkflowLaneEntity, IQueryable<WorkflowActivityEntity>>> LaneActivitiesExpression =
            e => Database.Query<WorkflowActivityEntity>().Where(a => a.Lane == e);
        [ExpressionField]
        public static IQueryable<WorkflowActivityEntity> WorkflowActivities(this WorkflowLaneEntity e)
        {
            return LaneActivitiesExpression.Evaluate(e);
        }

        static Expression<Func<IWorkflowNodeEntity, IQueryable<WorkflowConnectionEntity>>> NextConnectionsExpression =
            e => Database.Query<WorkflowConnectionEntity>().Where(a => a.From == e);
        [ExpressionField]
        public static IQueryable<WorkflowConnectionEntity> NextConnections(this IWorkflowNodeEntity e)
        {
            return NextConnectionsExpression.Evaluate(e);
        }

        public static IEnumerable<WorkflowConnectionEntity> NextConnectionsFromCache(this IWorkflowNodeEntity e)
        {
            return GetWorkflowNodeGraph(e.Lane.Pool.Workflow.ToLite()).NextGraph.RelatedTo(e).Values;
        }

        static Expression<Func<IWorkflowNodeEntity, IQueryable<WorkflowConnectionEntity>>> PreviousConnectionsExpression =
            e => Database.Query<WorkflowConnectionEntity>().Where(a => a.To == e);
        [ExpressionField]
        public static IQueryable<WorkflowConnectionEntity> PreviousConnections(this IWorkflowNodeEntity e)
        {
            return PreviousConnectionsExpression.Evaluate(e);
        }

        public static IEnumerable<WorkflowConnectionEntity> PreviousConnectionsFromCache(this IWorkflowNodeEntity e)
        {
            return GetWorkflowNodeGraph(e.Lane.Pool.Workflow.ToLite()).PreviousGraph.RelatedTo(e).Values;
        }


        public static ResetLazy<Dictionary<Lite<WorkflowEntity>, WorkflowNodeGraph>> WorkflowGraphLazy;

        public static List<Lite<IWorkflowNodeEntity>> AutocompleteNodes(Lite<WorkflowEntity> workflow, string subString, int count, List<Lite<IWorkflowNodeEntity>> excludes)
        {
            return WorkflowGraphLazy.Value.GetOrThrow(workflow).Autocomplete(subString, count, excludes);
        }

        public static WorkflowNodeGraph GetWorkflowNodeGraph(Lite<WorkflowEntity> workflow)
        {
            var graph = WorkflowGraphLazy.Value.GetOrThrow(workflow);
            if (graph.TrackId != null)
                return graph;

            lock (graph)
            {
                if (graph.TrackId != null)
                    return graph;

                var errors = graph.Validate((g, newDirection)=>
                {
                    throw new InvalidOperationException($"Unexpected direction of gateway '{g}' (Should be '{newDirection.NiceToString()}'). Consider saving Workflow '{workflow}'.");
                });

                if (errors.HasItems())
                    throw new ApplicationException("Errors in Workflow '" + workflow + "':\r\n" + errors.ToString("\r\n").Indent(4));

                return graph;
            }
        }

        static Func<WorkflowConfigurationEmbedded> getConfiguration;
        public static WorkflowConfigurationEmbedded Configuration
        {
            get { return getConfiguration(); }
        }

        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm, Func<WorkflowConfigurationEmbedded> getConfiguration)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                PermissionAuthLogic.RegisterPermissions(WorkflowScriptRunnerPanelPermission.ViewWorkflowScriptRunnerPanel);

                WorkflowLogic.getConfiguration = getConfiguration;

                sb.Include<WorkflowEntity>()
                    .WithQuery(dqm, () => e => new
                    {
                        Entity = e,
                        e.Id,
                        e.Name,
                        e.MainEntityType,
                        e.MainEntityStrategy,
                    });

                WorkflowGraph.Register();


                sb.Include<WorkflowPoolEntity>()
                    .WithUniqueIndex(wp => new { wp.Workflow, wp.Name })
                    .WithSave(WorkflowPoolOperation.Save)
                    .WithDelete(WorkflowPoolOperation.Delete)
                    .WithExpressionFrom(dqm, (WorkflowEntity p) => p.WorkflowPools())
                    .WithQuery(dqm, () => e => new
                    {
                        Entity = e,
                        e.Id,
                        e.Name,
                        e.BpmnElementId,
                        e.Workflow,
                    });

                sb.Include<WorkflowLaneEntity>()
                    .WithUniqueIndex(wp => new { wp.Pool, wp.Name })
                    .WithSave(WorkflowLaneOperation.Save)
                    .WithDelete(WorkflowLaneOperation.Delete)
                    .WithExpressionFrom(dqm, (WorkflowPoolEntity p) => p.WorkflowLanes())
                    .WithQuery(dqm, () => e => new
                    {
                        Entity = e,
                        e.Id,
                        e.Name,
                        e.BpmnElementId,
                        e.Pool,
                        e.Pool.Workflow,
                    });

                sb.Include<WorkflowActivityEntity>()
                    .WithUniqueIndex(w => new { w.Lane, w.Name })
                    .WithSave(WorkflowActivityOperation.Save)
                    .WithDelete(WorkflowActivityOperation.Delete)
                    .WithExpressionFrom(dqm, (WorkflowEntity p) => p.WorkflowActivities())
                    .WithExpressionFrom(dqm, (WorkflowLaneEntity p) => p.WorkflowActivities())
                    .WithQuery(dqm, () => e => new
                    {
                        Entity = e,
                        e.Id,
                        e.Name,
                        e.BpmnElementId,
                        e.Comments,
                        e.Lane,
                        e.Lane.Pool.Workflow,
                    });

                sb.AddUniqueIndexMList((WorkflowActivityEntity a) => a.Jumps, mle => new { mle.Parent, mle.Element.To });

                sb.Include<WorkflowEventEntity>()
                    .WithSave(WorkflowEventOperation.Save)
                    .WithExpressionFrom(dqm, (WorkflowEntity p) => p.WorkflowEvents())
                    .WithExpressionFrom(dqm, (WorkflowLaneEntity p) => p.WorkflowEvents())
                    .WithQuery(dqm, () => e => new
                    {
                        Entity = e,
                        e.Id,
                        e.Type,
                        e.Name,
                        e.BpmnElementId,
                        e.Lane,
                        e.Lane.Pool.Workflow,
                    });

                new Graph<WorkflowEventEntity>.Delete(WorkflowEventOperation.Delete)
                {
                    Delete = (e, _) =>
                    {

                        if (e.Type.IsTimerStart())
                        {
                            var scheduled = e.ScheduledTask();
                            if (scheduled != null)
                                WorkflowEventTaskLogic.DeleteWorkflowEventScheduledTask(scheduled);
                        }

                        e.Delete();
                    },
                }.Register();

                sb.Include<WorkflowGatewayEntity>()
                    .WithSave(WorkflowGatewayOperation.Save)
                    .WithDelete(WorkflowGatewayOperation.Delete)
                    .WithExpressionFrom(dqm, (WorkflowEntity p) => p.WorkflowGateways())
                    .WithExpressionFrom(dqm, (WorkflowLaneEntity p) => p.WorkflowGateways())
                    .WithQuery(dqm, () => e => new
                    {
                        Entity = e,
                        e.Id,
                        e.Type,
                        e.Name,
                        e.BpmnElementId,
                        e.Lane,
                        e.Lane.Pool.Workflow,
                    });

                sb.Include<WorkflowConnectionEntity>()
                    .WithSave(WorkflowConnectionOperation.Save)
                    .WithDelete(WorkflowConnectionOperation.Delete)
                    .WithExpressionFrom(dqm, (WorkflowEntity p) => p.WorkflowConnections())
                    .WithExpressionFrom(dqm, (WorkflowEntity p) => p.WorkflowMessageConnections(), null)
                    .WithExpressionFrom(dqm, (WorkflowPoolEntity p) => p.WorkflowConnections())
                    .WithExpressionFrom(dqm, (IWorkflowNodeEntity p) => p.NextConnections(), null)
                    .WithExpressionFrom(dqm, (IWorkflowNodeEntity p) => p.PreviousConnections(), null)
                    .WithQuery(dqm, () => e => new
                    {
                        Entity = e,
                        e.Id,
                        e.Name,
                        e.BpmnElementId,
                        e.From,
                        e.To,
                    });

                WorkflowGraphLazy = sb.GlobalLazy(() =>
                {
                    using (new EntityCache())
                    {
                        var events = Database.RetrieveAll<WorkflowEventEntity>().GroupToDictionary(a => a.Lane.Pool.Workflow.ToLite());
                        var gateways = Database.RetrieveAll<WorkflowGatewayEntity>().GroupToDictionary(a => a.Lane.Pool.Workflow.ToLite());
                        var activities = Database.RetrieveAll<WorkflowActivityEntity>().GroupToDictionary(a => a.Lane.Pool.Workflow.ToLite());
                        var connections = Database.RetrieveAll<WorkflowConnectionEntity>().GroupToDictionary(a => a.From.Lane.Pool.Workflow.ToLite());

                        var result = Database.RetrieveAll<WorkflowEntity>().ToDictionary(workflow => workflow.ToLite(), workflow =>
                        {
                            var w = workflow.ToLite();
                            var nodeGraph = new WorkflowNodeGraph
                            {
                                Workflow = workflow,
                                Events = events.TryGetC(w).EmptyIfNull().ToDictionary(e => e.ToLite()),
                                Gateways = gateways.TryGetC(w).EmptyIfNull().ToDictionary(g => g.ToLite()),
                                Activities = activities.TryGetC(w).EmptyIfNull().ToDictionary(a => a.ToLite()),
                                Connections = connections.TryGetC(w).EmptyIfNull().ToDictionary(c => c.ToLite()),
                            };

                            nodeGraph.FillGraphs();
                            return nodeGraph;
                        });

                        return result;
                    }
                }, new InvalidateWith(typeof(WorkflowConnectionEntity)));

                Validator.PropertyValidator((WorkflowConnectionEntity c) => c.Condition).StaticPropertyValidation = (e, pi) =>
                {
                    if (e.Condition != null && e.From != null)
                    {
                        var conditionType = Conditions.Value.GetOrThrow(e.Condition).MainEntityType;
                        var workflowType = e.From.Lane.Pool.Workflow.MainEntityType;

                        if (!conditionType.Is(workflowType))
                            return WorkflowMessage.Condition0IsDefinedFor1Not2.NiceToString(conditionType, workflowType);
                    }

                    return null;
                };

                sb.Include<WorkflowConditionEntity>()
                   .WithSave(WorkflowConditionOperation.Save)
                   .WithQuery(dqm, () => e => new
                   {
                       Entity = e,
                       e.Id,
                       e.Name,
                       e.MainEntityType,
                       e.Eval.Script
                   });

                new Graph<WorkflowConditionEntity>.Delete(WorkflowConditionOperation.Delete)
                {
                    Delete = (e, _) =>
                    {
                        ThrowConnectionError(Database.Query<WorkflowConnectionEntity>().Where(a => a.Condition == e.ToLite()), e);
                        e.Delete();
                    },
                }.Register();

                new Graph<WorkflowConditionEntity>.ConstructFrom<WorkflowConditionEntity>(WorkflowConditionOperation.Clone)
                {
                    Construct = (e, args) =>
                    {
                        return new WorkflowConditionEntity
                        {
                            MainEntityType = e.MainEntityType,
                            Eval = new WorkflowConditionEval { Script = e.Eval.Script }
                        };
                    },
                }.Register();

                WorkflowEventTaskEntity.GetWorkflowEntity = lite => WorkflowGraphLazy.Value.GetOrThrow(lite).Workflow;

                Conditions = sb.GlobalLazy(() => Database.Query<WorkflowConditionEntity>().ToDictionary(a => a.ToLite()),
                    new InvalidateWith(typeof(WorkflowConditionEntity)));

                sb.Include<WorkflowActionEntity>()
                   .WithSave(WorkflowActionOperation.Save)
                   .WithQuery(dqm, () => e => new
                   {
                       Entity = e,
                       e.Id,
                       e.Name,
                       e.MainEntityType,
                       e.Eval.Script
                   });

                new Graph<WorkflowActionEntity>.Delete(WorkflowActionOperation.Delete)
                {
                    Delete = (e, _) =>
                    {
                        ThrowConnectionError(Database.Query<WorkflowConnectionEntity>().Where(a => a.Action == e.ToLite()), e);
                        e.Delete();
                    },
                }.Register();

                new Graph<WorkflowActionEntity>.ConstructFrom<WorkflowActionEntity>(WorkflowActionOperation.Clone)
                {
                    Construct = (e, args) =>
                    {
                        return new WorkflowActionEntity
                        {
                            MainEntityType = e.MainEntityType,
                            Eval = new WorkflowActionEval { Script = e.Eval.Script }
                        };
                    },
                }.Register();

                Actions = sb.GlobalLazy(() => Database.Query<WorkflowActionEntity>().ToDictionary(a => a.ToLite()),
                    new InvalidateWith(typeof(WorkflowActionEntity)));

                sb.Include<WorkflowScriptEntity>()
                 .WithSave(WorkflowScriptOperation.Save)
                 .WithQuery(dqm, () => s => new
                 {
                     Entity = s,
                     s.Id,
                     s.Name,
                     s.MainEntityType,
                 });

                new Graph<WorkflowScriptEntity>.Delete(WorkflowScriptOperation.Delete)
                {
                    Delete = (s, _) =>
                    {
                        ThrowConnectionError(Database.Query<WorkflowActivityEntity>().Where(a => a.Script.Script == s.ToLite()), s);
                        s.Delete();
                    },
                }.Register();

                Scripts = sb.GlobalLazy(() => Database.Query<WorkflowScriptEntity>().ToDictionary(a => a.ToLite()),
                    new InvalidateWith(typeof(WorkflowScriptEntity)));

                sb.Include<WorkflowScriptRetryStrategyEntity>()
                    .WithSave(WorkflowScriptRetryStrategyOperation.Save)
                    .WithDelete(WorkflowScriptRetryStrategyOperation.Delete)
                    .WithQuery(dqm, () => e => new
                    {
                        Entity = e,
                        e.Id,
                        e.Rule
                    });
            }
        }


        private static void ThrowConnectionError(IQueryable<WorkflowConnectionEntity> queryable, Entity toDelete)
        {
            if (queryable.Count() == 0)
                return;

            var errors = queryable.Select(a => new { Connection = a.ToLite(), From = a.From.ToLite(), To = a.To.ToLite(), Workflow = a.From.Lane.Pool.Workflow.ToLite() }).ToList();

            var formattedErrors = errors.GroupBy(a => a.Workflow).ToString(gr => $"Workflow '{gr.Key}':" +
                  gr.ToString(a => $"Connection {a.Connection.Id} ({a.Connection}): {a.From} -> {a.To}", "\r\n").Indent(4),
                "\r\n\r\n").Indent(4);

            throw new ApplicationException($"Impossible to delete '{toDelete}' because is used in some connections: \r\n" + formattedErrors);
        }

        private static void ThrowConnectionError(IQueryable<WorkflowActivityEntity> queryable, Entity toDelete)
        {
            if (queryable.Count() == 0)
                return;

            var errors = queryable.Select(a => new { Activity = a.ToLite(), Workflow = a.Lane.Pool.Workflow.ToLite() }).ToList();

            var formattedErrors = errors.GroupBy(a => a.Workflow).ToString(gr => $"Workflow '{gr.Key}':" +
                  gr.ToString(a => $"Activity {a.Activity}", "\r\n").Indent(4),
                "\r\n\r\n").Indent(4);

            throw new ApplicationException($"Impossible to delete '{toDelete}' because is used in some activities: \r\n" + formattedErrors);
        }


        public class WorkflowGraph : Graph<WorkflowEntity>
        {
            public static void Register()
            {
                new Execute(WorkflowOperation.Save)
                {
                    AllowsNew = true,
                    Lite = false,
                    Execute = (e, args) =>
                    {
                        WorkflowLogic.ApplyDocument(e, args.GetArg<WorkflowModel>(), args.TryGetArgC<WorkflowReplacementModel>());
                    }
                }.Register();

                new ConstructFrom<WorkflowEntity>(WorkflowOperation.Clone)
                {
                    Construct = (w, args) =>
                    {
                        WorkflowBuilder wb = new WorkflowBuilder(w);

                        var result = wb.Clone();

                        return result;
                    }
                }.Register();

                new Delete(WorkflowOperation.Delete)
                {
                    CanDelete = w => 
                    {
                        var usedWorkflows = Database.Query<CaseEntity>()
                                                .Where(c => c.Workflow.Is(w) && c.ParentCase != null)
                                                .Select(c => c.ParentCase.Workflow)
                                                .ToList();

                        if (usedWorkflows.Any())
                            return WorkflowMessage.WorkflowUsedIn0ForDecompositionOrCallWorkflow.NiceToString(usedWorkflows.ToString(", "));

                        return null;
                    },

                    Delete = (w, _) =>
                    {
                        var wb = new WorkflowBuilder(w);
                        wb.Delete();
                    }
                }.Register();
            }
        }

        public static ResetLazy<Dictionary<Lite<WorkflowConditionEntity>, WorkflowConditionEntity>> Conditions;
        public static WorkflowConditionEntity RetrieveFromCache(this Lite<WorkflowConditionEntity> wc)
        {
            return WorkflowLogic.Conditions.Value.GetOrThrow(wc);
        }

        public static ResetLazy<Dictionary<Lite<WorkflowActionEntity>, WorkflowActionEntity>> Actions;
        public static WorkflowActionEntity RetrieveFromCache(this Lite<WorkflowActionEntity> wa)
        {
            return WorkflowLogic.Actions.Value.GetOrThrow(wa);
        }

        public static ResetLazy<Dictionary<Lite<WorkflowScriptEntity>, WorkflowScriptEntity>> Scripts;
        public static WorkflowScriptEntity RetrieveFromCache(this Lite<WorkflowScriptEntity> ws)
        {
            return WorkflowLogic.Scripts.Value.GetOrThrow(ws);
        }

        public static Expression<Func<Lite<Entity>, UserEntity, bool>> IsCurrentUserActor = (actor, user) =>
           actor.RefersTo(user) ||
           actor.Is(user.Role);


        public static List<WorkflowEntity> GetAllowedStarts()
        {
            return (from w in Database.Query<WorkflowEntity>()
                    let s = w.WorkflowEvents().Single(a => a.Type == WorkflowEventType.Start)
                    let a = (WorkflowActivityEntity)s.NextConnections().Single().To
                    where a.Lane.Actors.Any(a => IsCurrentUserActor.Evaluate(a, UserEntity.Current))
                    select w).ToList();
        }

        public static WorkflowModel GetWorkflowModel(WorkflowEntity workflow)
        {
            var wb = new WorkflowBuilder(workflow);
            return wb.GetWorkflowModel();
        }

        public static PreviewResult PreviewChanges(WorkflowEntity workflow, WorkflowModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var document = XDocument.Parse(model.DiagramXml);
            var wb = new WorkflowBuilder(workflow);
            return wb.PreviewChanges(document, model);
        }

        public static void ApplyDocument(WorkflowEntity workflow, WorkflowModel model, WorkflowReplacementModel replacements)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));


            var wb = new WorkflowBuilder(workflow);
            if (workflow.IsNew)
                workflow.Save();

            wb.ApplyChanges(model, replacements);
            wb.ValidateGraph();
            workflow.FullDiagramXml = new WorkflowXmlEmbedded { DiagramXml = wb.GetXDocument().ToString() };
            workflow.Save();
        }
    }
}
