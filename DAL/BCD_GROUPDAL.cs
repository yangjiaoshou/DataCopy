using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DataCopy.DAL
{
    class BCD_GROUPDAL
    {
        HKM_SYSEntities db = new HKM_SYSEntities();
        public BCD_GROUP GetBy(Expression<Func<BCD_GROUP,bool>> func)
        {
            return db.BCD_GROUP.Where(func).FirstOrDefault();
        }

        public List<BCD_GROUP> GetListBy(Expression<Func<BCD_GROUP, bool>> func)
        {
            return db.BCD_GROUP.Where(func).ToList();
        }
    }
}
