﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class InBillDetailService:ServiceBase<InBillDetail>,IInBillDetailService
    {
        [Dependency]
        public IInBillDetailRepository InBillDetailRepository { get; set; }
        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IInBillDetailService 成员

        public object GetDetails(int page, int rows, string BillNo)
        {
            if (BillNo != "" && BillNo != null)
            {
                IQueryable<InBillDetail> inBillDetailQuery = InBillDetailRepository.GetQueryable();
                var inBillDetail = inBillDetailQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo).AsEnumerable().Select(i => new
                {
                    i.ID,
                    i.BillNo,
                    i.ProductCode,
                    i.Product.ProductName,
                    i.UnitCode,
                    i.Unit.UnitName,
                    i.BillQuantity,
                    i.RealQuantity,
                    i.AllotQuantity,
                    i.Price,
                    i.Description
                });
                int total = inBillDetail.Count();
                inBillDetail = inBillDetail.Skip((page - 1) * rows).Take(rows);
                return new { total, rows = inBillDetail.ToArray() };
            }
            return "";
        }

        public new bool Add(InBillDetail inBillDetail)
        {
            IQueryable<InBillDetail> inBillDetailQuery = InBillDetailRepository.GetQueryable();
            var isExistProduct = inBillDetailQuery.FirstOrDefault(i=>i.BillNo==inBillDetail.BillNo&&i.ProductCode==inBillDetail.ProductCode);
            if (isExistProduct == null)
            {
                var ibd = new InBillDetail();
                ibd.BillNo = inBillDetail.BillNo;
                ibd.ProductCode = inBillDetail.ProductCode;
                ibd.UnitCode = inBillDetail.UnitCode;
                ibd.Price = inBillDetail.Price;
                ibd.BillQuantity = inBillDetail.BillQuantity;
                ibd.AllotQuantity = 0;
                ibd.RealQuantity = 0;
                ibd.Description = inBillDetail.Description;

                InBillDetailRepository.Add(ibd);
                InBillDetailRepository.SaveChanges();
            }
            else
            {
                var ibd = inBillDetailQuery.FirstOrDefault(i => i.BillNo == inBillDetail.BillNo && i.ProductCode == inBillDetail.ProductCode);
                ibd.BillQuantity = ibd.BillQuantity + inBillDetail.BillQuantity;
                InBillDetailRepository.SaveChanges();
            }
            return true;
        }

        public bool Delete(string ID)
        {
            IQueryable<InBillDetail> inBillDetailQuery = InBillDetailRepository.GetQueryable();
            int intID = Convert.ToInt32(ID);
            var ibd = inBillDetailQuery.FirstOrDefault(i=>i.ID==intID);
            InBillDetailRepository.Delete(ibd);
            InBillDetailRepository.SaveChanges();
            return true;
        }

        public bool Save(InBillDetail inBillDetail)
        {
            IQueryable<InBillDetail> inBillDetailQuery = InBillDetailRepository.GetQueryable();
            var ibd = inBillDetailQuery.FirstOrDefault(i=>i.ID==inBillDetail.ID&&i.BillNo==inBillDetail.BillNo);
            ibd.ProductCode = inBillDetail.ProductCode;
            ibd.UnitCode = inBillDetail.UnitCode;
            ibd.Price = inBillDetail.Price;
            ibd.BillQuantity = inBillDetail.BillQuantity;
            ibd.Description = inBillDetail.Description;
            InBillDetailRepository.SaveChanges();
            return true;
        }

        #endregion

        #region IInBillDetailService 成员


        public object GetProductDetails(int page, int rows, string QueryString, string Value)
        {
            string ProductName = "";
            string ProductCode = "";
            if (QueryString == "ProductCode")
            {
                ProductCode = Value;
            }
            else
            {
                ProductName = Value;
            }
            IQueryable<Product> ProductQuery = ProductRepository.GetQueryable();
            var product = ProductQuery.Where(c => c.ProductName.Contains(ProductName) && c.ProductCode.Contains(ProductCode)&& c.IsActive=="1")
                .OrderBy(c => c.ProductCode).AsEnumerable()
                .Select(c => new
                {
                    c.AbcTypeCode,
                    c.BarBarcode,
                    c.BelongRegion,
                    c.BrandCode,
                    c.BuyPrice,
                    c.CostPrice,
                    c.CustomCode,
                    c.Description,
                    IsAbnormity = c.IsAbnormity == "1" ? "是" : "不是",
                    IsActive = c.IsActive == "1" ? "可用" : "不可用",
                    c.IsConfiscate,
                    c.IsFamous,
                    c.IsFilterTip,
                    c.IsMainProduct,
                    c.IsNew,
                    c.IsProvinceMainProduct,
                    c.OneProjectBarcode,
                    c.PackageBarcode,
                    c.PackTypeCode,
                    c.PieceBarcode,
                    c.PriceLevelCode,
                    c.ProductCode,
                    c.ProductName,
                    c.ProductTypeCode,
                    c.RetailPrice,
                    c.ShortCode,
                    c.StatisticType,
                    c.SupplierCode,
                    c.TradePrice,
                    c.UniformCode,
                    c.UnitCode,
                    c.Unit.UnitName,
                    c.UnitListCode,
                    UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            int total = product.Count();
            product = product.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = product.ToArray() };
        }

        #endregion
    }
}
