﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading;
using Signum.Entities.Authorization;
using Signum.Engine;
using Signum.Engine.Authorization;
using Signum.Services;
using Signum.Utilities;
using Signum.Web.Extensions.Properties;
using System.Net.Mail;
using System.Net;
using System.Text;
using Signum.Entities;
using Signum.Web.Controllers;
using System.IO;
using System.Xml;

namespace Signum.Web.AuthAdmin
{
    public class AuthAdminController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (UserDN.Current != null)
                BasicPermissions.AdminRules.Authorize();
        }

        public ViewResult Permissions(Lite<RoleDN> role)
        {
            return Navigator.View(this, PermissionAuthLogic.GetPermissionRules(role.FillToStr()), true);
        }

        [HttpPost]
        public ActionResult Permissions(FormCollection form)
        {
            Lite<RoleDN> role = this.ExtractLite<RoleDN>("Role");

            var prp = PermissionAuthLogic.GetPermissionRules(role).ApplyChanges(ControllerContext, "", true); ;

            PermissionAuthLogic.SetPermissionRules(prp.Value);

            return RedirectToAction("Permissions", new { role = role.Id });
        }


        public ViewResult FacadeMethods(Lite<RoleDN> role)
        {
            return Navigator.View(this, FacadeMethodAuthLogic.GetFacadeMethodRules(role.FillToStr()), true);
        }

        [HttpPost]
        public ActionResult FacadeMethods(FormCollection form)
        {
            Lite<RoleDN> role = this.ExtractLite<RoleDN>("Role");

            var prp = FacadeMethodAuthLogic.GetFacadeMethodRules(role).ApplyChanges(ControllerContext, "", true); ;

            FacadeMethodAuthLogic.SetFacadeMethodRules(prp.Value);

            return RedirectToAction("FacadeMethods", new { role = role.Id });
        }


        public ViewResult EntityGroups(Lite<RoleDN> role)
        {
            return Navigator.View(this, EntityGroupAuthLogic.GetEntityGroupRules(role.FillToStr()), true);
        }

        [HttpPost]
        public ActionResult EntityGroups(FormCollection form)
        {
            Lite<RoleDN> role = this.ExtractLite<RoleDN>("Role");

            var prp = EntityGroupAuthLogic.GetEntityGroupRules(role).ApplyChanges(ControllerContext, "", true); ;

            EntityGroupAuthLogic.SetEntityGroupRules(prp.Value);

            return RedirectToAction("EntityGroups", new { role = role.Id });
        }


        public ViewResult Types(Lite<RoleDN> role)
        {
            return Navigator.View(this, TypeAuthLogic.GetTypeRules(role.FillToStr()), true);
        }

        [HttpPost]
        public ActionResult Types(FormCollection form)
        {
            Lite<RoleDN> role = this.ExtractLite<RoleDN>("Role");

            var prp = TypeAuthLogic.GetTypeRules(role).ApplyChanges(ControllerContext, "", true); ;

            TypeAuthLogic.SetTypeRules(prp.Value);

            return RedirectToAction("Types", new { role = role.Id });
        }

        [HttpPost]
        public ActionResult Properties(Lite<RoleDN> role, Lite<TypeDN> type, string prefix)
        {
            ViewData[ViewDataKeys.WriteSFInfo] = true;
            return Navigator.PopupView(this, PropertyAuthLogic.GetPropertyRules(role.FillToStr(), type.Retrieve()), prefix);
        }

        [HttpPost]
        public ActionResult SaveProperties(FormCollection form, string prefix)
        {
            Lite<RoleDN> role = this.ExtractLite<RoleDN>(TypeContextUtilities.Compose(prefix, "Role"));
            TypeDN type = this.ExtractEntity<TypeDN>(TypeContextUtilities.Compose(prefix, "Type"));

            var prp = PropertyAuthLogic.GetPropertyRules(role, type).ApplyChanges(ControllerContext, prefix, true); ;

            PropertyAuthLogic.SetPropertyRules(prp.Value);

            return JsonAction.ModelState(ModelState);
        }

        [HttpPost]
        public ActionResult Queries(Lite<RoleDN> role, Lite<TypeDN> type, string prefix)
        {
            ViewData[ViewDataKeys.WriteSFInfo] = true;
            return Navigator.PopupView(this, QueryAuthLogic.GetQueryRules(role.FillToStr(), type.Retrieve()), prefix);
        }

        [HttpPost]
        public JsonResult SaveQueries(FormCollection form, string prefix)
        {
            Lite<RoleDN> role = this.ExtractLite<RoleDN>(TypeContextUtilities.Compose(prefix, "Role"));
            TypeDN type = this.ExtractEntity<TypeDN>(TypeContextUtilities.Compose(prefix, "Type"));

            var prp = QueryAuthLogic.GetQueryRules(role, type).ApplyChanges(ControllerContext, prefix, true); ;

            QueryAuthLogic.SetQueryRules(prp.Value);

            return JsonAction.ModelState(ModelState);
        }


        [HttpPost]
        public ActionResult Operations(Lite<RoleDN> role, Lite<TypeDN> type, string prefix)
        {
            ViewData[ViewDataKeys.WriteSFInfo] = true;
            return Navigator.PopupView(this, OperationAuthLogic.GetOperationRules(role.FillToStr(), type.Retrieve()), prefix);
        }

        [HttpPost]
        public ActionResult SaveOperations(FormCollection form, string prefix)
        {
            Lite<RoleDN> role = this.ExtractLite<RoleDN>(TypeContextUtilities.Compose(prefix, "Role"));
            TypeDN type = this.ExtractEntity<TypeDN>(TypeContextUtilities.Compose(prefix, "Type"));

            var prp = OperationAuthLogic.GetOperationRules(role, type).ApplyChanges(ControllerContext, prefix, true);

            OperationAuthLogic.SetOperationRules(prp.Value);

            return JsonAction.ModelState(ModelState);
        }

        [HttpGet]
        public FileResult Export()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                AuthLogic.ExportRules().Save(ms);

                return File(ms.ToArray(), MimeType.FromExtension("xml"), "AuthRules.xml");
            }
        }
    }
}
