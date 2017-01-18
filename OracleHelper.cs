﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace DataCopy
{
    public class OracleHelper
    {
        protected OracleConnection Connection;

        private string connectionString;

        public OracleHelper()
        {
            string connStr;

            connStr = ConfigurationSettings.AppSettings["DbOracle"].ToString();

            connectionString = connStr;

            Connection = new OracleConnection(connectionString);

        }

        #region 带参数的构造函数

        /// <summary>

        /// 带参数的构造函数

        /// </summary>

        /// <param name="ConnString">数据库联接字符串</param>

        public OracleHelper(string ConnString)
        {

            string connStr;

            connStr =

               ConnString;

            Connection = new OracleConnection(connStr);

        }

        #endregion

        #region 打开数据库

        /// <summary>

        /// 打开数据库

        /// </summary>

        public void OpenConn()
        {

            if (this.Connection.State != ConnectionState.Open)

                this.Connection.Open();

        }

        #endregion

        #region 关闭数据库联接

        /// <summary>

        /// 关闭数据库联接

        /// </summary>

        public void CloseConn()
        {

            if (Connection.State == ConnectionState.Open)

                Connection.Close();

        }

        #endregion

        #region 执行SQL语句，返回数据到DataSet中
        public DataSet ReturnDataSet(string sql)
        {
            return ReturnDataSet(sql, "table1");
        }
        #endregion

        #region 执行SQL语句，返回数据到DataSet中

        /// <summary>
        /// 执行SQL语句，返回数据到DataSet中
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="DataSetName">自定义返回的DataSet表名</param>
        /// <returns>返回DataSet</returns>
        public DataSet ReturnDataSet(string sql, string DataSetName)
        {
            DataSet dataSet = new DataSet();
            OpenConn();
            OracleDataAdapter OraDA = new OracleDataAdapter(sql, Connection);
            OraDA.Fill(dataSet, DataSetName);
            //    CloseConn();
            return dataSet;
        }

        #endregion

        #region 执行Sql语句,返回带分页功能的dataset

        /// <summary>

        /// 执行Sql语句,返回带分页功能的dataset

        /// </summary>

        /// <param name="sql">Sql语句</param>

        /// <param name="PageSize">每页显示记录数</param>

        /// <param name="CurrPageIndex"><当前页/param>

        /// <param name="DataSetName">返回dataset表名</param>

        /// <returns>返回DataSet</returns>

        public DataSet ReturnDataSet(string sql, int PageSize,

            int CurrPageIndex, string DataSetName)
        {

            DataSet dataSet = new DataSet();

            OpenConn();

            OracleDataAdapter OraDA =

                new OracleDataAdapter(sql, Connection);

            OraDA.Fill(dataSet, PageSize * (CurrPageIndex - 1),

                PageSize, DataSetName);

            //    CloseConn();

            return dataSet;

        }

        #endregion

        #region 返回记录数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sTableName"></param>
        /// <param name="sWhere"></param>
        /// <returns></returns>
        public int ReturnRecordCount(string sTableName, string sWhere)
        {
            int iCount = 0;
            string sql = string.Format("select count(ID) from {0} where 1=1 {1}", sTableName, sWhere);
            DataSet ds = ReturnDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    iCount = Convert.ToInt32(dt.Rows[0][0]);
                }
            }
            return iCount;
        }
        #endregion

        #region 执行SQL语句，返回 DataReader,用之前一定要先.read()打开,然后才能读到数据

        /// <summary>

        /// 执行SQL语句，返回 DataReader,用之前一定要先.read()打开,然后才能读到数据

        /// </summary>

        /// <param name="sql">sql语句</param>

        /// <returns>返回一个OracleDataReader</returns>

        public OracleDataReader ReturnDataReader(String sql)
        {

            OpenConn();

            OracleCommand command = new OracleCommand(sql, Connection);

            return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

        }

        #endregion

        #region 执行SQL语句，返回记录总数数

        /// <summary>

        /// 执行SQL语句，返回记录总数数

        /// </summary>

        /// <param name="sql">sql语句</param>

        /// <returns>返回记录总条数</returns>

        public int GetRecordCount(string sql)
        {

            int recordCount = 0;

            OpenConn();

            OracleCommand command = new OracleCommand(sql, Connection);

            OracleDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {

                recordCount++;

            }

            dataReader.Close();

            //    CloseConn();

            return recordCount;

        }

        #endregion

        #region 取当前序列,条件为seq.nextval或seq.currval

        /// <summary>

        /// 取当前序列

        /// </summary>

        /// <param name="seqstr"></param>

        /// <param name="table"></param>

        /// <returns></returns>

        public decimal GetSeq(string seqstr)
        {

            decimal seqnum = 0;

            string sql = "select " + seqstr + " from dual";

            OpenConn();

            OracleCommand command = new OracleCommand(sql, Connection);

            OracleDataReader dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {

                seqnum = decimal.Parse(dataReader[0].ToString());

            }

            dataReader.Close();

            //    CloseConn();

            return seqnum;

        }

        #endregion

        #region 执行SQL语句,返回所影响的行数

        /// <summary>

        /// 执行SQL语句,返回所影响的行数

        /// </summary>

        /// <param name="sql"></param>

        /// <returns></returns>

        public int ExecuteSQL(string sql)
        {

            int Cmd = 0;

            OpenConn();

            OracleCommand command = new OracleCommand(sql, Connection);

            try
            {

                Cmd = command.ExecuteNonQuery();

            }

            catch
            {



            }

            finally
            {

                //     CloseConn();

            }



            return Cmd;

        }

        #endregion

        #region 执行SQL语句
        public bool EditDatabase(string sql)
        {
            return ExecuteSQL(sql) > 0;
        }
        #endregion

        //　＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

        //　用hashTable对数据库进行insert,update,del操作,注意此时只能

        //   用默认的数据库连接"connstr"

        //　＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

        #region 根据表名及哈稀表自动插入数据库 用法：Insert("test",ht)

        public int Insert(string TableName, Hashtable ht)
        {

            OracleParameter[] Parms = new OracleParameter[ht.Count];

            IDictionaryEnumerator et = ht.GetEnumerator();

            DataTable dt = GetTabType(TableName);

            System.Data.OracleClient.OracleType otype;

            int size = 0;

            int i = 0;



            while (et.MoveNext()) // 作哈希表循环
            {

                GetoType(et.Key.ToString().ToUpper(), dt, out otype, out size);

                System.Data.OracleClient.OracleParameter op = MakeParam(":" +

                    et.Key.ToString(), otype, size, et.Value.ToString());

                Parms[i] = op; // 添加SqlParameter对象

                i = i + 1;

            }

            string str_Sql = GetInsertSqlbyHt(TableName, ht); // 获得插入sql语句

            int val = ExecuteNonQuery(str_Sql, Parms);

            return val;

        }

        #endregion

        #region 根据相关条件对数据库进行更新操作 用法：Update("test","Id=:Id",ht);

        public int Update(string TableName, string ht_Where, Hashtable ht)
        {

            OracleParameter[] Parms = new OracleParameter[ht.Count];

            IDictionaryEnumerator et = ht.GetEnumerator();

            DataTable dt = GetTabType(TableName);

            System.Data.OracleClient.OracleType otype;

            int size = 0;

            int i = 0;

            // 作哈希表循环

            while (et.MoveNext())
            {

                GetoType(et.Key.ToString().ToUpper(), dt, out otype, out size);

                System.Data.OracleClient.OracleParameter op =

                    MakeParam(":" + et.Key.ToString(), otype, size, et.Value.ToString());

                Parms[i] = op; // 添加SqlParameter对象

                i = i + 1;

            }

            string str_Sql = GetUpdateSqlbyHt(TableName, ht_Where, ht); // 获得插入sql语句

            int val = ExecuteNonQuery(str_Sql, Parms);

            return val;

        }

        #endregion

        #region del操作

        //,注意此处条件个数与hash里参数个数应该一致

        //用法：Del("test","Id=:Id",ht)

        public int Del(string TableName, string ht_Where, Hashtable ht)
        {

            OracleParameter[] Parms = new OracleParameter[ht.Count];

            IDictionaryEnumerator et = ht.GetEnumerator();

            DataTable dt = GetTabType(TableName);

            System.Data.OracleClient.OracleType otype;

            int i = 0;

            int size = 0;

            // 作哈希表循环

            while (et.MoveNext())
            {

                GetoType(et.Key.ToString().ToUpper(), dt, out otype, out size);

                System.Data.OracleClient.OracleParameter op =

                    MakeParam(":" + et.Key.ToString(), et.Value.ToString());

                Parms[i] = op; // 添加SqlParameter对象

                i = i + 1;

            }

            string str_Sql = GetDelSqlbyHt(TableName, ht_Where, ht); // 获得删除sql语句

            int val = ExecuteNonQuery(str_Sql, Parms);

            return val;

        }

        #endregion

        //　＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

        //　＝＝＝＝＝＝＝＝上面三个操作的内部调用函数＝＝＝＝＝＝＝

        //　＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

        #region 根据哈稀表及表名自动生成相应insert语句(参数类型的)

        /// <summary>

        /// 根据哈稀表及表名自动生成相应insert语句

        /// </summary>

        /// <param name="TableName">要插入的表名</param>

        /// <param name="ht">哈稀表</param>

        /// <returns>返回sql语句</returns>

        public static string GetInsertSqlbyHt(string TableName, Hashtable ht)
        {

            string str_Sql = "";

            int i = 0;

            int ht_Count = ht.Count; // 哈希表个数

            IDictionaryEnumerator myEnumerator = ht.GetEnumerator();

            string before = "";

            string behide = "";

            while (myEnumerator.MoveNext())
            {

                if (i == 0)
                {

                    before = "(" + myEnumerator.Key;

                }

                else if (i + 1 == ht_Count)
                {

                    before = before + "," + myEnumerator.Key + ")";

                }

                else
                {

                    before = before + "," + myEnumerator.Key;

                }

                i = i + 1;

            }

            behide = " Values" + before.Replace(",", ",:").Replace("(", "(:");

            str_Sql = "Insert into " + TableName + before + behide;

            return str_Sql;

        }

        #endregion

        #region 根据表名，where条件，哈稀表自动生成更新语句(参数类型的)

        public static string GetUpdateSqlbyHt(string Table,

            string ht_Where, Hashtable ht)
        {

            string str_Sql = "";

            int i = 0;

            int ht_Count = ht.Count; // 哈希表个数

            IDictionaryEnumerator myEnumerator = ht.GetEnumerator();

            while (myEnumerator.MoveNext())
            {

                if (i == 0)
                {

                    if (ht_Where.ToString().ToLower().IndexOf((myEnumerator.Key +

                        "=:" + myEnumerator.Key).ToLower()) == -1)
                    {

                        str_Sql = myEnumerator.Key + "=:" + myEnumerator.Key;

                    }

                }

                else
                {

                    if (ht_Where.ToString().ToLower().IndexOf((":" +

                        myEnumerator.Key + " ").ToLower()) == -1)
                    {

                        str_Sql = str_Sql + "," + myEnumerator.Key + "=:" + myEnumerator.Key;

                    }



                }

                i = i + 1;

            }

            if (ht_Where == null || ht_Where.Replace(" ", "") == "") // 更新时候没有条件
            {

                str_Sql = "update " + Table + " set " + str_Sql;

            }

            else
            {

                str_Sql = "update " + Table + " set " + str_Sql + " where " + ht_Where;

            }

            str_Sql = str_Sql.Replace("set ,", "set ").Replace("update ,", "update ");

            return str_Sql;

        }

        #endregion

        #region 根据表名，where条件，哈稀表自动生成del语句(参数类型的)

        public static string GetDelSqlbyHt(string Table,

            string ht_Where, Hashtable ht)
        {

            string str_Sql = "";

            int i = 0;



            int ht_Count = ht.Count; // 哈希表个数

            IDictionaryEnumerator myEnumerator = ht.GetEnumerator();

            while (myEnumerator.MoveNext())
            {

                if (i == 0)
                {

                    if (ht_Where.ToString().ToLower().IndexOf((myEnumerator.Key +

                        "=:" + myEnumerator.Key).ToLower()) == -1)
                    {

                        str_Sql = myEnumerator.Key + "=:" + myEnumerator.Key;

                    }

                }

                else
                {

                    if (ht_Where.ToString().ToLower().IndexOf((":" +

                        myEnumerator.Key + " ").ToLower()) == -1)
                    {

                        str_Sql = str_Sql + "," + myEnumerator.Key + "=:" + myEnumerator.Key;

                    }



                }

                i = i + 1;

            }

            if (ht_Where == null || ht_Where.Replace(" ", "") == "") // 更新时候没有条件
            {

                str_Sql = "Delete " + Table;

            }

            else
            {

                str_Sql = "Delete " + Table + " where " + ht_Where;

            }

            return str_Sql;

        }

        #endregion

        #region 生成oracle参数

        /// <summary>

        /// 生成oracle参数

        /// </summary>

        /// <param name="ParamName">字段名</param>

        /// <param name="otype">数据类型</param>

        /// <param name="size">数据大小</param>

        /// <param name="Value">值</param>

        /// <returns></returns>

        public static OracleParameter MakeParam(string ParamName,

            System.Data.OracleClient.OracleType otype, int size, Object Value)
        {

            OracleParameter para = new OracleParameter(ParamName, Value);

            para.OracleType = otype;

            para.Size = size;

            return para;

        }

        #endregion

        #region 生成oracle参数

        public static OracleParameter MakeParam(string ParamName, string Value)
        {

            return new OracleParameter(ParamName, Value);

        }

        #endregion

        #region 根据表结构字段的类型和长度拼装oracle sql语句参数

        public static void GetoType(string key, DataTable dt,

            out System.Data.OracleClient.OracleType otype, out int size)
        {



            DataView dv = dt.DefaultView;

            dv.RowFilter = "column_name='" + key + "'";

            string fType = dv[0]["data_type"].ToString().ToUpper();

            switch (fType)
            {

                case "DATE":

                    otype = OracleType.DateTime;

                    size = int.Parse(dv[0]["data_length"].ToString());

                    break;

                case "CHAR":

                    otype = OracleType.Char;

                    size = int.Parse(dv[0]["data_length"].ToString());

                    break;

                case "LONG":

                    otype = OracleType.Double;

                    size = int.Parse(dv[0]["data_length"].ToString());

                    break;

                case "NVARCHAR2":

                    otype = OracleType.NVarChar;

                    size = int.Parse(dv[0]["data_length"].ToString());

                    break;

                case "VARCHAR2":

                    otype = OracleType.NVarChar;

                    size = int.Parse(dv[0]["data_length"].ToString());

                    break;

                default:

                    otype = OracleType.NVarChar;

                    size = 100;

                    break;

            }

        }

        #endregion

        #region 动态 取表里字段的类型和长度,此处没有动态用到connstr,是默认的！

        public System.Data.DataTable GetTabType(string tabnale)
        {

            string sql = "select column_name,data_type,data_length " +

                "from all_tab_columns where table_name='" + tabnale.ToUpper() + "'";

            OpenConn();

            return (ReturnDataSet(sql, "dv")).Tables[0];



        }

        #endregion

        #region 执行sql语句

        public int ExecuteNonQuery(string cmdText, params OracleParameter[] cmdParms)
        {
            OracleCommand cmd = new OracleCommand();

            OpenConn();

            cmd.Connection = Connection;

            cmd.CommandText = cmdText;

            if (cmdParms != null)
            {

                foreach (OracleParameter parm in cmdParms)

                    cmd.Parameters.Add(parm);

            }

            int val = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();

            //    conn.CloseConn();

            return val;

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bNextValue">true:return next value, false:return current value</param>
        /// <param name="sSeqName"></param>
        /// <returns></returns>
        public int GetSequence(bool bNextValue, string sSeqName)
        {
            int i = -1;
            try
            {
                string sql = string.Format("select {0}.{1} from dual", sSeqName, bNextValue ? "nextval" : "currval");
                DataSet ds = ReturnDataSet(sql);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {

                        i = Convert.ToInt32(dt.Rows[0][0]);

                    }
                }
            }
            catch { }
            return i;
        }

        #region 执行事务
        public bool RunTrans(List<string> sql)
        {
            bool bTag = true;
            OpenConn();
            OracleTransaction trans = Connection.BeginTransaction();
            try
            {
                OracleCommand cmd = Connection.CreateCommand();
                cmd.Transaction = trans;
                for (int i = 0; i < sql.Count; i++)
                {
                    cmd.CommandText = sql[i];
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                bTag = false;
            }
            finally
            {
                Connection.Close();
            }
            return bTag;
        }
        #endregion
    }
}
