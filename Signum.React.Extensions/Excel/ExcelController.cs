﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Signum.Engine.Authorization;
using Signum.Entities;
using Signum.Entities.Authorization;
using Signum.Services;
using Signum.Utilities;
using Signum.React.Facades;
using Signum.React.Authorization;
using Signum.Engine.Cache;
using Signum.Engine;
using Signum.Entities.Cache;
using Signum.Utilities.ExpressionTrees;
using Signum.Entities.Processes;
using Signum.Engine.Processes;
using System.Threading;
using Signum.React.ApiControllers;
using Signum.Engine.Basics;
using System.Web;
using Signum.React.Files;
using System.IO;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Excel;
using Signum.Entities.DynamicQuery;
using Signum.Entities.Excel;
using Signum.Entities.Chart;
using Signum.Engine.Chart;
using System.Threading.Tasks;

namespace Signum.React.Excel
{
    public class ExcelController : ApiController
    {
        [Route("api/excel/plain"), HttpPost]
        public async Task<HttpResponseMessage> ToPlainExcel(QueryRequestTS request, CancellationToken token)
        {
            var queryRequest = request.ToQueryRequest();

            ResultTable queryResult = await DynamicQueryManager.Current.ExecuteQueryAsync(queryRequest, token);
            byte[] binaryFile = PlainExcelGenerator.WritePlainExcel(queryResult, QueryUtils.GetNiceName(queryRequest.QueryName));

            var fileName = request.queryKey + TimeZoneManager.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx";

            return FilesController.GetHttpReponseMessage(new MemoryStream(binaryFile), fileName);            
        }

        [Route("api/excel/plainChart"), HttpPost]
        public async Task<HttpResponseMessage> ToPlainExcel(ChartRequest request, CancellationToken token)
        {
            var resultTable = await ChartLogic.ExecuteChartAsync(request, token);

            byte[] binaryFile = PlainExcelGenerator.WritePlainExcel(resultTable, QueryUtils.GetNiceName(request.QueryName));

            var fileName = request.ChartScript.ToString() + " " + QueryUtils.GetKey(request.QueryName) + TimeZoneManager.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx";

            return FilesController.GetHttpReponseMessage(new MemoryStream(binaryFile), fileName);
        }


        [Route("api/excel/reportsFor/{queryKey}"), HttpGet]
        public IEnumerable<Lite<ExcelReportEntity>> GetExcelReports(string queryKey)
        {
            return ExcelLogic.GetExcelReports(QueryLogic.ToQueryName(queryKey));
        }

        [Route("api/excel/excelReport"), HttpPost]
        public async Task<HttpResponseMessage> GenerateExcelReport(ExcelReportRequest request, CancellationToken token)
        {
            byte[] file = await ExcelLogic.ExecuteExcelReport(request.excelReport, request.queryRequest.ToQueryRequest(), token);

            var fileName = request.excelReport.ToString() + "-" + TimeZoneManager.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx";

            return FilesController.GetHttpReponseMessage(new MemoryStream(file),  fileName);
        }

        public class ExcelReportRequest
        {
            public QueryRequestTS queryRequest;
            public Lite<ExcelReportEntity> excelReport;
        }
    }
}