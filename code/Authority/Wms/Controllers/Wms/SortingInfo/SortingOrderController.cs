﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Common.WebUtil;
using THOK.WMS.DownloadWms.Bll;
using THOK.Wms.DownloadWms.Bll;
using THOK.Authority.Bll.Interfaces;
using THOK.Security;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;

namespace Authority.Controllers.Wms.SortingInfo
{
    [TokenAclAuthorize]
    public class SortingOrderController : Controller
    {
        [Dependency]
        public ISortOrderService SortOrderService { get; set; }
        [Dependency]
        public ISortOrderDetailService SortOrderDetailService { get; set; }
        [Dependency]
        public IDeliverLineService DeliverLineService { get; set; }
        [Dependency]
        public ICustomerService CustomerService { get; set; }
        [Dependency]
        public ISystemParameterService SystemParameterService { get; set; }
        //
        // GET: /SortingOrder/
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasDownload = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        //查询主单
        // GET: /SortingOrder/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string OrderID = collection["OrderID"] ?? "";
            string orderDate = collection["orderDate"] ?? "";
            string productCode = collection["productCode"] ?? "";
            var sortOrder = SortOrderService.GetDetails(page, rows, OrderID, orderDate,productCode);
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //查询细单
        // GET: /SortingOrder/sortOrderDetails/
        public ActionResult sortOrderDetails(int page, int rows, string OrderID)
        {
            var SortOrderDetail = SortOrderDetailService.GetDetails(page, rows, OrderID);
            return Json(SortOrderDetail, "text", JsonRequestBehavior.AllowGet);
        }

        //根据时间分组查询主单
        // GET: /SortingOrder/GetOrderMaster/
        public ActionResult GetOrderMaster(string orderDate)
        {
            var sortOrder = SortOrderService.GetDetails(orderDate);
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SortingOrder/DownSortOrder/
        public ActionResult DownSortOrder(string beginDate, string endDate, string sortLineCode, bool isSortDown, string batch)
        {
            string errorInfo = string.Empty;
            string lineErrorInfo = string.Empty;
            string custErrorInfo = string.Empty;
            bool bResult = false;
            bool lineResult = false;

            beginDate = Convert.ToDateTime(beginDate).ToString("yyyyMMdd");
            endDate = Convert.ToDateTime(endDate).ToString("yyyyMMdd");

            DownSortingInfoBll sortBll = new DownSortingInfoBll();
            DownRouteBll routeBll = new DownRouteBll();
            DownSortingOrderBll orderBll = new DownSortingOrderBll();
            DownCustomerBll custBll = new DownCustomerBll();
            DownDistStationBll stationBll = new DownDistStationBll();
            DownDistCarBillBll carBll = new DownDistCarBillBll();
            DownUnitBll ubll = new DownUnitBll();
            DownProductBll pbll = new DownProductBll();

            try
            {
                ubll.DownUnitCodeInfo();
                pbll.DownProductInfo();
                routeBll.DeleteTable();
                stationBll.DownDistStationInfo();
                if (!SystemParameterService.SetSystemParameter())
                {
                    bool custResult = custBll.DownCustomerInfo();
                    carBll.DownDistCarBillInfo(beginDate);
                    if (isSortDown)
                    {
                        //从分拣下载分拣数据
                        lineResult = routeBll.DownSortRouteInfo();
                        bResult = sortBll.GetSortingOrderDate(beginDate, endDate, sortLineCode, batch, out errorInfo);
                    }
                    else
                    {
                        //从营销下载分拣数据 
                        lineResult = routeBll.DownRouteInfo();
                        bResult = orderBll.GetSortingOrderDate(beginDate, endDate, out errorInfo);
                    }
                }
                else
                {
                    bool custResult = custBll.DownCustomerInfos();//创联
                    //carBll.DownDistCarBillInfo(beginDate);
                    if (isSortDown)
                    {
                        //从分拣下载分拣数据
                        lineResult = routeBll.DownSortRouteInfo();
                        bResult = sortBll.GetSortingOrderDate(beginDate, endDate, sortLineCode, batch, out errorInfo);
                    }
                    else
                    {
                        //从营销下载分拣数据 创联
                        lineResult = routeBll.DownRouteInfos();
                        bResult = orderBll.GetSortingOrderDates(beginDate, endDate, out errorInfo);
                    }
                }
            }
            catch (Exception e)
            {
                errorInfo += e.Message;
            }            

            string info = "线路：" + lineErrorInfo + "。客户：" + custErrorInfo + "。分拣" + errorInfo;
            string msg = bResult ? "下载成功" : "下载失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, errorInfo), "text", JsonRequestBehavior.AllowGet);
        }

        #region /SortingOrder/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string orderId = Request.QueryString["orderId"];

            ExportParam ep = new ExportParam();
            ep.DT1 = SortOrderDetailService.GetSortOrderDetail(page, rows, orderId);
            ep.HeadTitle1 = "分拣订单管理";
            return PrintService.Print(ep);
        }
        #endregion
    }
}
