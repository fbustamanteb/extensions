﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.Scheduler.Views
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
    
    #line 1 "..\..\Scheduler\Views\CustomTask.cshtml"
    using Signum.Engine;
    
    #line default
    #line hidden
    using Signum.Entities;
    
    #line 2 "..\..\Scheduler\Views\CustomTask.cshtml"
    using Signum.Entities.Scheduler;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.4.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Scheduler/Views/CustomTask.cshtml")]
    public partial class CustomTask : System.Web.Mvc.WebViewPage<dynamic>
    {
        public CustomTask()
        {
        }
        public override void Execute()
        {


WriteLiteral("\r\n");


            
            #line 4 "..\..\Scheduler\Views\CustomTask.cshtml"
 using (var e = Html.TypeContext<CustomTaskDN>()) 
{
    
            
            #line default
            #line hidden
            
            #line 6 "..\..\Scheduler\Views\CustomTask.cshtml"
Write(Html.ValueLine(e, f => f.Key));

            
            #line default
            #line hidden
            
            #line 6 "..\..\Scheduler\Views\CustomTask.cshtml"
                                   
}
            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591
