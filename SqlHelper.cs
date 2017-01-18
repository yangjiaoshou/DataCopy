
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections;
using System.Data.SqlClient;
 
/// <summary>
/// 数据库的通用访问代码
/// 此类为抽象类，不允许实例化，在应用时直接调用即可
/// </summary>
public abstract class SqlHelper
{
    //获取数据库连接字符串，其属于静态变量且只读，项目中所有文档可以直接使用，但不能修改
    public static readonly string ConnectionStringLocalTransaction = "server=192.168.188.100,1425;database=HKM_SYS;uid=develop;pwd=Develop123!@#;";
 
    // 哈希表用来存储缓存的参数信息，哈希表可以存储任意类型的参数。
    private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

            /// <summary>   
       /// 将SqlParameter参数数组(参数值)分配给SqlCommand命令.   
       /// 这个方法将给任何一个参数分配DBNull.Value;   
       /// 该操作将阻止默认值的使用.   
       /// </summary>   
       /// <param name="command">命令名</param>   
       /// <param name="commandParameters">SqlParameters数组</param>   
       private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)  
       {   
           if (command == null) throw new ArgumentNullException("command");   
           if (commandParameters != null)   
           {   
               foreach (SqlParameter p in commandParameters)   
               {   
                   if (p != null)   
                   {   
                       // 检查未分配值的输出参数,将其分配以DBNull.Value.   
                       if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) &&   
                           (p.Value == null))   
                       {   
                           p.Value = DBNull.Value;   
                       }   
                       command.Parameters.Add(p);   
                   }   
               }   
           }   
       }  

 
    /// <summary>
    ///执行一个不需要返回值的SqlCommand命令，通过指定专用的连接字符串。
    /// 使用参数数组形式提供参数列表 
    /// </summary>
    /// <remarks>
    /// 使用示例：
    ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    /// </remarks>
    /// <param name="connectionString">一个有效的数据库连接字符串</param>
    /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
    /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
    /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
    /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
    public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
 
        SqlCommand cmd = new SqlCommand();
 
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
 
            //清空SqlCommand中的参数列表
            cmd.Parameters.Clear();
            return val;
        }
    }
     
    /// <summary>
    ///执行一条不返回结果的SqlCommand，通过一个已经存在的数据库连接 
    /// 使用参数数组提供参数
    /// </summary>
    /// <remarks>
    /// 使用示例：  
    ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    /// </remarks>
    /// <param name="conn">一个现有的数据库连接</param>
    /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
    /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
    /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
    /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
    public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
 
        SqlCommand cmd = new SqlCommand();
 
        PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
        int val = cmd.ExecuteNonQuery();
        cmd.Parameters.Clear();
        return val;
    }
 
    /// <summary>
    /// 执行一条不返回结果的SqlCommand，通过一个已经存在的数据库事物处理 
    /// 使用参数数组提供参数
    /// </summary>
    /// <remarks>
    /// 使用示例： 
    ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    /// </remarks>
    /// <param name="trans">一个存在的 sql 事物处理</param>
    /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
    /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
    /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
    /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
    public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
        SqlCommand cmd = new SqlCommand();
        PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
        int val = cmd.ExecuteNonQuery();
        cmd.Parameters.Clear();
        return val;
    }
 
    /// <summary>
    /// 执行一条返回结果集的SqlCommand命令，通过专用的连接字符串。
    /// 使用参数数组提供参数
    /// </summary>
    /// <remarks>
    /// 使用示例：  
    ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    /// </remarks>
    /// <param name="connectionString">一个有效的数据库连接字符串</param>
    /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
    /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
    /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
    /// <returns>返回一个包含结果的SqlDataReader</returns>
    public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
        SqlCommand cmd = new SqlCommand();
        SqlConnection conn = new SqlConnection(connectionString);
 
        // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
        //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
        //关闭数据库连接，并通过throw再次引发捕捉到的异常。
        try
        {
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;
        }
        catch
        {
            conn.Close();
            throw;
        }
    }
 
    /// <summary>
    /// 执行一条返回第一条记录第一列的SqlCommand命令，通过专用的连接字符串。 
    /// 使用参数数组提供参数
    /// </summary>
    /// <remarks>
    /// 使用示例：  
    ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    /// </remarks>
    /// <param name="connectionString">一个有效的数据库连接字符串</param>
    /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
    /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
    /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
    /// <returns>返回一个object类型的数据，可以通过 Convert.To{Type}方法转换类型</returns>
    public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
        SqlCommand cmd = new SqlCommand();
 
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }
    }
 
    /// <summary>
    /// 执行一条返回第一条记录第一列的SqlCommand命令，通过已经存在的数据库连接。
    /// 使用参数数组提供参数
    /// </summary>
    /// <remarks>
    /// 使用示例： 
    ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    /// </remarks>
    /// <param name="conn">一个已经存在的数据库连接</param>
    /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
    /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
    /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
    /// <returns>返回一个object类型的数据，可以通过 Convert.To{Type}方法转换类型</returns>
    public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
 
        SqlCommand cmd = new SqlCommand();
 
        PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
        object val = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
        return val;
    }


   /// <summary>   
       /// 执行指定数据库连接对象的命令,指定存储过程参数,返回DataSet.   
       /// </summary>   
       /// <remarks>   
       /// 示例:    
       ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));   
       /// </remarks>   
       /// <param name="connection">一个有效的数据库连接对象</param>   
       /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>   
       /// <param name="commandText">存储过程名或T-SQL语句</param>   
       /// <param name="commandParameters">SqlParamter参数数组</param>   
       /// <returns>返回一个包含结果集的DataSet</returns>   
       public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)   
       {   
           if (connection == null) throw new ArgumentNullException("connection");  
 
           // 预处理   
           SqlCommand cmd = new SqlCommand();   
           bool mustCloseConnection = false;   
           PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);  
 
           // 创建SqlDataAdapter和DataSet.   
           using (SqlDataAdapter da = new SqlDataAdapter(cmd))   
           {   
               DataSet ds = new DataSet();  
 
               // 填充DataSet.   
               da.Fill(ds);  
 
               cmd.Parameters.Clear();  
 
               if (mustCloseConnection)   
                   connection.Close();  
 
               return ds;   
           }   
       }  


 
    /// <summary>
    /// 缓存参数数组
    /// </summary>
    /// <param name="cacheKey">参数缓存的键值</param>
    /// <param name="cmdParms">被缓存的参数列表</param>
    public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
    {
        parmCache[cacheKey] = commandParameters;
    }
 
    /// <summary>
    /// 获取被缓存的参数
    /// </summary>
    /// <param name="cacheKey">用于查找参数的KEY值</param>
    /// <returns>返回缓存的参数数组</returns>
    public static SqlParameter[] GetCachedParameters(string cacheKey)
    {
        SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];
 
        if (cachedParms == null)
            return null;
 
        //新建一个参数的克隆列表
        SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];
 
        //通过循环为克隆参数列表赋值
        for (int i = 0, j = cachedParms.Length; i < j; i++)
            //使用clone方法复制参数列表中的参数
            clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();
 
        return clonedParms;
    }
 
    /// <summary>
    /// 为执行命令准备参数
    /// </summary>
    /// <param name="cmd">SqlCommand 命令</param>
    /// <param name="conn">已经存在的数据库连接</param>
    /// <param name="trans">数据库事物处理</param>
    /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
    /// <param name="cmdText">Command text，T-SQL语句 例如 Select * from Products</param>
    /// <param name="cmdParms">返回带参数的命令</param>
    private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
    {
 
        //判断数据库连接状态
        if (conn.State != ConnectionState.Open)
            conn.Open();
 
        cmd.Connection = conn;
        cmd.CommandText = cmdText;
 
        //判断是否需要事物处理
        if (trans != null)
            cmd.Transaction = trans;
 
        cmd.CommandType = cmdType;
 
        if (cmdParms != null)
        {
            foreach (SqlParameter parm in cmdParms)
                cmd.Parameters.Add(parm);
        }
    }

       /// <summary>   
       /// 预处理用户提供的命令,数据库连接/事务/命令类型/参数   
       /// </summary>   
       /// <param name="command">要处理的SqlCommand</param>   
       /// <param name="connection">数据库连接</param>   
       /// <param name="transaction">一个有效的事务或者是null值</param>   
       /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>   
       /// <param name="commandText">存储过程名或都T-SQL命令文本</param>   
       /// <param name="commandParameters">和命令相关联的SqlParameter参数数组,如果没有参数为'null'</param>   
       /// <param name="mustCloseConnection"><c>true</c> 如果连接是打开的,则为true,其它情况下为false.</param>   
       private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)   
       {   
           if (command == null) throw new ArgumentNullException("command");   
           if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");  
 
           // If the provided connection is not open, we will open it   
           if (connection.State != ConnectionState.Open)   
           {   
               mustCloseConnection = true;   
               connection.Open();   
           }   
           else   
           {   
               mustCloseConnection = false;   
           }  
 
           // 给命令分配一个数据库连接.   
           command.Connection = connection;  
 
           // 设置命令文本(存储过程名或SQL语句)   
           command.CommandText = commandText;  
 
           // 分配事务   
           if (transaction != null)   
           {   
               if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");   
               command.Transaction = transaction;   
           }  
 
           // 设置命令类型.   
           command.CommandType = commandType;  
 
           // 分配命令参数   
           if (commandParameters != null)   
           {   
               AttachParameters(command, commandParameters);   
           }   
           return;   
       }  

} 
 