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
    
    #line 1 "..\..\Auth\Views\SetPassword.cshtml"
    using Signum.Web.Auth;
    
    #line default
    #line hidden
    using Signum.Web.Extensions.Properties;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Auth/Views/SetPassword.cshtml")]
    public class SetPassword : System.Web.Mvc.WebViewPage<dynamic>
    {
        public SetPassword()
        {
        }
        public override void Execute()
        {

WriteLiteral("\r\n");


            
            #line 3 "..\..\Auth\Views\SetPassword.cshtml"
 using (var e = Html.TypeContext<SetPasswordModel>())
{ 
    
            
            #line default
            #line hidden
            
            #line 5 "..\..\Auth\Views\SetPassword.cshtml"
Write(Html.HiddenRuntimeInfo(e, sp => sp.User));

            
            #line default
            #line hidden
            
            #line 5 "..\..\Auth\Views\SetPassword.cshtml"
                                             
    
            
            #line default
            #line hidden
            
            #line 6 "..\..\Auth\Views\SetPassword.cshtml"
Write(Html.Field(AuthMessage.NewPassword.NiceToString(),  
    Html.TextBox(e.Compose(UserMapping.NewPasswordKey), "", new {type="password"})));

            
            #line default
            #line hidden
            
            #line 7 "..\..\Auth\Views\SetPassword.cshtml"
                                                                                   
    
            
            #line default
            #line hidden
            
            #line 8 "..\..\Auth\Views\SetPassword.cshtml"
Write(Html.Field(AuthMessage.ConfirmNewPassword.NiceToString(),  
    Html.TextBox(e.Compose(UserMapping.NewPasswordBisKey), "", new {type="password"})));

            
            #line default
            #line hidden
            
            #line 9 "..\..\Auth\Views\SetPassword.cshtml"
                                                                                       
}

            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591
