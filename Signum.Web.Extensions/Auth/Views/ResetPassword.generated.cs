#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.Auth.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Signum.Entities;
    using Signum.Entities.Authorization;
    using Signum.Utilities;
    using Signum.Web;
    using Signum.Web.Auth;
        
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Auth/Views/ResetPassword.cshtml")]
    public class ResetPassword : System.Web.Mvc.WebViewPage<dynamic>
    {
        public ResetPassword()
        {
        }
        public override void Execute()
        {

            
            #line 1 "..\..\Auth\Views\ResetPassword.cshtml"
  
    ViewBag.Title = AuthMessage.ResetPassword.NiceToString();


            
            #line default
            #line hidden
WriteLiteral("<script type=\"text/javascript\">    $(function () { $(\"#email\").focus(); }); </scr" +
"ipt>\r\n<div class=\"sf-reset-password-container\">\r\n    <h2>");


            
            #line 6 "..\..\Auth\Views\ResetPassword.cshtml"
   Write(AuthMessage.ResetPassword.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("</h2>\r\n    <p>");


            
            #line 7 "..\..\Auth\Views\ResetPassword.cshtml"
  Write(AuthMessage.ForgotYourPassword.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("</p>\r\n    ");


            
            #line 8 "..\..\Auth\Views\ResetPassword.cshtml"
Write(Html.ValidationSummary());

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 9 "..\..\Auth\Views\ResetPassword.cshtml"
     using (Html.BeginForm())
        {

            
            #line default
            #line hidden
WriteLiteral("        <div class=\"sf-reset-password-form\">\r\n            <label for=\"email\">\r\n  " +
"              Email</label>:\r\n            ");


            
            #line 14 "..\..\Auth\Views\ResetPassword.cshtml"
       Write(Html.TextBox("email", "", new { size = 30 }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            ");


            
            #line 15 "..\..\Auth\Views\ResetPassword.cshtml"
       Write(Html.ValidationMessage("email"));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <input class=\"sf-button\" type=\"submit\" />\r\n        </div>\r\n");


            
            #line 18 "..\..\Auth\Views\ResetPassword.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");


        }
    }
}
#pragma warning restore 1591
