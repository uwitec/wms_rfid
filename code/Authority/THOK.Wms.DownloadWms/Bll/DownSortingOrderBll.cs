using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;
using THOK.WMS.DownloadWms.Dao;
using System.Threading;

namespace THOK.WMS.DownloadWms.Bll
{
    public class DownSortingOrderBll
   {

       #region �ֶ���Ӫ��ϵͳ���طּ���Ϣ

       /// <summary>
       /// �ֶ�����
       /// </summary>
       /// <param name="billno"></param>
       /// <returns></returns>
       public string GetSortingOrderById(string orderid)
       {
           string tag = "true";
           using (PersistentManager dbPm = new PersistentManager())
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               try
               {
                   orderid = "ORDER_ID IN (" + orderid + ")";
                   DataTable masterdt = this.GetSortingOrder(orderid);
                   DataTable detaildt = this.GetSortingOrderDetail(orderid);
                   if (masterdt.Rows.Count > 0 && detaildt.Rows.Count>0)
                   {
                       DataSet masterds = this.SaveSortingOrder(masterdt);
                       DataSet detailds = this.SaveSortingOrderDetail(detaildt);
                       this.Insert(masterds, detailds);
                   }
                   else
                       return "û�����ݿ����أ�";
               }
               catch (Exception e)
               {
                   tag = e.Message;
               }
           }
           return tag;
       }
       #endregion

       #region �Զ���Ӫ��ϵͳ���طּ���Ϣ

       /// <summary>
       /// �Զ����ض���
       /// </summary>
       /// <returns></returns>
       public string DownSortingOrder()
       {
           string tag = "true";
           using (PersistentManager dbpm = new PersistentManager())
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               try
               {
                   DataTable orderdt = this.GetOrderId();
                   string orderlist = UtinString.StringMake(orderdt, "ORDER_ID");
                   orderlist = UtinString.StringMake(orderlist);
                   orderlist = "ORDER_ID NOT IN(" + orderlist + ")";

                   DataTable masterdt = this.GetSortingOrder(orderlist);
                   DataTable detaildt = this.GetSortingOrderDetail(orderlist);
                   if (masterdt.Rows.Count > 0 && detaildt.Rows.Count > 0)
                   {
                       DataSet masterds = this.SaveSortingOrder(masterdt);
                       DataSet detailds = this.SaveSortingOrderDetail(detaildt);
                       this.Insert(masterds, detailds);
                   }
                   else
                       return "û�п��õ��������أ�";
               }
               catch (Exception e)
               {
                   tag = e.Message;
               }
           }
           return tag;
       }

       #endregion

       #region ѡ�����ڴ�Ӫ��ϵͳ���طּ���Ϣ

       /// <summary>
       /// ѡ�����ڴ�Ӫ��ϵͳ���طּ���Ϣ
       /// </summary>
       /// <param name="startDate"></param>
       /// <param name="endDate"></param>
       /// <returns></returns>
       public bool GetSortingOrderDate(string startDate, string endDate, out string errorInfo)
       {
           bool tag = false;
           errorInfo=string.Empty;
           using (PersistentManager dbpm = new PersistentManager())
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               try
               {
                   //��ѯ�ֿ�7���ڵĶ�����
                   DataTable orderdt = this.GetOrderId();
                   string orderlist = UtinString.StringMake(orderdt, "order_id");
                   orderlist = UtinString.StringMake(orderlist);
                   string orderlistDate = "ORDER_DATE>='" + startDate + "' AND ORDER_DATE<='" + endDate + "' AND ORDER_ID NOT IN(" + orderlist + ")";
                   DataTable masterdt = this.GetSortingOrder(orderlistDate);//����ʱ���ѯ������Ϣ

                   string ordermasterlist = UtinString.StringMake(masterdt, "ORDER_ID");//ȡ�ø���ʱ���ѯ�Ķ�����
                   string ordermasterid = UtinString.StringMake(ordermasterlist);
                   ordermasterid = "ORDER_ID IN (" + ordermasterid + ")";
                   DataTable detaildt = this.GetSortingOrderDetail(ordermasterid);//���ݶ����Ų�ѯ��ϸ
                   if (masterdt.Rows.Count > 0 && detaildt.Rows.Count > 0)
                   {
                       DataSet masterds = this.SaveSortingOrder(masterdt);
                       DataSet detailds = this.SaveSortingOrderDetail(detaildt);
                       this.Insert(masterds, detailds);
                       tag = true;
                   }
                   else
                       errorInfo= "û�п��õ��������أ�";
               }
               catch (Exception e)
               {
                   errorInfo = "����" + e.Message;
               }
           }
           return tag;
       }

       #endregion

       #region ��ҳ��Ӫ��ϵͳ�ݲ�ѯ�ּ𶩵�����

       /// <summary>
       /// ��ѯӪ��ϵͳ�ּ𶩵��������ݽ�������
       /// </summary>
       /// <param name="pageIndex"></param>
       /// <param name="pageSize"></param>
       /// <returns></returns>
       public DataTable GetSortingOrder(int pageIndex, int pageSize)
       {
           using (PersistentManager dbpm = new PersistentManager("YXConnection"))
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               dao.SetPersistentManager(dbpm);
               DataTable orderdt = this.GetOrderId();
               string orderlist = UtinString.StringMake(orderdt, "ORDER_ID");
               orderlist = UtinString.StringMake(orderlist);
               orderlist = "ORDER_ID NOT IN(" + orderlist + ")";
               return dao.GetSortingOrder(orderlist);
           }
       }

       /// <summary>
       /// ����ʱ���ѯӪ��ϵͳ�ּ𶩵��������ݽ�������
       /// </summary>
       /// <param name="pageIndex"></param>
       /// <param name="pageSize"></param>
       /// <returns></returns>
       public DataTable GetSortingOrder(int pageIndex, int pageSize,string startDate,string endDate)
       {
           using (PersistentManager dbpm = new PersistentManager("YXConnection"))
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               dao.SetPersistentManager(dbpm);
               DataTable orderdt = this.GetOrderId();
               string orderlist = UtinString.StringMake(orderdt, "ORDER_ID");
               orderlist = UtinString.StringMake(orderlist);
               //orderlist = "ORDER_DATE>='" + startDate + "' AND ORDER_DATE<='" + endDate + "' AND ORDER_ID NOT IN(" + orderlist + ")";
               orderlist = "ORDER_ID NOT IN(" + orderlist + ")";
               DataTable masert = dao.GetSortingOrder("ORDER_ID NOT IN('')");
               DataRow[] orderdr = masert.Select(orderlist);
               return this.SaveSortingOrder(orderdr).Tables[0];
           }
       }

       /// <summary>
       /// ��ѯӪ��ϵͳ�ּ���ϸ������
       /// </summary>
       /// <param name="pageIndex"></param>
       /// <param name="pageSize"></param>
       /// <param name="inBillNo"></param>
       /// <returns></returns>
       public DataTable GetSortingOrderDetail(int pageIndex, int pageSize, string inBillNo)
       {
           using (PersistentManager dbpm = new PersistentManager("YXConnection"))
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               dao.SetPersistentManager(dbpm);
               inBillNo = "ORDER_ID = '" + inBillNo + "'";
               return dao.GetSortingOrderDetail(inBillNo);
           }
       }

       #endregion

       #region ��ѯ�ִ��ּ���Ϣ

       /// <summary>
       /// ��ѯ4��֮�ڵķּ𶩵�
       /// </summary>
       /// <returns></returns>
       public DataTable GetOrderId()
       {
           using (PersistentManager dbpm = new PersistentManager())
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               return dao.GetOrderId();
           }
       }

       /// <summary>
       /// ��ȡ�������ı���
       /// </summary>
       /// <returns></returns>
       public string GetCompany()
       {
           using (PersistentManager dbpm = new PersistentManager())
           {
               DownDistDao dao = new DownDistDao();
               return dao.GetCompany().ToString();
           }
       }
       #endregion

       #region ����������Ϣ

       /// <summary>
       /// �����ص��������ӵ����ݿ⡣
       /// </summary>
       /// <param name="masterds"></param>
       /// <param name="detailds"></param>
       public void Insert(DataSet masterds, DataSet detailds)
       {
           using (PersistentManager pm = new PersistentManager())
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               if (masterds.Tables["DWV_OUT_ORDER"].Rows.Count > 0)
               {
                   dao.InsertSortingOrder(masterds);
               }
               if (detailds.Tables["DWV_OUT_ORDER_DETAIL"].Rows.Count > 0)
               {
                   dao.InsertSortingOrderDetail(detailds);
               }
           }
       }

       /// <summary>
       /// ���涩��������Ϣ�����������������DATATABLE
       /// </summary>
       /// <param name="dr"></param>
       /// <returns></returns>
       public DataSet SaveSortingOrder(DataTable masterdt)
       {
           DataSet ds = this.GenerateEmptyTables();
           foreach (DataRow row in masterdt.Rows)
           {
               DataRow masterrow = ds.Tables["DWV_OUT_ORDER"].NewRow();
               masterrow["order_id"] = row["ORDER_ID"].ToString().Trim();//�������
               masterrow["company_code"] = row["ORG_CODE"].ToString().Trim();//������λ���
               masterrow["sale_region_code"] = row["SALE_REG_CODE"].ToString().Trim();//Ӫ�������
               masterrow["order_date"] = row["ORDER_DATE"].ToString().Trim();//��������
               masterrow["order_type"] = row["ORDER_TYPE"].ToString().Trim();//��������
               masterrow["customer_code"] = row["CUST_CODE"].ToString().Trim();//�ͻ����
               masterrow["customer_name"] = row["CUST_NAME"].ToString().Trim();//�ͻ�����
               masterrow["quantity_sum"] = Convert.ToDecimal(row["QUANTITY_SUM"].ToString());//������
               masterrow["amount_sum"] = Convert.ToDecimal(row["AMOUNT_SUM"].ToString());//�ܽ��
               masterrow["detail_num"] = Convert.ToInt32(row["DETAIL_NUM"].ToString());//��ϸ��
               masterrow["deliver_order"] = row["DELIVER_ORDER"].ToString().Trim();//�䳵����
               masterrow["DeliverDate"] = row["ORDER_TYPE"].ToString().Trim();//�ͻ��������
               masterrow["description"] = "";//�ͻ���������
               masterrow["is_active"] = row["ISACTIVE"].ToString().Trim();//�ͻ���·����
               masterrow["update_time"] = DateTime.Now;//�ͻ���·����
               masterrow["deliver_line_code"] = row["DELIVER_LINE_CODE"].ToString().Trim() + "_" + row["DIST_BILL_ID"].ToString().Trim();//�ͻ�˳�����
               ds.Tables["DWV_OUT_ORDER"].Rows.Add(masterrow);
           }
           return ds;
       }


       /// <summary>
       /// ���涩��������Ϣ�����������������DATAROW
       /// </summary>
       /// <param name="dr"></param>
       /// <returns></returns>
       public DataSet SaveSortingOrder(DataRow[] masterdr)
       {
           DataSet ds = this.GenerateEmptyTables();
           foreach (DataRow row in masterdr)
           {
               DataRow masterrow = ds.Tables["DWV_OUT_ORDER"].NewRow();
               masterrow["ORDER_ID"] = row["ORDER_ID"].ToString().Trim();//�������
               masterrow["ORG_CODE"] = row["ORG_CODE"].ToString().Trim();//������λ���
               masterrow["SALE_REG_CODE"] = row["SALE_REG_CODE"].ToString().Trim();//Ӫ�������
               masterrow["ORDER_DATE"] = row["ORDER_DATE"].ToString().Trim();//��������
               masterrow["ORDER_TYPE"] = row["ORDER_TYPE"].ToString().Trim();//��������
               masterrow["CUST_CODE"] = row["CUST_CODE"].ToString().Trim();//�ͻ����
               masterrow["CUST_NAME"] = row["CUST_NAME"].ToString().Trim();//�ͻ�����
               masterrow["QUANTITY_SUM"] = Convert.ToDecimal(row["QUANTITY_SUM"].ToString());//������
               masterrow["AMOUNT_SUM"] = Convert.ToDecimal(row["AMOUNT_SUM"].ToString());//�ܽ��
               masterrow["DETAIL_NUM"] = Convert.ToInt32(row["DETAIL_NUM"].ToString());//��ϸ��
               masterrow["DIST_BILL_ID"] = row["DIST_BILL_ID"].ToString().Trim();//�䳵����
               masterrow["DIST_STA_CODE"] =  row["DIST_STA_CODE"].ToString().Trim();//�ͻ��������
               masterrow["DIST_STA_NAME"] =  row["DIST_STA_NAME"].ToString().Trim();//�ͻ���������
               masterrow["DELIVER_LINE_CODE"] = row["DELIVER_LINE_CODE"].ToString().Trim();//�ͻ���·����
               masterrow["DELIVER_LINE_NAME"] =  row["DELIVER_LINE_NAME"].ToString().Trim();//�ͻ���·����
               masterrow["DELIVER_ORDER"] = row["DELIVER_ORDER"];//�ͻ�˳�����
               masterrow["ISACTIVE"] = row["ISACTIVE"];//�Ƿ����
               masterrow["UPDATE_DATE"] = row["UPDATE_DATE"];//����ʱ��
               masterrow["SORTING_CODE"] = "";//�ּ����
               masterrow["IS_IMPORT"] = "0";
               masterrow["ISSORTING"] = "0";
               ds.Tables["DWV_OUT_ORDER"].Rows.Add(masterrow);
           }
           return ds;
       }

       /// <summary>
       /// ���涩����ϸ�������������DataTable
       /// </summary>
       /// <param name="dr"></param>
       /// <returns></returns>
       public DataSet SaveSortingOrderDetail(DataTable detaildt)
       {
           DownProductBll pbll=new DownProductBll();
           DownSortingOrderDao dao = new DownSortingOrderDao();
           DataTable unitList = dao.GetUnitProduct();
           DataSet ds = this.GenerateEmptyTables();
           try
           {
               int i=0;
               foreach (DataRow row in detaildt.Rows)
               {
                   DataRow[] list = unitList.Select(string.Format("unit_list_code='{0}'", row["BRAND_N"].ToString().Trim()));
                   //DataTable product = pbll.FindProductInfo(row["BRAND_CODE"].ToString());
                   DataRow detailrow = ds.Tables["DWV_OUT_ORDER_DETAIL"].NewRow();
                   i++;
                   detailrow["order_detail_id"] = row["ORDER_DETAIL_ID"].ToString().Trim() + i;
                   detailrow["order_id"] = row["ORDER_ID"].ToString().Trim();
                   detailrow["product_code"] = row["BRAND_N"].ToString().Trim();
                   detailrow["product_name"] = row["BRAND_NAME"].ToString().Trim();
                   detailrow["unit_code"] = list[0]["unit_code02"].ToString();// product.Rows[0]["unit_code02"].ToString();
                   detailrow["unit_name"] = row["BRAND_UNIT_NAME"].ToString().Trim(); ;
                   detailrow["demand_quantity"] = Convert.ToDecimal(row["QUANTITY"]);
                   detailrow["real_quantity"] = Convert.ToDecimal(row["QUANTITY"]);
                   detailrow["price"] = Convert.ToDecimal(row["PRICE"]);
                   detailrow["amount"] = Convert.ToDecimal(row["AMOUNT"]);
                   detailrow["unit_quantity"] = 50;
                   ds.Tables["DWV_OUT_ORDER_DETAIL"].Rows.Add(detailrow);
               }
               return ds;
           }
           catch (Exception e)
           {
               string s = e.Message;
               return null;
           }
       }

       /// <summary>
       /// ��������������ϸ�������
       /// </summary>
       /// <returns></returns>
       private DataSet GenerateEmptyTables()
       {
           DataSet ds = new DataSet();
           DataTable mastertable = ds.Tables.Add("DWV_OUT_ORDER");
           mastertable.Columns.Add("order_id");
           mastertable.Columns.Add("company_code");
           mastertable.Columns.Add("sale_region_code");
           mastertable.Columns.Add("order_date");
           mastertable.Columns.Add("order_type");
           mastertable.Columns.Add("customer_code");
           mastertable.Columns.Add("customer_name");
           mastertable.Columns.Add("quantity_sum");
           mastertable.Columns.Add("amount_sum");
           mastertable.Columns.Add("detail_num");
           mastertable.Columns.Add("deliver_order");
           mastertable.Columns.Add("DeliverDate");
           mastertable.Columns.Add("description");
           mastertable.Columns.Add("is_active");
           mastertable.Columns.Add("update_time");
           mastertable.Columns.Add("deliver_line_code");

           DataTable detailtable = ds.Tables.Add("DWV_OUT_ORDER_DETAIL");
           detailtable.Columns.Add("order_detail_id");
           detailtable.Columns.Add("order_id");
           detailtable.Columns.Add("product_code");
           detailtable.Columns.Add("product_name");
           detailtable.Columns.Add("unit_code");
           detailtable.Columns.Add("unit_name");
           detailtable.Columns.Add("demand_quantity");
           detailtable.Columns.Add("real_quantity");
           detailtable.Columns.Add("price");
           detailtable.Columns.Add("amount");
           detailtable.Columns.Add("unit_quantity");
           return ds;
       }
       #endregion

       #region ������ѯ���طּ����ݵķ���


       /// <summary>
       /// �����û�ѡ��Ķ������طּ��߶�������
       /// </summary>
       /// <returns></returns>
       public DataTable GetSortingOrder(string orderid)
       {
           using (PersistentManager dbpm = new PersistentManager("YXConnection"))
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               dao.SetPersistentManager(dbpm);
               return dao.GetSortingOrder(orderid);
           }
       }

       /// <summary>
       /// �����û�ѡ��Ķ������طּ��߶�����ϸ��
       /// </summary>
       /// <returns></returns>
       public DataTable GetSortingOrderDetail(string orderid)
       {
           using (PersistentManager dbpm = new PersistentManager("YXConnection"))
           {
               DownSortingOrderDao dao = new DownSortingOrderDao();
               dao.SetPersistentManager(dbpm);
               return dao.GetSortingOrderDetail(orderid);
           }
       }

       #endregion

   }
}