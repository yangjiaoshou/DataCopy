using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DataCopy
{
   public class DataRetriver
    {
       public static DataSet GetData(DataBase database, string SourceIP, string SourcePort, string SourceDataBase, string SourceUID, string SourcePWD, string sourceTable,string dataSpan)
       {
           string sourceConnectionString=string.Empty;
           DataSet dsSource = null;
           if (database == DataBase.SqlServer)
           {
               sourceConnectionString = string.Format("server={0},{1};database={2};uid={3};pwd={4}", SourceIP, SourcePort, SourceDataBase, SourceUID, SourcePWD);
               SqlConnection ConnServer = new SqlConnection(sourceConnectionString);
               dsSource 
                   //= SqlHelper.ExecuteDataset(ConnServer, CommandType.Text, string.Format("select * from {0} where CHECKCODE='{1}'", sourceTable, "021607180294"), null);
                   = SqlHelper.ExecuteDataset(ConnServer, CommandType.Text, string.Format("select * from {0} where DATECHECK>='{1}' AND DATECHECK<'{2}'", sourceTable,DateTime.Now.AddDays(-double.Parse(dataSpan)),DateTime.Now.AddDays(1)), null);
           }
           else if (database == DataBase.Oracle)
           {
               sourceConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}",SourceIP,SourcePort,SourceDataBase,SourceUID,SourcePWD);
               dsSource = new OracleHelper(sourceConnectionString).ReturnDataSet(string.Format("select * from {0}.{1} where to_date(DATECHECK,'YYYY/MM/DD')>= to_date('{2}','YYYY/MM/DD')  AND to_date(DATECHECK,'YYYY/MM/DD') <= to_date('{3}','YYYY/MM/DD')", SourceDataBase, sourceTable, DateTime.Now.AddDays(-double.Parse(dataSpan)).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")));
           }

           return dsSource;
       }
    }

   public enum DataBase
   { 
      SqlServer,
       Oracle
   }
}
