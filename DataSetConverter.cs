using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataCopy.ViewModels;

namespace DataCopy
{
   public class DataSetConverter
    {
        public static List<VW_DIME_RESULT> ConvertToVM_DIME_RESULT(DataSet ds)
        {
            List<VW_DIME_RESULT> listVM_DIME_RESULT = new List<VW_DIME_RESULT>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    VW_DIME_RESULT vM_DIME_RESULT = new VW_DIME_RESULT();
                    vM_DIME_RESULT.CHECKCODE = item["CHECKCODE"].ToString();
                    vM_DIME_RESULT.ITEMEID = item["ITEMEID"].ToString();
                    vM_DIME_RESULT.ITEMNAME = item["ITEMNAME"].ToString();
                    vM_DIME_RESULT.RESULT = item["RESULT"].ToString();
                    vM_DIME_RESULT.UNIT = item["UNIT"].ToString();
                    vM_DIME_RESULT.VALSTANDARD = item["VALSTANDARD"].ToString();
                    vM_DIME_RESULT.DOCTORNAME = item["DOCTORNAME"].ToString();
                    vM_DIME_RESULT.DATECHECK =DateTime.Parse(item["DATECHECK"].ToString());
                    vM_DIME_RESULT.RECHECKDATE = DateTime.Parse(item["RECHECKDATE"].ToString());
                    vM_DIME_RESULT.RECHECKDOCTORNAME = item["RECHECKDOCTORNAME"].ToString();
                    vM_DIME_RESULT.RESULTTAG = item["RESULTTAG"].ToString();
                    vM_DIME_RESULT.cp_uncheckflag = (int)item["cp_uncheckflag"];
                    listVM_DIME_RESULT.Add(vM_DIME_RESULT);
                }
            }
            return listVM_DIME_RESULT;
        }


        public static List<VW_DIME_RESULT> ConvertToVM_DIME_RESULT(DataSet ds,string interfaceGUID,string typeGUID)
        {
            List<VW_DIME_RESULT> listVM_DIME_RESULT = new List<VW_DIME_RESULT>();
            if (ds.Tables[0].Rows.Count > 0)
            {
              List<ItemContrastInfo> list = InterfaceManager.GetItemContrastColletion(interfaceGUID, typeGUID);
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    VW_DIME_RESULT vM_DIME_RESULT = new VW_DIME_RESULT();
                    vM_DIME_RESULT.CHECKCODE = item["CHECKCODE"].ToString();
                    ItemContrastInfo itemInfo = list.FirstOrDefault(u => u.IDExternal == item["ITEMEID"].ToString());
                    vM_DIME_RESULT.ITEMEID = itemInfo == null ? string.Empty : itemInfo.IDInternal;
                    vM_DIME_RESULT.ITEMNAME = itemInfo==null?string.Empty:itemInfo.NameInternal;
                    vM_DIME_RESULT.RESULT = item["RESULT"].ToString();
                    vM_DIME_RESULT.UNIT = item["UNIT"].ToString();
                    vM_DIME_RESULT.VALSTANDARD = item["VALSTANDARD"].ToString();
                    vM_DIME_RESULT.DOCTORNAME = item["DOCTORNAME"].ToString();
                    vM_DIME_RESULT.DATECHECK = DateTime.Parse(item["DATECHECK"].ToString());
                    vM_DIME_RESULT.RECHECKDATE = DateTime.Parse(item["RECHECKDATE"].ToString());
                    vM_DIME_RESULT.RECHECKDOCTORNAME = item["RECHECKDOCTORNAME"].ToString();
                    vM_DIME_RESULT.RESULTTAG = item["RESULTTAG"].ToString();
                    vM_DIME_RESULT.cp_uncheckflag = (int)item["cp_uncheckflag"];
                    listVM_DIME_RESULT.Add(vM_DIME_RESULT);
                }
            }
            return listVM_DIME_RESULT;
        }
    }
}
