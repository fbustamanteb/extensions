﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.ControlPanel.Views.Admin
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
    
    #line 1 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
    using Signum.Entities.ControlPanel;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 2 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
    using Signum.Web.ControlPanel;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/ControlPanel/Views/Admin/PanelPartViewAdmin.cshtml")]
    public partial class PanelPartViewAdmin : System.Web.Mvc.WebViewPage<dynamic>
    {
        public PanelPartViewAdmin()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
 using (var tc = Html.TypeContext<PanelPartDN>())
{
    var part = tc.Value;
    var offset = part.StartColumn - (int)ViewData[GridRepeaterHelper.LastEnd];
    

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteAttribute("id", Tuple.Create(" id=\"", 245), Tuple.Create("\"", 297)
            
            #line 9 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
, Tuple.Create(Tuple.Create("", 250), Tuple.Create<System.Object, System.Int32>(tc.Compose(EntityRepeaterKeys.RepeaterElement)
            
            #line default
            #line hidden
, 250), false)
);

WriteAttribute("class", Tuple.Create(" class=\"", 298), Tuple.Create("\"", 392)
, Tuple.Create(Tuple.Create("", 306), Tuple.Create("sf-grid-element", 306), true)
, Tuple.Create(Tuple.Create(" ", 321), Tuple.Create("col-sm-", 322), true)
            
            #line 9 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
             , Tuple.Create(Tuple.Create("", 329), Tuple.Create<System.Object, System.Int32>(part.Columns
            
            #line default
            #line hidden
, 329), false)
            
            #line 9 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
                           , Tuple.Create(Tuple.Create(" ", 342), Tuple.Create<System.Object, System.Int32>(offset == 0 ? null : "col-sm-offset-" + offset
            
            #line default
            #line hidden
, 343), false)
);

WriteLiteral(">\r\n        <div");

WriteAttribute("class", Tuple.Create(" class=\"", 408), Tuple.Create("\"", 460)
, Tuple.Create(Tuple.Create("", 416), Tuple.Create("panel", 416), true)
, Tuple.Create(Tuple.Create(" ", 421), Tuple.Create("panel-", 422), true)
            
            #line 10 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
, Tuple.Create(Tuple.Create("", 428), Tuple.Create<System.Object, System.Int32>(part.Style.ToString().ToLower()
            
            #line default
            #line hidden
, 428), false)
);

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"panel-heading form-inline\"");

WriteLiteral(" draggable=\"true\"");

WriteLiteral(">\r\n                <a");

WriteAttribute("id", Tuple.Create(" id=\"", 552), Tuple.Create("\"", 581)
            
            #line 12 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
, Tuple.Create(Tuple.Create("", 557), Tuple.Create<System.Object, System.Int32>(tc.Compose("btnRemove")
            
            #line default
            #line hidden
, 557), false)
);

WriteLiteral(" class=\"sf-line-button sf-remove pull-right\"");

WriteAttribute("title", Tuple.Create(" \r\n                    title=\"", 626), Tuple.Create("\"", 699)
            
            #line 13 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
, Tuple.Create(Tuple.Create("", 656), Tuple.Create<System.Object, System.Int32>(EntityControlMessage.Remove.NiceToString()
            
            #line default
            #line hidden
, 656), false)
);

WriteLiteral(">\r\n                    <span");

WriteLiteral(" class=\"glyphicon glyphicon-remove\"");

WriteLiteral("></span>\r\n                </a>\r\n");

WriteLiteral("                ");

            
            #line 16 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
           Write(Html.ValueLine(tc, pp => pp.Title, vl => { vl.FormGroupStyle = FormGroupStyle.None; vl.ValueHtmlProps["placeholder"] = Html.PropertyNiceName(() => tc.Value.Title); }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 17 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
           Write(Html.ValueLine(tc, pp => pp.Style, vl => { vl.FormGroupStyle = FormGroupStyle.None; }));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("                ");

            
            #line 19 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
           Write(Html.HiddenRuntimeInfo(tc));

            
            #line default
            #line hidden
WriteLiteral("\r\n                ");

WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 23 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
           Write(Html.Hidden(tc.Compose("Row"), part.Row));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 24 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
           Write(Html.Hidden(tc.Compose("Columns"), part.Columns));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 25 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
           Write(Html.Hidden(tc.Compose("StartColumn"), part.StartColumn));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"panel-body\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 28 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
           Write(Html.HiddenRuntimeInfo(tc, pp => pp.Content));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 29 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
           Write(Html.EmbeddedControl(tc, pp => pp.Content, ecs => ecs.ViewName = ControlPanelClient.PanelPartViews[part.Content.GetType()].Admin));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"sf-leftHandle\"");

WriteLiteral(" draggable=\"true\"");

WriteLiteral("></div>\r\n            <div");

WriteLiteral(" class=\"sf-rightHandle\"");

WriteLiteral(" draggable=\"true\"");

WriteLiteral("></div>\r\n        </div>\r\n    </div>\r\n");

            
            #line 35 "..\..\ControlPanel\Views\Admin\PanelPartViewAdmin.cshtml"
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
