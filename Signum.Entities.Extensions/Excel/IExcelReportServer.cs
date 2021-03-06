﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Signum.Entities;
using Signum.Entities.DynamicQuery;
using Signum.Entities.Excel;

namespace Signum.Services
{
    [ServiceContract]
    public interface IExcelReportServer
    {
        [OperationContract, NetDataContract]
        List<Lite<ExcelReportEntity>> GetExcelReports(object queryName);

        [OperationContract, NetDataContract]
        byte[] ExecuteExcelReport(Lite<ExcelReportEntity> excelReport, QueryRequest request);

        [OperationContract, NetDataContract]
        byte[] ExecutePlainExcel(QueryRequest request);
    }
}
