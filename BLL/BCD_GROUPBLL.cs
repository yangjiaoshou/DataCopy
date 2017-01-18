using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.DAL;
using System.Linq.Expressions;

namespace DataCopy.BLL
{
    class BCD_GROUPBLL
    {
        BCD_GROUPDAL dal = new BCD_GROUPDAL();

        public BCD_GROUP GetBy(Expression<Func<BCD_GROUP, bool>> func)
        {
            return dal.GetBy(func);
        }

        public List<BCD_GROUP> GetListBy(Expression<Func<BCD_GROUP, bool>> func)
        {
            return dal.GetListBy(func);
        }
    }
}
