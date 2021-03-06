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
    
    #line 3 "..\..\Chart\Views\ChartScriptCode.cshtml"
    using Signum.Engine;
    
    #line default
    #line hidden
    using Signum.Entities;
    
    #line 1 "..\..\Chart\Views\ChartScriptCode.cshtml"
    using Signum.Entities.Chart;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 4 "..\..\Chart\Views\ChartScriptCode.cshtml"
    using Signum.Web.Chart;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Chart\Views\ChartScriptCode.cshtml"
    using Signum.Web.Files;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Chart/Views/ChartScriptCode.cshtml")]
    public partial class _Chart_Views_ChartScriptCode_cshtml : System.Web.Mvc.WebViewPage<TypeContext<ChartScriptEntity>>
    {
        public _Chart_Views_ChartScriptCode_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 6 "..\..\Chart\Views\ChartScriptCode.cshtml"
Write(Html.ScriptCss(
    "~/Libs/CodeMirror/lib/codemirror.css",
    "~/Libs/CodeMirror/addon/dialog/dialog.css",
    "~/Libs/CodeMirror/addon/display/fullscreen.css",
    "~/Libs/CodeMirror/addon/hint/show-hint.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(@">
      .CodeMirror {
        border: 1px solid #eee;
      }
      
      span.CodeMirror-matchhighlight 
      { 
          background: #efefef
      }
      .CodeMirror-focused span.CodeMirror-matchhighlight 
      { 
          background: #efe7ff !important 
      }
      
      .exceptionLine {background: #FFFF00 !important;}
    </style>
    <pre");

WriteLiteral(" style=\"color: Green; overflow-wrap: inherit;\"");

WriteLiteral(@">
//var chart = d3.select('#sfChartControl .sf-chart-container').append('svg:svg').attr('width', width).attr('height', height))
//var data = { 
//              ""columns"": { ""c0"": { ""title"":""Product"", ""token"":""Product"", ""isGroupKey"":true, ... }, 
                             ""c1"": { ""title"":""Count"", ""token"":""Count"", ""isGroupKey"":true, ...} 
                          },
//              ""rows"": [ { ""c0"": { ""key"": ""Product;1"", ""toStr"": ""Apple"", ""color"": null }, ""c1"": { ""key"": ""140"", ""toStr"": ""140"" } },
//                        { ""c0"": { ""key"": ""Product;2"", ""toStr"": ""Orange"", ""color"": null }, ""c1"": { ""key"": ""179"", ""toStr"": ""179"" } }, ...
//                      ]
//           }
// DrawChart(chart, data);
// 
// Visit: http://d3js.org/
// Other functions defined in: \Chart\Scripts\ChartUtils.js
// use 'debugger' keyword or just throw JSON.stringify(myVariable)
// All yours!...
 </pre>
");

WriteLiteral("    ");

            
            #line 44 "..\..\Chart\Views\ChartScriptCode.cshtml"
Write(Html.ValueLine(Model, c => c.Script, vl => { vl.ValueLineType = ValueLineType.TextArea; vl.FormGroupStyle = FormGroupStyle.None; }));

            
            #line default
            #line hidden
WriteLiteral("\r\n \r\n    <script>\r\n        require([\"");

            
            #line 47 "..\..\Chart\Views\ChartScriptCode.cshtml"
             Write(ChartClient.ModuleScript);

            
            #line default
            #line hidden
WriteLiteral(@""",
            ""Libs/CodeMirror/lib/codemirror"",
            ""Libs/CodeMirror/mode/javascript/javascript"",
            ""Libs/CodeMirror/addon/comment/comment"",
            ""Libs/CodeMirror/addon/comment/continuecomment"",
            ""Libs/CodeMirror/addon/dialog/dialog"",
            ""Libs/CodeMirror/addon/display/fullscreen"",
            ""Libs/CodeMirror/addon/edit/closebrackets"",
            ""Libs/CodeMirror/addon/edit/matchbrackets"",
            ""Libs/CodeMirror/addon/hint/show-hint"",
            ""Libs/CodeMirror/addon/hint/javascript-hint"",
            ""Libs/CodeMirror/addon/search/match-highlighter"",
            ""Libs/CodeMirror/addon/search/search"",
            ""Libs/CodeMirror/addon/search/searchcursor"",
        ], function (ChartScript, CodeMirror) {
            ChartScript.init($(""#");

            
            #line 62 "..\..\Chart\Views\ChartScriptCode.cshtml"
                             Write(Model.Compose("Script"));

            
            #line default
            #line hidden
WriteLiteral("\"), CodeMirror);\r\n        }); \r\n    </script>\r\n");

        }
    }
}
#pragma warning restore 1591
