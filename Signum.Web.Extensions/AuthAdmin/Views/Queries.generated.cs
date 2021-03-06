﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
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
    
    #line 2 "..\..\AuthAdmin\Views\Queries.cshtml"
    using Signum.Engine.Authorization;
    
    #line default
    #line hidden
    using Signum.Entities;
    using Signum.Entities.Authorization;
    using Signum.Utilities;
    using Signum.Web;
    using Signum.Web.Auth;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/AuthAdmin/Views/Queries.cshtml")]
    public partial class _AuthAdmin_Views_Queries_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _AuthAdmin_Views_Queries_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 1 "..\..\AuthAdmin\Views\Queries.cshtml"
Write(Html.ScriptCss("~/authAdmin/Content/AuthAdmin.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 3 "..\..\AuthAdmin\Views\Queries.cshtml"
 using (var tc = Html.TypeContext<QueryRulePack>())
{

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"form-compact\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 6 "..\..\AuthAdmin\Views\Queries.cshtml"
   Write(Html.EntityLine(tc, f => f.Role));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 7 "..\..\AuthAdmin\Views\Queries.cshtml"
   Write(Html.ValueLine(tc, f => f.Strategy));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 8 "..\..\AuthAdmin\Views\Queries.cshtml"
   Write(Html.EntityLine(tc, f => f.Type));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

WriteLiteral("    <table");

WriteLiteral(" class=\"sf-auth-rules\"");

WriteLiteral(" id=\"queries\"");

WriteLiteral(">\r\n        <thead>\r\n            <tr>\r\n                <th>\r\n");

WriteLiteral("                    ");

            
            #line 14 "..\..\AuthAdmin\Views\Queries.cshtml"
               Write(typeof(Signum.Entities.Basics.QueryEntity).NiceName());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n                <th>\r\n");

WriteLiteral("                    ");

            
            #line 17 "..\..\AuthAdmin\Views\Queries.cshtml"
               Write(AuthAdminMessage.Allow.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n                <th>\r\n");

WriteLiteral("                    ");

            
            #line 20 "..\..\AuthAdmin\Views\Queries.cshtml"
               Write(AuthAdminMessage.Deny.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n                <th>\r\n");

WriteLiteral("                    ");

            
            #line 23 "..\..\AuthAdmin\Views\Queries.cshtml"
               Write(AuthAdminMessage.Overriden.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n");

            
            #line 27 "..\..\AuthAdmin\Views\Queries.cshtml"
        
            
            #line default
            #line hidden
            
            #line 27 "..\..\AuthAdmin\Views\Queries.cshtml"
         foreach (var item in tc.TypeElementContext(p => p.Rules))
        {

            
            #line default
            #line hidden
WriteLiteral("            <tr>\r\n                <td>\r\n");

WriteLiteral("                    ");

            
            #line 31 "..\..\AuthAdmin\Views\Queries.cshtml"
               Write(Html.Span(null, item.Value.Resource.Key));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 32 "..\..\AuthAdmin\Views\Queries.cshtml"
               Write(Html.Hidden(item.Compose("Resource_Key"), item.Value.Resource.Key));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 33 "..\..\AuthAdmin\Views\Queries.cshtml"
               Write(Html.Hidden(item.Compose("AllowedBase"), item.Value.AllowedBase));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n                <td>\r\n");

            
            #line 36 "..\..\AuthAdmin\Views\Queries.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 36 "..\..\AuthAdmin\Views\Queries.cshtml"
                     if (!item.Value.CoercedValues.Contains(true))
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <a");

WriteLiteral(" class=\"sf-auth-chooser sf-auth-allowed\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 39 "..\..\AuthAdmin\Views\Queries.cshtml"
                       Write(Html.RadioButton(item.Compose("Allowed"), "True", item.Value.Allowed));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </a>\r\n");

            
            #line 41 "..\..\AuthAdmin\Views\Queries.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n                <td>\r\n");

            
            #line 44 "..\..\AuthAdmin\Views\Queries.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 44 "..\..\AuthAdmin\Views\Queries.cshtml"
                     if (!item.Value.CoercedValues.Contains(false))
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <a");

WriteLiteral(" class=\"sf-auth-chooser sf-auth-not-allowed\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 47 "..\..\AuthAdmin\Views\Queries.cshtml"
                       Write(Html.RadioButton(item.Compose("Allowed"), "False", !item.Value.Allowed));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </a> \r\n");

            
            #line 49 "..\..\AuthAdmin\Views\Queries.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </td>\r\n                <td>\r\n");

WriteLiteral("                    ");

            
            #line 52 "..\..\AuthAdmin\Views\Queries.cshtml"
               Write(Html.CheckBox(item.Compose("Overriden"), item.Value.Overriden, new { disabled = "disabled", @class = "sf-auth-overriden" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n");

            
            #line 55 "..\..\AuthAdmin\Views\Queries.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </table>\r\n");

            
            #line 57 "..\..\AuthAdmin\Views\Queries.cshtml"
}
            
            #line default
            #line hidden
WriteLiteral(" ");

        }
    }
}
#pragma warning restore 1591
