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
    
    #line 1 "..\..\Dashboard\Views\SearchControlPart.cshtml"
    using System.Configuration;
    
    #line default
    #line hidden
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
    
    #line 2 "..\..\Dashboard\Views\SearchControlPart.cshtml"
    using Signum.Entities.Dashboard;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Dashboard\Views\SearchControlPart.cshtml"
    using Signum.Entities.DynamicQuery;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Dashboard\Views\SearchControlPart.cshtml"
    using Signum.Entities.UserQueries;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 3 "..\..\Dashboard\Views\SearchControlPart.cshtml"
    using Signum.Web.Dashboard;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Dashboard\Views\SearchControlPart.cshtml"
    using Signum.Web.UserQueries;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Dashboard/Views/SearchControlPart.cshtml")]
    public partial class _Dashboard_Views_SearchControlPart_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Dashboard_Views_SearchControlPart_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 8 "..\..\Dashboard\Views\SearchControlPart.cshtml"
 using (var tc = Html.TypeContext<UserQueryPartEntity>())
{
    UserQueryEntity uq = tc.Value.UserQuery;
    object queryName = Finder.Manager.QuerySettings.Keys.FirstEx(k => QueryUtils.GetKey(k) == uq.Query.Key);
    
    FindOptions fo = new FindOptions(queryName)
    {
        ShowHeader = false,
        ShowFooter = false,
        AllowSelection = tc.Value.AllowSelection,
        SearchOnLoad = true,
    };
    
    
            
            #line default
            #line hidden
            
            #line 21 "..\..\Dashboard\Views\SearchControlPart.cshtml"
Write(Html.SearchControl(uq, fo, new Context(null, Model.Prefix), sc => sc.AvoidFullScreenButton = true));

            
            #line default
            #line hidden
            
            #line 21 "..\..\Dashboard\Views\SearchControlPart.cshtml"
                                                                                                       ;
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
