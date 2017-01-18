using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DataCopy.DAL
{
    class BCD_ITEMQUALDESCDAL
    {
        HKM_SYSEntities db = new HKM_SYSEntities();

        public BCD_ITEMQUALDESC GetBy(Expression<Func<BCD_ITEMQUALDESC, bool>> func)
        {
            return db.BCD_ITEMQUALDESC.Where(func).FirstOrDefault();
        }

        public List<BCD_ITEMQUALDESC> GetListBy(Expression<Func<BCD_ITEMQUALDESC, bool>> func)
        {
            return db.BCD_ITEMQUALDESC.Where(func).ToList();
        }

        public List<BCD_ITEMQUALDESC> GetAllList()
        {
            return db.BCD_ITEMQUALDESC.ToList();
        }
    }
}
