﻿using Microsoft.SqlServer.Types;
using Signum.Engine;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Maps;
using Signum.Engine.Operations;
using Signum.Entities;
using Signum.Entities.Tree;
using Signum.Utilities;
using Signum.Utilities.ExpressionTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Signum.Engine.Tree
{  
    public static class TreeLogic
    {
        public class DescendatsMethodExpander<T> : GenericMethodExpander
            where T: TreeEntity
        {
            public static Expression<Func<T, IQueryable<T>>> Expression =
                cp => Database.Query<T>().Where(cc => (bool)cc.Route.IsDescendantOf(cp.Route));
            public DescendatsMethodExpander() : base(Expression) { }
        }
        [MethodExpander(typeof(DescendatsMethodExpander<>))]
        public static IQueryable<T> Descendants<T>(this T e)
            where T : TreeEntity
        {
            return DescendatsMethodExpander<T>.Expression.Evaluate(e);
        }

     
        public class ChildrensMethodExpander<T> : GenericMethodExpander
            where T: TreeEntity
        {
            public static Expression<Func<T, IQueryable<T>>> Expression =
                cp => Database.Query<T>().Where(cc => (bool)(cc.Route.GetAncestor(1) == cp.Route));
            public ChildrensMethodExpander() : base(Expression) { }
        }
        [MethodExpander(typeof(ChildrensMethodExpander<>))]
        public static IQueryable<T> Children<T>(this T e)
             where T : TreeEntity
        {
            return ChildrensMethodExpander<T>.Expression.Evaluate(e);
        }

       
        public class AscendantsMethodExpander<T> : GenericMethodExpander
            where T: TreeEntity
        {
            public static Expression<Func<T, IQueryable<T>>> Expression =
                cc => Database.Query<T>().Where(cp => (bool)cc.Route.IsDescendantOf(cp.Route)).OrderBy(cp => (Int16)cp.Route.GetLevel());
            public AscendantsMethodExpander() : base(Expression) { }
        }
        [MethodExpander(typeof(AscendantsMethodExpander<>))]
        public static IQueryable<T> Ascendants<T>(this T e)
             where T : TreeEntity
        {
            return AscendantsMethodExpander<T>.Expression.Evaluate(e);
        }

     
        public class ParentMethodExpander<T> : GenericMethodExpander
            where T : TreeEntity
        {
            public static Expression<Func<T, T>> Expression = 
                cc => Database.Query<T>().SingleOrDefaultEx(cp => (bool)(cp.Route == cc.Route.GetAncestor(1)));
            public ParentMethodExpander() : base(Expression) { }
        }
        [MethodExpander(typeof(ParentMethodExpander<>))]
        public static T Parent<T>(this T e)
             where T : TreeEntity
        {
            return ParentMethodExpander<T>.Expression.Evaluate(e);
        }
       
        public static Expression<Func<TreeEntity, short>> LevelExpression =
                cc => (short)cc.Route.GetLevel();
        [ExpressionField]
        public static short Level(this TreeEntity e)
        {
            return LevelExpression.Evaluate(e);
        }

        public static void CalculateFullName<T>(T tree)
            where T : TreeEntity
        {
            tree.SetFullName(tree.Ascendants().Select(a => a.Name).ToString(" > "));
        }

        static SqlHierarchyId LastChild<T>(SqlHierarchyId node)
            where T : TreeEntity
        {
            using (ExecutionMode.Global())
                return Database.Query<T>()
                    .Select(c => (SqlHierarchyId?)c.Route)
                    .Where(n => (bool)((SqlHierarchyId)n.Value.GetAncestor(1) == node))
                    .OrderByDescending(n => n).FirstOrDefault() ?? SqlHierarchyId.Null;
        }

        private static SqlHierarchyId Next<T>(SqlHierarchyId node)
            where T : TreeEntity
        {
            using (ExecutionMode.Global())
                return Database.Query<T>()
                    .Select(t => (SqlHierarchyId?)t.Route)
                    .Where(n => (bool)(n.Value.GetAncestor(1) == node.GetAncestor(1)) && (bool)(n.Value > node))
                    .OrderBy(n => n).FirstOrDefault() ?? SqlHierarchyId.Null;
        }


        internal static int RemoveDescendants<T>(T f)
            where T : TreeEntity
        {
            return f.Descendants().UnsafeDelete();
        }


        public static void FixName<T>(T f)
            where T : TreeEntity
        {
            if (f.IsNew)
            {
                f.SetFullName(f.Name);
                f.Save();
                CalculateFullName(f);
            }
            else
            {
                f.Save();
                CalculateFullName(f);

                if (f.IsGraphModified)
                {
                    var list = f.Descendants().Where(c => c != f).ToList();
                    foreach (T h in list)
                    {
                        CalculateFullName(h);
                        h.Save();
                    }
                }
            }
        }

        internal static void FixRouteAndNames<T>(T f, Lite<T> parent)
            where T : TreeEntity
        {
            var list = f.Descendants().Where(c => c != f).ToList();

            var oldNode = f.Route;

            var catRoute = parent.InDB().Select(a => a.Route).SingleEx();

            f.Route = catRoute.GetDescendant(LastChild<T>(catRoute), SqlHierarchyId.Null);

            f.Save();
            CalculateFullName(f);
            f.Save();

            foreach (T h in list)
            {
                h.Route = h.Route.GetReparentedValue(oldNode, f.Route);
                h.Save();
                CalculateFullName(h);
                h.Save();
            }
        }

        public static FluentInclude<T> WithTree<T>(this FluentInclude<T> include, DynamicQueryManager dqm) where T : TreeEntity, new()
        {
            RegisterExpressions<T>(dqm);
            RegisterOperations<T>();
            include.WithUniqueIndex(n => new { n.ParentRoute, n.Name });
            return include;
        }

        public static void RegisterExpressions<T>(DynamicQueryManager dqm)
            where T : TreeEntity
        {
            dqm.RegisterExpression((T c) => c.Children(), () => TreeMessage.Children.NiceToString());
            dqm.RegisterExpression((T c) => c.Parent(), () => TreeMessage.Parent.NiceToString());
            dqm.RegisterExpression((T c) => c.Descendants(), () => TreeMessage.Descendants.NiceToString());
            dqm.RegisterExpression((T c) => c.Ascendants(), () => TreeMessage.Ascendants.NiceToString());
            dqm.RegisterExpression((T c) => c.Level(), () => TreeMessage.Level.NiceToString());
        }

        public static void RegisterOperations<T>() where T : TreeEntity, new()
        {
            Graph<T>.Construct.Untyped(TreeOperation.CreateRoot).Do(c =>
            {
                c.Construct = (_) => new T
                {
                    ParentOrSibling = null,
                    Level = 0,
                    IsSibling = false
                };
                c.Register();
            });

            Graph<T>.ConstructFrom<T>.Untyped(TreeOperation.CreateChild).Do(c =>
            {
                c.Construct = (t, _) => new T
                {
                    ParentOrSibling = t.ToLite(),
                    Level = t.Level + 1,
                    IsSibling = false
                    //                    
                };
                c.Register();
            });

            Graph<T>.ConstructFrom<T>.Untyped(TreeOperation.CreateNextSibling).Do(c =>
            {
                c.Construct = (t, _) => new T
                {
                    ParentOrSibling = t.ToLite(),
                    Level = t.Level,
                    IsSibling = true
                    //                    
                };
                c.Register();
            });

            new Graph<T>.Execute(TreeOperation.Save)
            {
                AllowsNew = true,
                Lite = false,
                Execute = (t, _) =>
                {
                    if (t.IsNew)
                    {
                        t.Route = CalculateRoute(t);                       
                    }

                    TreeLogic.FixName(t);
                }
            }.Register();

            new Graph<T>.Execute(TreeOperation.Move)
            {
                Execute = (f, args) =>
                {
                    var parent = args.GetArg<Lite<T>>();

                    TreeLogic.FixRouteAndNames(f, parent);
                }
            }.Register();

            new Graph<T>.Delete(TreeOperation.Delete)
            {
                Delete = (f, args) =>
                {
                    TreeLogic.RemoveDescendants(f);
                }
            }.Register();
        }

        public static SqlHierarchyId CalculateRoute<T>(T t) where T : TreeEntity, new()
        {
            if (!t.IsSibling)
            {
                if (t.ParentOrSibling == null)
                    return SqlHierarchyId.GetRoot().GetDescendant(LastChild<T>(SqlHierarchyId.GetRoot()), SqlHierarchyId.Null);

                var parentRoute = t.ParentOrSibling.InDB(p => p.Route);
                return parentRoute.GetDescendant(LastChild<T>(parentRoute), SqlHierarchyId.Null);
            }
            else
            {
                var siblingRoute = t.ParentOrSibling.InDB(p => p.Route);
                return siblingRoute.GetAncestor(1).GetDescendant(siblingRoute, Next<T>(siblingRoute));
            }
        }
    }
}
