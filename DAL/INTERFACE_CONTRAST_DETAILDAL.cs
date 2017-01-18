using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.ViewModels;

namespace DataCopy.DAL
{
    public class INTERFACE_CONTRAST_DETAILDAL
    {
        HKM_SYSEntities db = new HKM_SYSEntities();

        public List<ItemContrastInfo> GetItemContrastCollection(string interfaceGUID, string typeGUID)
        {
            var query = from info in db.INTERFACE_INFO
                        join type in db.INTERFACE_CONTRAST_TYPE on info.GUID equals type.INTERFACE_GUID
                        join field in db.INTERFACE_CONTRAST_FIELD on type.GUID equals field.INTERFACE_TYPE_GUID
                        join detail in db.INTERFACE_CONTRAST_DETAIL on field.GUID equals detail.CONTRAST_FIELD_GUID
                        where info.GUID==interfaceGUID&&type.GUID==typeGUID 
                        select new ItemContrastInfo
                        {
                            IDExternal = detail.VALUE_EXTERNAL,
                            NameExternal = detail.NAME_EXTERNAL,
                            IDInternal = detail.FIELD_ID_LOCAL,
                            NameInternal = detail.FIELD_NAME_LOCAL
                        };
            return query.ToList();
        }
    }
}
