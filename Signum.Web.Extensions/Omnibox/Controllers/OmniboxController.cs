﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Signum.Entities.Omnibox;
using Signum.Utilities;

namespace Signum.Web.Omnibox
{
    public class OmniboxController : Controller
    {
        [HttpPost]
        public JsonResult Autocomplete(string text)
        {
            var result = OmniboxParser.Results(text, new System.Threading.CancellationToken())
                .Select(or => new
                {
                    label = OmniboxClient.RenderHtml.Invoke(or).ToString(),
                    cleanText = or.ToString()
                });

            return Json(result);
        }
    }
}