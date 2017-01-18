using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataCopy.DAL;

namespace DataCopy.BLL
{
    class BCB_CHECKFLOWBLL
    {
        BCB_CHECKFLOWDAL dal = new BCB_CHECKFLOWDAL();
        public int UpdateIfNotCompleted(Expression<Func<BCB_CHECKFLOW, bool>> func)
        {
            return dal.UpdateIfNotCompleted(func);
        }

        public int SaveChanges()
        {
            return dal.SaveChanges();
        }

        public void ChangeCompleteTagToOne(Expression<Func<BCB_CHECKFLOW, bool>> func)
        {
            BCB_CHECKFLOW bCB_CHECKFLOW = dal.GetBy(func);
            if (bCB_CHECKFLOW.COMPLETETAG != "1")
            {
                bCB_CHECKFLOW.COMPLETETAG = "1";
            }
        }

        public BCB_CHECKFLOW GetBy(Expression<Func<BCB_CHECKFLOW, bool>> func)
        {
            return dal.GetBy(func);
        }

        public int UpdateByMainGUIDANDGroupGUID(string mainGUID, List<string> GroupGUID)
        {
            return dal.UpdateByMainGUIDANDGroupGUID(mainGUID, GroupGUID);
        }

        public List<BCB_CHECKFLOW> GetListBy(Expression<Func<BCB_CHECKFLOW, bool>> func)
        {
            return dal.GetListBy(func);
        }
    }
}
