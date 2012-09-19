﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using System.Web.Routing;
using THOK.WebUtil;

namespace Wms.Controllers.Wms.ProductQuality
{
    public class ProductTimeOutController : Controller
    {
        [Dependency]
        public IProductWarningService ProductWarningService { get; set; }
        //
        // GET: /ProductTimeOut/
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        [HttpPost]
        public ActionResult GetProductDetails(int page, int rows, FormCollection collection)
        {
            string productCode = collection["ProductCode"] ?? "";
            decimal assemblyTime=360;
            if (collection["AssemblyTime"] != null && collection["AssemblyTime"] != "")
            {
               assemblyTime= decimal.Parse(collection["AssemblyTime"]);
            }
            var product = ProductWarningService.GetProductDetails(page, rows, productCode, assemblyTime);
            return Json(product, "text", JsonRequestBehavior.AllowGet);
        }

        #region /ProductTimeOut/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string productCode = Request.QueryString["productCode"]??"";
            decimal assemblyTime = 360;
            if (Request.QueryString["assemblyTime"] != null && Request.QueryString["assemblyTime"] != "")
            {
                assemblyTime = decimal.Parse(Request.QueryString["assemblyTime"]);
            }
            System.Data.DataTable dt = ProductWarningService.GetProductTimeOut(page, rows, productCode, assemblyTime);
            string headText = "产品预警信息设置";
            string headFontName = "微软雅黑"; Int16 headFontSize = 20;
            string colHeadFontName = "Arial"; Int16 colHeadFontSize = 10;

            System.IO.MemoryStream ms = THOK.Common.ExportExcel.ExportDT(dt, null, headText, null, headFontName, headFontSize, colHeadFontName, colHeadFontSize);
            return new FileStreamResult(ms, "application/ms-excel");
        }
        #endregion
    }
}
