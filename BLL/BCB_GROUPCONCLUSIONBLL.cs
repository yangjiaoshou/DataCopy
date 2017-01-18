using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.DAL;
using System.Linq.Expressions;

namespace DataCopy.BLL
{
   public class BCB_GROUPCONCLUSIONBLL
    {
       BCB_GROUPCONCLUSIONDAL dal = new BCB_GROUPCONCLUSIONDAL();

       public bool Add(BCB_GROUPCONCLUSION bCB_GROUPCONCLUSION)
       {
          return dal.Add(bCB_GROUPCONCLUSION);
       }

       public BCB_GROUPCONCLUSION GetBy(Expression<Func<BCB_GROUPCONCLUSION, bool>> func)
       {
           return dal.GetBy(func);
       }

       public List<BCB_GROUPCONCLUSION> GetListBy(Expression<Func<BCB_GROUPCONCLUSION, bool>> func)
       {
           return dal.GetListBy(func);
       }

       public bool SaveChanges()
       {
           return dal.SaveChanges();
       }
    }
}
