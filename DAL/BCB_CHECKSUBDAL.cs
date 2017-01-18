using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DataCopy.DAL
{
    class BCB_CHECKSUBDAL
    {
        HKM_SYSEntities db = new HKM_SYSEntities();

        public BCB_CHECKSUB GetBy(Expression<Func<BCB_CHECKSUB, bool>> func)
        {
            return db.BCB_CHECKSUB.Where(func).FirstOrDefault();
        }

        public int BatchUpdate(List<BCB_CHECKSUB> listBCB_CHECKSUB)
        {
            return db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

        public List<BCB_CHECKSUB> GetListBy(Expression<Func<BCB_CHECKSUB, bool>> func)
        {
            return db.BCB_CHECKSUB.Where(func).ToList();
        }

        /// <summary>
        /// 是否合格
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool IsAllPassed(Expression<Func<BCB_CHECKSUB, bool>> func)
        {
            bool isAllPassed = true; ;
            List<BCB_CHECKSUB> listBCB_CHECKSUB = db.BCB_CHECKSUB.Where(func).ToList();
            foreach (BCB_CHECKSUB item in listBCB_CHECKSUB)
            {
                if (item.PASSTAG == "0")
                {
                    isAllPassed = false;
                    break;
                }
            }
            return isAllPassed;
        }
    }
}
