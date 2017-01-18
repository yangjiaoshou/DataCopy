using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DataCopy.DAL
{
    class BCD_ITEMDAL
    {
        HKM_SYSEntities db = new HKM_SYSEntities();

        public BCD_ITEM GetBy(Expression<Func<BCD_ITEM,bool>> func)
        {
            return db.BCD_ITEM.Where(func).FirstOrDefault();
        }

        public List<BCD_ITEM> GetALL()
        {
            return db.BCD_ITEM.ToList();
        }
    }
}
