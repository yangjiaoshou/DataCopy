using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DataCopy.DAL
{
   public class BCB_GROUPCONCLUSIONDAL
    {
       HKM_SYSEntities db = new HKM_SYSEntities();

       public bool Add(BCB_GROUPCONCLUSION bCB_GROUPCONCLUSION)
       {
           db.BCB_GROUPCONCLUSION.AddObject(bCB_GROUPCONCLUSION);
           return  db.SaveChanges()>0?true:false;
       }

       public BCB_GROUPCONCLUSION GetBy(Expression<Func<BCB_GROUPCONCLUSION, bool>> func)
       {
           return db.BCB_GROUPCONCLUSION.Where(func).FirstOrDefault();
       }

       public bool SaveChanges()
       {
         return db.SaveChanges()>0?true:false;
       }

       public List<BCB_GROUPCONCLUSION> GetListBy(Expression<Func<BCB_GROUPCONCLUSION, bool>> func)
       {
           return db.BCB_GROUPCONCLUSION.Where(func).ToList();
       }
    }
}
