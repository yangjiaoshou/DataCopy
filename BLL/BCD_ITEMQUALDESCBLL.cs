using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.DAL;
using System.Linq.Expressions;

namespace DataCopy.BLL
{
    class BCD_ITEMQUALDESCBLL
    {
        BCD_ITEMQUALDESCDAL dal = new BCD_ITEMQUALDESCDAL();

        public BCD_ITEMQUALDESC GetBy(Expression<Func<BCD_ITEMQUALDESC, bool>> func)
        {
            return dal.GetBy(func);
        }

        public List<BCD_ITEMQUALDESC> GetListBy(Expression<Func<BCD_ITEMQUALDESC, bool>> func)
        {
            return dal.GetListBy(func);
        }

        public List<BCD_ITEMQUALDESC> GetAllList()
        {
            return dal.GetAllList();
        }

    }
}
