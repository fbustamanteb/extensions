﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Signum.Entities.Authorization;
using Signum.Entities.Basics;
using Signum.Utilities;
using Signum.Utilities.ExpressionTrees;

namespace Signum.Entities.Help
{
    [Serializable, EntityKind(EntityKind.Main, EntityData.Master)]
    public class NamespaceHelpEntity : Entity
    {
        [NotNullable, SqlDbType(Size = 300)]
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 300)]
        public string Name { get; set; }

        [NotNullable]
        [NotNullValidator]
        public CultureInfoEntity Culture { get; set; }

        [NotNullable, SqlDbType(Size = 200)]
        public string Title { get; set; }

        [SqlDbType(Size = int.MaxValue)]
		[StringLengthValidator(AllowNulls = true, Min = 3, MultiLine = true)]
        public string Description { get; set; }

        static Expression<Func<NamespaceHelpEntity, string>> ToStringExpression = e => e.Name;
        [ExpressionField]
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    [AutoInit]
    public static class NamespaceHelpOperation
    {
        public static ExecuteSymbol<NamespaceHelpEntity> Save;
    }


}
