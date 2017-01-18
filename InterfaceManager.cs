using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.BLL;
using DataCopy.ViewModels;

namespace DataCopy
{
    public class InterfaceManager
    {
        private static List<ItemContrastInfo> itemContrastCollection;

        public static List<ItemContrastInfo> GetItemContrastColletion(string interfaceGUID, string typeGUID)
        {
            if (itemContrastCollection==null)
            {
                itemContrastCollection = new INTERFACE_CONTRAST_DETAILBLL().GetItemContrastCollection(interfaceGUID, typeGUID);
            }
            return itemContrastCollection;
        }
    }
}
