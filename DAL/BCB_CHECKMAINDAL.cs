using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;

namespace DataCopy.DAL
{
    public class BCB_CHECKMAINDAL
    {
        HKM_SYSEntities db = new HKM_SYSEntities();

        /// <summary>
        /// 更新主表
        /// </summary>
        /// <param name="MainGUID">主表GUID</param>
        /// <param name="isAllComplete">所有流程是否均已完成</param>
        /// <param name="isAllPassed">所有细项是否合格</param>
        /// <param name="checkResult">检查结果</param>
        /// <returns>更新条目数</returns>
        public int Update(string MainGUID,bool isAllComplete, bool isAllPassed,string checkResult)
        {
            BCB_CHECKMAIN bCB_CHECKMAIN = db.BCB_CHECKMAIN.Where(u => u.GUID == MainGUID).FirstOrDefault();
            if (bCB_CHECKMAIN != null)
            {
                if (isAllComplete)
                {
                    bCB_CHECKMAIN.COMPLETETAG = "1";
                    bCB_CHECKMAIN.CHECKRESULT = checkResult;
                    bCB_CHECKMAIN.FAILED = isAllPassed ? "0" : "1";
                }

                if (bCB_CHECKMAIN.CHECKDATE == null || bCB_CHECKMAIN.CHECKDATE == new DateTime(1900, 1, 1))
                {
                    bCB_CHECKMAIN.CHECKDATE = DateTime.Now;
                }
            }
            int count = 0;
            try
            {
                count = db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                db.Dispose();
            }
            return count;
        }

        ///// <summary>
        ///// 更新主表完成标记为1
        ///// </summary>
        ///// <param name="MainGUID">主表GUID</param>
        ///// <returns>更新条目数</returns>
        //public int Update(string MainGUID, bool isAllPassed, string checkResult,string connString)
        //{
        //   return SqlHelper.ExecuteNonQuery(connString,System.Data.CommandType.Text
        //       ,string.Format("Update BCB_CHECKMAIN SET COMPLETETAG='1',FAILED='{0}',CHECKRESULT='{1}' WHERE MAINGUID='{0}'",isAllPassed?"0":"1",checkResult,MainGUID));
        //}



        public BCB_CHECKMAIN GetBy(Expression<Func<BCB_CHECKMAIN, bool>> func)
        {
            return db.BCB_CHECKMAIN.Where(func).FirstOrDefault();
        }
    }
}
