﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Windows.Reports;
using Signum.Entities.Reports;
using Signum.Services;

namespace Signum.Windows.Reports
{
    public class ReportClient
    {
        public static Dictionary<string, object> QueryNames; 

        public static void Start(bool toExcel, bool excelReport, bool compositeReport)
        {
            if (toExcel)
            {
                SearchControl.GetCustomMenuItems += qn => new ExcelReportMenuItem();
                Navigator.Manager.Settings.Add(typeof(ExcelReportDN), new EntitySettings(false) { View = () => new ExcelReport() });
            }
            if (excelReport)
            {
                QueryNames = Server.Service<IQueryServer>().GetQueryNames().ToDictionary(a => a.ToString()); 

                SearchControl.GetCustomMenuItems += qn => qn == typeof(ExcelReportDN) ? null : new ExcelReportPivotTableMenuItem();
                
                if (compositeReport)
                {
                    Navigator.Manager.Settings.Add(typeof(CompositeReportDN), new EntitySettings(false) { View = () => new CompositeReport() });
                }
            }
            else
                if (compositeReport)
                    throw new InvalidOperationException(); 
        }
    }
}
