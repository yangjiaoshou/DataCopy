using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCopy.DAL
{
    class BCB_CHECKFLOWDAL
    {
        HKM_SYSEntities db = new HKM_SYSEntities();

        public int UpdateIfNotCompleted(System.Linq.Expressions.Expression<Func<BCB_CHECKFLOW, bool>> func)
        {
           BCB_CHECKFLOW bCB_CHECKFLOW = db.BCB_CHECKFLOW.Where(func).FirstOrDefault();
           if (bCB_CHECKFLOW.COMPLETETAG != "1")
           {
               bCB_CHECKFLOW.COMPLETETAG = "1";
           }
          
            return db.SaveChanges();
        }

        public List<BCB_CHECKFLOW> getListByMainGUID(string mainGUID)
        {
            return db.BCB_CHECKFLOW.Where(u => u.MAINGUID == mainGUID).ToList();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

        public BCB_CHECKFLOW GetBy(System.Linq.Expressions.Expression<Func<BCB_CHECKFLOW, bool>> func)
        {
            BCB_CHECKFLOW bCB_CHECKFLOW = db.BCB_CHECKFLOW.Where(func).FirstOrDefault();
            return bCB_CHECKFLOW;
        }

        public  int UpdateByMainGUIDANDGroupGUID(string mainGUID, List<string> GroupGUID)
        {
            List<BCB_CHECKFLOW> listBCB_CHECKFLOW = db.BCB_CHECKFLOW.Where(u=>u.MAINGUID==mainGUID&&GroupGUID.Contains(u.GROUPGUID)).ToList();
            listBCB_CHECKFLOW.ForEach(u => {
                u.COMPLETETAG = "1";
            });
            return db.SaveChanges();
        }

        public List<BCB_CHECKFLOW> GetListBy(System.Linq.Expressions.Expression<Func<BCB_CHECKFLOW, bool>> func)
        {
           return db.BCB_CHECKFLOW.Where(func).ToList();
        }
    }
}
