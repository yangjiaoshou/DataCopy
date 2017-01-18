using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.DAL;
using DataCopy.ViewModels;

namespace DataCopy.BLL
{
   public class INTERFACE_CONTRAST_DETAILBLL
    {
       INTERFACE_CONTRAST_DETAILDAL dal = new INTERFACE_CONTRAST_DETAILDAL();

       public List<ItemContrastInfo> GetItemContrastCollection(string interfaceGUID, string typeGUID)
       {
           return dal.GetItemContrastCollection(interfaceGUID,typeGUID);
       }
    }
}
