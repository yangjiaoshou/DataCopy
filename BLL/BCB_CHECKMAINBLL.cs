using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.DAL;
using System.Linq.Expressions;

namespace DataCopy.BLL
{
    class BCB_CHECKMAINBLL
    {
        BCB_CHECKMAINDAL dal = new BCB_CHECKMAINDAL();

        /// <summary>
        /// 更新体检主表
        /// </summary>
        /// <param name="MainGUID">主表GUID</param>
        /// <returns></returns>
        public int UpdateAfterUpload(string MainGUID,bool isAllPassed,string checkResult)
        {
            List<BCB_CHECKFLOW> listBCB_CHECKFLOW = new BCB_CHECKFLOWDAL().getListByMainGUID(MainGUID);
            bool isAllCompleted = true;
            foreach (BCB_CHECKFLOW checkflow in listBCB_CHECKFLOW)
            {
                if (checkflow.COMPLETETAG != "1")
                {
                    isAllCompleted = false;
                    break;
                }
            }
            //if (isAllCompleted)
            //{
                return dal.Update(MainGUID, isAllCompleted, isAllPassed, checkResult);
            //}
            //return 0;
        }


        public BCB_CHECKMAIN GetBy(Expression<Func<BCB_CHECKMAIN,bool>> func)
        {
            return dal.GetBy(func);
        }
    }
}
