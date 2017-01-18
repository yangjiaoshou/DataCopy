using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.DAL;
using System.Linq.Expressions;

namespace DataCopy.BLL
{
    class BCD_ITEMBLL
    {
        BCD_ITEMDAL dal = new BCD_ITEMDAL();

        public BCD_ITEM GetBy(Expression<Func<BCD_ITEM, bool>> func)
        {
            return dal.GetBy(func);
        }

        public List<BCD_ITEM> GetALL()
        {
            return dal.GetALL();
        }
    }
}
