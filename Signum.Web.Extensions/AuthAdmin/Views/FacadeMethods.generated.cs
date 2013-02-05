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

namespace Signum.Web.Extensions.AuthAdmin.Views
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
    using Signum.Engine;
    using Signum.Entities;
    using Signum.Entities.Authorization;
    using Signum.Utilities;
    using Signum.Web;
    using Signum.Web.Auth;
    using Signum.Web.Extensions.Properties;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/AuthAdmin/Views/FacadeMethods.cshtml")]
    public class FacadeMethods : System.Web.Mvc.WebViewPage<dynamic>
    {
        public FacadeMethods()
        {
        }
        public override void Execute()
        {

            
            #line 1 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
Write(Html.DynamicCss("~/authAdmin/Content/SF_AuthAdmin.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 2 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
Write(Html.ScriptsJs("~/authAdmin/Scripts/SF_AuthAdmin.js"));

            
            #line default
            #line hidden
WriteLiteral("\r\n<script type=\"text/javascript\">\r\n    $(function () {\r\n        SF.Auth.coloredRa" +
"dios($(document));\r\n    });\r\n</script>\r\n");


            
            #line 8 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
 using (var tc = Html.TypeContext<FacadeMethodRulePack>())
{
    
            
            #line default
            #line hidden
            
            #line 10 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
Write(Html.EntityLine(tc, f => f.Role));

            
            #line default
            #line hidden
            
            #line 10 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
                                     
    
            
            #line default
            #line hidden
            
            #line 11 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
Write(Html.ValueLine(tc, f => f.DefaultRule, vl => { vl.UnitText = tc.Value.DefaultLabel; }));

            
            #line default
            #line hidden
            
            #line 11 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
                                                                                           


            
            #line default
            #line hidden
WriteLiteral("    <table class=\"sf-auth-rules\">\r\n        <thead>\r\n            <tr>\r\n           " +
"     <th>\r\n                    ");


            
            #line 17 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
               Write(AuthMessage.FacadeMethodsAscx_FacadeMethod.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n                <th>\r\n                    ");


            
            #line 20 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
               Write(AuthMessage.FacadeMethodsAscx_Allow.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n                <th>\r\n                    ");


            
            #line 23 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
               Write(AuthMessage.FacadeMethodsAscx_Deny.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n                <th>\r\n                    ");


            
            #line 26 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
               Write(AuthMessage.FacadeMethodsAscx_Overriden.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n");


            
            #line 30 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
         foreach (var item in tc.TypeElementContext(p => p.Rules))
        {

            
            #line default
            #line hidden
WriteLiteral("            <tr>\r\n                <td>\r\n                    ");


            
            #line 34 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
               Write(Html.Span(null, item.Value.Resource.ToString()));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    ");


            
            #line 35 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
               Write(Html.Hidden(item.Compose("Resource_Key"), item.Value.Resource.ToString()));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    ");


            
            #line 36 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
               Write(Html.Hidden(item.Compose("AllowedBase"), item.Value.AllowedBase));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    <a class=\"sf-a" +
"uth-chooser sf-auth-allowed\">\r\n                        ");


            
            #line 40 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
                   Write(Html.RadioButton(item.Compose("Allowed"), "True", item.Value.Allowed));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </a>\r\n                </td>\r\n                <td>\r\n        " +
"            <a class=\"sf-auth-chooser sf-auth-not-allowed\">\r\n                   " +
"     ");


            
            #line 45 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
                   Write(Html.RadioButton(item.Compose("Allowed"), "False", !item.Value.Allowed));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </a>\r\n                </td>\r\n                <td>\r\n        " +
"            ");


            
            #line 49 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
               Write(Html.CheckBox(item.Compose("Overriden"), item.Value.Overriden, new { disabled = "disabled", @class = "sf-auth-overriden" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n");


            
            #line 52 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </table>\r\n");


            
            #line 54 "..\..\AuthAdmin\Views\FacadeMethods.cshtml"
}
            
            #line default
            #line hidden
WriteLiteral(" ");


        }
    }
}
#pragma warning restore 1591
