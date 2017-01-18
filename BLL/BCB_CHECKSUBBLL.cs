using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.BLL;
using System.Linq.Expressions;
using DataCopy.DAL;

namespace DataCopy.BLL
{

    public class BCB_CHECKSUBBLL
    {
        BCB_CHECKSUBDAL dal = new BCB_CHECKSUBDAL();

        public BCB_CHECKSUB GetBy(Expression<Func<BCB_CHECKSUB, bool>> func)
        {
            return dal.GetBy(func);
        }

        public List<BCB_CHECKSUB> GetListBy(Expression<Func<BCB_CHECKSUB, bool>> func)
        {
            return dal.GetListBy(func);
        }

        public int BatchUpdate(List<BCB_CHECKSUB> listBCB_CHECKSUB)
        {
            if (listBCB_CHECKSUB == null)
            {
                return 0;
            }
            return dal.BatchUpdate(listBCB_CHECKSUB);
        }

        public bool IsAllPassed(Expression<Func<BCB_CHECKSUB, bool>> func)
        {
            return dal.IsAllPassed(func);
        }

        public int SaveChanges()
        {
            return dal.SaveChanges();
        }
    }
}
