﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Signum.Services;
using Signum.Utilities;
using Signum.Entities;
using Signum.Web;
using Signum.Engine;
using Signum.Engine.Operations;
using Signum.Entities.Operations;
using Signum.Engine.Basics;

namespace Signum.Web.Operations
{
    [HandleError]
    public class OperationController : Controller
    {
        public ActionResult OperationExecute(string sfTypeName, int? sfId, string sfOperationFullKey, string prefix, string sfOnOk, string sfOnCancel)
        {
            Type type = Navigator.ResolveType(sfTypeName);

            IdentifiableEntity entity = null;
            if (sfId.HasValue)
                entity = Database.Retrieve(type, sfId.Value);
            else
                entity = (IdentifiableEntity)Navigator.CreateInstance(type);

            Dictionary<string, List<string>> errors = Navigator.ApplyChangesAndValidate(this, "", ref entity);

            if (errors != null && errors.Count > 0)
            {
                this.ModelState.FromDictionary(errors, Request.Form);
                return Content("{\"ModelState\":" + this.ModelState.ToJsonData() + "}");
            }

            entity = OperationLogic.ServiceExecute(entity, EnumLogic<OperationDN>.ToEnum(sfOperationFullKey));

            return Navigator.PopupView(this, entity, prefix);
        }

        public ActionResult ConstructFromManyExecute(string sfTypeName, string sfQueryName, string sfIds, string sfOperationFullKey, string prefix, string sfOnOk, string sfOnCancel)
        {
            Type type = Navigator.ResolveType(sfTypeName);

            List<Lazy> sourceEntities = null;
            if (sfIds.HasText())
            {
                string[] ids = sfIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (ids == null || ids.Length == 0)
                    throw new ArgumentException("Construct From Many Operation needs source Ids as parameter");
                sourceEntities = ids.Select(idstr => Lazy.Create(type, int.Parse(idstr))).ToList();
            }
            if (sourceEntities == null)
                throw new ArgumentException("Construct From Many Operation needs source Lazies");

            IdentifiableEntity entity = OperationLogic.ServiceConstructFromMany(sourceEntities, type, EnumLogic<OperationDN>.ToEnum(sfOperationFullKey));

            return Navigator.PopupView(this, entity, prefix);
        }
    }
}
