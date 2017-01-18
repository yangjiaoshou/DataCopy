using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using ConsoleApplication1;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using DataCopy.DAL;
using DataCopy.BLL;
using System.Reflection;
using DataCopy.Logic;
using DataCopy.ViewModels;

namespace DataCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument xdoc = null;
            try
            {
                xdoc = LoadDocument();
            }
            catch (Exception)
            {
                Console.WriteLine("配置文件加载失败！");
                Console.ReadKey();
                return;
            }

            string sourceTable = "vw_dime_result";
            string dataSpan = xdoc.SelectSingleNode("//InterfaceConfig//DataSpan").Attributes["Value"].Value.ToString();
            string SourceIP = xdoc.SelectSingleNode("//InterfaceConfig//SourceDatabase").Attributes["IP"].Value.ToString();
            string TargetIP = xdoc.SelectSingleNode("//InterfaceConfig//TargetDatabase").Attributes["IP"].Value.ToString();
            string SourcePort = xdoc.SelectSingleNode("//InterfaceConfig//SourceDatabase").Attributes["Port"].Value.ToString();
            string TargetPort = xdoc.SelectSingleNode("//InterfaceConfig//TargetDatabase").Attributes["Port"].Value.ToString();
            string SourceDataBase = xdoc.SelectSingleNode("//InterfaceConfig//SourceDatabase").Attributes["InstanceName"].Value.ToString();
            string TargetDataBase = xdoc.SelectSingleNode("//InterfaceConfig//TargetDatabase").Attributes["InstanceName"].Value.ToString();
            string SourceUID = xdoc.SelectSingleNode("//InterfaceConfig//SourceDatabase").Attributes["UserName"].Value.ToString();
            string TargetUID = xdoc.SelectSingleNode("//InterfaceConfig//TargetDatabase").Attributes["UserName"].Value.ToString();
            string SourcePWD = xdoc.SelectSingleNode("//InterfaceConfig//SourceDatabase").Attributes["Password"].Value.ToString();
            string TargetPWD = xdoc.SelectSingleNode("//InterfaceConfig//TargetDatabase").Attributes["Password"].Value.ToString();
            

            //同步间隔
            string SynchronizationInterval = xdoc.SelectSingleNode("//InterfaceConfig//UploadInterval").Attributes["Value"].Value.ToString(); 
            bool IsUseInterface = xdoc.SelectSingleNode("//InterfaceConfig//Interface").Attributes["IsUse"].Value.ToString()=="1"?true:false;
            string interfaceGUID = xdoc.SelectSingleNode("//InterfaceConfig//Interface").Attributes["GUID"].Value.ToString();
            string typeGUID = xdoc.SelectSingleNode("//InterfaceConfig//Interface").Attributes["TypeGUID"].Value.ToString();

            string sourceConnectionString = string.Format("server={0},{1};database={2};uid={3};pwd={4}", SourceIP, SourcePort, SourceDataBase, SourceUID, SourcePWD);
            string targetConnectionString = string.Format("server={0},{1};database={2};uid={3};pwd={4}", TargetIP, TargetPort, TargetDataBase, TargetUID, TargetPWD);

            List<BCD_ITEMQUALDESC> listAllBCD_ITEMQUALDESC = new BCD_ITEMQUALDESCBLL().GetAllList();



            while (true)
            {
                DateTime SynchronizationStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(xdoc.SelectSingleNode("//InterfaceConfig//RunTimeSpan").Attributes["StartHour"].Value), 0, 0);
                DateTime SynchronizationEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(xdoc.SelectSingleNode("//InterfaceConfig//RunTimeSpan").Attributes["EndHour"].Value), 0, 0);
                if (DateTime.Now >= SynchronizationStart && DateTime.Now < SynchronizationEnd)
                {
                    SqlConnection ConnServer = new SqlConnection(sourceConnectionString);
                    SqlConnection ConnTarget = new SqlConnection(targetConnectionString);


                    DataSet dsSource = DataRetriver.GetData(xdoc.SelectSingleNode("//InterfaceConfig//SourceDatabase").Attributes["DatabaseType"].Value.ToString() == "SqlServer" ? DataBase.SqlServer : DataBase.Oracle, SourceIP, SourcePort, SourceDataBase, SourceUID, SourcePWD, sourceTable,dataSpan);
                    List<VW_DIME_RESULT> listVM_DIME_RESULT = IsUseInterface ? DataSetConverter.ConvertToVM_DIME_RESULT(dsSource, interfaceGUID, typeGUID) : DataSetConverter.ConvertToVM_DIME_RESULT(dsSource);
                    List<BCD_ITEM> listAllBCD_ITEM = new BCD_ITEMBLL().GetALL();
                    Dictionary<string, List<string>> dic = batchUpdate(listVM_DIME_RESULT, listAllBCD_ITEMQUALDESC,listAllBCD_ITEM);
                    BCD_GROUPBLL bCD_GROUPBLL = new BCD_GROUPBLL();
                    foreach (var item in dic)
                    {
                        string groupName = string.Format("体检小组：{0}", bCD_GROUPBLL.GetBy(u => u.GUID == item.Key).GROUPNAME);
                        LogHelper.WriteInfo(MethodBase.GetCurrentMethod().DeclaringType, groupName);
                        Console.WriteLine(groupName);
                        string personCount = string.Format("更新人数:{0}", item.Value.Count.ToString());
                        LogHelper.WriteInfo(MethodBase.GetCurrentMethod().DeclaringType, personCount);
                        Console.WriteLine(personCount);
                        Console.WriteLine("------------------------------------------");
                    }
                    Console.WriteLine("暂停" + SynchronizationInterval + "分钟");
                    System.Threading.Thread.Sleep(TimeSpan.FromMinutes(double.Parse(SynchronizationInterval)));
                }
                //解决CPU占用慢的问题
                System.Threading.Thread.Sleep(100);
            }
        }

        static XmlDocument LoadDocument()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Path.Combine(Environment.CurrentDirectory, "SQLConfiguration.xml"));
            return xdoc;
        }


        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="listVW_DIME_RESULT"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> batchUpdate(List<VW_DIME_RESULT> listVW_DIME_RESULT, List<BCD_ITEMQUALDESC> listALLBCD_ITEMQUALDESC,List<BCD_ITEM> listALLBCD_ITEM)
        {
            BCB_CHECKSUBBLL bCB_CHECKSUBBLL = new BCB_CHECKSUBBLL();
            BCB_CHECKFLOWBLL bCB_CHECKFLOWBLL = new BCB_CHECKFLOWBLL();
            BCD_GROUPBLL bCD_GROUPBLL = new BCD_GROUPBLL();
            BCD_ITEMBLL bCD_ITEMBLL = new BCD_ITEMBLL();
            BCB_GROUPCONCLUSIONBLL bCB_GROUPCONCLUSIONBLL = new BCB_GROUPCONCLUSIONBLL();
            var queryByCheckCode = (from p in listVW_DIME_RESULT group p by p.CHECKCODE into g select g).ToList();
            Dictionary<string, List<string>> dicResult = new Dictionary<string, List<string>>();//key为小组GUID，value为体检编号

            foreach (var item in queryByCheckCode)
            {
                BCB_CHECKMAIN bCB_CHECKMAIN = new BCB_CHECKMAINBLL().GetBy(a => a.CHECKCODE == item.Key);
                List<string> listGROUPGUID = new List<string>();
                int completeTag = 0;
                if (bCB_CHECKMAIN != null)
                {
                    int.TryParse(bCB_CHECKMAIN.COMPLETETAG, out completeTag);
                }
                if (bCB_CHECKMAIN == null ||completeTag>1)
                {
                    Console.WriteLine(string.Format("未查询到编号为:{0}的人员信息",item.Key));
                    continue;
                }
                List<BCB_CHECKFLOW> listBCB_CHECKFLOW = bCB_CHECKFLOWBLL.GetListBy(u => u.MAINGUID == bCB_CHECKMAIN.GUID);
                if (listBCB_CHECKFLOW.Count > 0)
                {
                    for (int i = 0; i < listBCB_CHECKFLOW.Count; i++)
                    {
                        string groupGUID = listBCB_CHECKFLOW[i].GROUPGUID;
                    }
                }
                foreach (VW_DIME_RESULT vw_dime_result in item.ToList())
                {
                    int itemId = 0;
                    int.TryParse(vw_dime_result.ITEMEID, out itemId);
                    if (itemId != 0)
                    {
                        BCD_ITEM bCD_ITEM = listALLBCD_ITEM.Where(u => u.ID == itemId).FirstOrDefault();
                            //= new BCD_ITEMBLL().GetBy(a => a.ID == itemId);
                        
                        //if (bCD_ITEM!=null&&listRevertBCB_CHECKFLOW.Where(u => u.GROUPGUID == bCD_ITEM.GROUPGUID).Count() > 0)
                        //{
                        //    continue;
                        //}
                        BCB_CHECKSUB bCB_CHECKSUB = bCB_CHECKSUBBLL.GetBy(a => a.ITEMGUID == bCD_ITEM.GUID && a.MAINGUID == bCB_CHECKMAIN.GUID);
                        if (bCB_CHECKSUB == null)
                        {
                            continue;
                        }
                        BCB_CHECKFLOW bCB_CHECKFLOW = bCB_CHECKFLOWBLL.GetBy(a => a.MAINGUID == bCB_CHECKMAIN.GUID && a.GROUPGUID == bCB_CHECKSUB.GROUPGUID);
                        if (bCB_CHECKFLOW == null)
                        {
                            Console.WriteLine(string.Format("未查询到主表GUID为{0},小组GUID为{1}的流程表信息",bCB_CHECKMAIN.GUID,bCB_CHECKSUB.GROUPGUID));
                            continue;
                        }
                        if (bCB_CHECKFLOW.COMPLETETAG == "1" && vw_dime_result.cp_uncheckflag==0)
                        {
                            continue;
                        }
                        
                        if (bCB_CHECKSUB != null)
                        {
                            if (!listGROUPGUID.Contains(bCB_CHECKFLOW.GROUPGUID))
                            {
                                listGROUPGUID.Add(bCB_CHECKFLOW.GROUPGUID);
                            }
                            if (!dicResult.ContainsKey(bCB_CHECKFLOW.GROUPGUID))
                            {
                                dicResult.Add(bCB_CHECKFLOW.GROUPGUID, new List<string> { item.Key });
                            }
                            else if(!dicResult[bCB_CHECKFLOW.GROUPGUID].Contains(item.Key))
                            {
                                dicResult[bCB_CHECKFLOW.GROUPGUID].Add(item.Key);
                            }
                            bCB_CHECKSUB.RESULT = vw_dime_result.RESULT;
                            double result = 0;
                            if (double.TryParse(bCB_CHECKSUB.RESULT, out result))
                            {
                                //定量
                                bCB_CHECKSUB.PASSTAG = (result >= bCB_CHECKSUB.REFVALMIN && result <= bCB_CHECKSUB.REFVALMAX) ? "1" : "0";
                            }
                            else
                            {
                                //定性
                                List<BCD_ITEMQUALDESC> listBCD_ITEMQUALDESC = new BCD_ITEMQUALDESCBLL().GetListBy(u => u.ITEMGUID == bCB_CHECKSUB.ITEMGUID && u.PASS == "1");
                                bCB_CHECKSUB.PASSTAG = "0";
                                foreach (var description in listBCD_ITEMQUALDESC)
                                {
                                    if (description.DESCRIPTION == bCB_CHECKSUB.RESULT)
                                    {
                                        bCB_CHECKSUB.PASSTAG = "1";
                                        break;
                                    }
                                }
                            }
                            bCB_CHECKSUB.DOCTORNAME = vw_dime_result.DOCTORNAME;
                            bCB_CHECKSUB.DATECHECK = vw_dime_result.DATECHECK;
                            bCB_CHECKSUB.RECHECKDOCTORNAME = vw_dime_result.RECHECKDOCTORNAME;
                            bCB_CHECKSUB.RECHECKDATE = vw_dime_result.RECHECKDATE;
                        }
                    }


                }
                bCB_CHECKSUBBLL.SaveChanges();
                #region 更新流程表完成标记
                //更新流程表完成标记
                List<string> updatedGroupGUID = new List<string>();
                foreach (var groupGUID in listGROUPGUID.Distinct())
                {
                    BCD_GROUP bCD_GROUP = bCD_GROUPBLL.GetBy(u => u.GUID == groupGUID);
                    if (bCD_GROUP == null)
                    {
                        Console.WriteLine(string.Format("未查询到GUID为{0}的小组",groupGUID));
                        continue;
                    }
                    if (bCD_GROUP.ISPRECONDITION == "1")
                    {
                        continue;
                    }
                    List<BCB_CHECKSUB> listBCB_CHECKSUB = bCB_CHECKSUBBLL.GetListBy(u => u.MAINGUID == bCB_CHECKMAIN.GUID && u.GROUPGUID == groupGUID);
                    bool allCompleted = true;
                    foreach (var checksub in listBCB_CHECKSUB)
                    {
                        if (string.IsNullOrEmpty(checksub.RESULT))
                        {
                            allCompleted = false;
                            break;
                        }
                    }
                    bool isPreconditionComplete = true;

                    if (bCD_GROUP != null)
                    {
                        if (bCD_GROUP.NEEDPRECONDITION == "1")
                        {
                            BCD_GROUP preCondition = bCD_GROUPBLL.GetBy(u => u.GUID == bCD_GROUP.PRECONDITION);
                            if (preCondition == null)
                            {
                                Console.WriteLine(string.Format("未查询到{0}对应的前置小组", bCD_GROUP.GROUPNAME));
                            }
                            else
                            {
                                BCB_CHECKFLOW bCB_CHECKFLOW = new BCB_CHECKFLOWBLL().GetBy(u => u.GROUPGUID == preCondition.GUID && u.MAINGUID == bCB_CHECKMAIN.GUID);
                                if (bCB_CHECKFLOW != null)
                                {
                                    if (bCB_CHECKFLOW.COMPLETETAG != "1")
                                    {
                                        isPreconditionComplete = false;
                                    }
                                }
                            }
                        }
                    }
                    if (allCompleted && isPreconditionComplete)
                    {
                        updatedGroupGUID.Add(groupGUID);
                    }
                }
                new BCB_CHECKFLOWBLL().UpdateByMainGUIDANDGroupGUID(bCB_CHECKMAIN.GUID, updatedGroupGUID.Distinct().ToList()); 
                #endregion
                //更新小组结论
                foreach (var groupGUID in updatedGroupGUID.Distinct().ToList())
                {
                    List<BCBCHECKSUB> listBCBCHECKSUB = new List<BCBCHECKSUB>();
                    List<BCB_CHECKSUB> list = bCB_CHECKSUBBLL.GetListBy(u => u.GROUPGUID == groupGUID && u.MAINGUID == bCB_CHECKMAIN.GUID);
                    foreach (var BCB_CHECKSUB in list)
                    {
                        BCBCHECKSUB temp = new BCBCHECKSUB();
                        temp.BCB_CHECKSUB = BCB_CHECKSUB;
                        temp.BCDITEM = new BCDITEM
                        {
                            BCD_ITEM = bCD_ITEMBLL.GetBy(u => u.GUID == BCB_CHECKSUB.ITEMGUID)
                        };
                        listBCBCHECKSUB.Add(temp);
                    }
                    if (list.Count > 0)
                    {
                        string result = ResultGenerator.GenerateResult(listBCBCHECKSUB, listALLBCD_ITEMQUALDESC);
                       BCB_GROUPCONCLUSION bCB_GROUPCONCLUSION = bCB_GROUPCONCLUSIONBLL.GetBy(u => u.MAINGUID == bCB_CHECKMAIN.GUID && u.GROUPGUID == groupGUID);
                       if (bCB_GROUPCONCLUSION == null)
                       {
                           bCB_GROUPCONCLUSION = new BCB_GROUPCONCLUSION();
                           bCB_GROUPCONCLUSION.GUID = Guid.NewGuid().ToString();
                           bCB_GROUPCONCLUSION.MAINGUID = bCB_CHECKMAIN.GUID;
                           bCB_GROUPCONCLUSION.GROUPGUID = groupGUID;
                           bCB_GROUPCONCLUSION.CONSLUSION = result;
                           bCB_GROUPCONCLUSION.TIMECREATE = DateTime.Now;
                           bCB_GROUPCONCLUSIONBLL.Add(bCB_GROUPCONCLUSION);
                       }
                       else {
                           bCB_GROUPCONCLUSION.MAINGUID = bCB_CHECKMAIN.GUID;
                           bCB_GROUPCONCLUSION.CONSLUSION = result;

                           bCB_GROUPCONCLUSIONBLL.SaveChanges();
                       }
                    }
                }

                List<BCB_GROUPCONCLUSION> listBCB_GROUPCONCLUSION =bCB_GROUPCONCLUSIONBLL.GetListBy(u=>u.MAINGUID==bCB_CHECKMAIN.GUID);
                StringBuilder sbResult = new StringBuilder(1024);
                foreach (var bCB_GROUPCONCLUSION in listBCB_GROUPCONCLUSION)
                {
                    if (!string.IsNullOrEmpty(bCB_GROUPCONCLUSION.CONSLUSION) && bCB_GROUPCONCLUSION.CONSLUSION != "未见明显异常")
                    {
                        sbResult.AppendFormat("[{0}]{1}", bCD_GROUPBLL.GetBy(u=>u.GUID==bCB_GROUPCONCLUSION.GROUPGUID).GROUPNAME, bCB_GROUPCONCLUSION.CONSLUSION);
                        sbResult.Append(Environment.NewLine);
                    }
                }
                if (string.IsNullOrEmpty(sbResult.ToString()))
                {
                    sbResult.Append("所检项目未见明显异常");
                }
                else
                {
                    sbResult.Append("其余所检项目未见明显异常");
                }
                //更新主表完成标记
                bool isAllPassed = bCB_CHECKSUBBLL.IsAllPassed(u => u.MAINGUID == bCB_CHECKMAIN.GUID);
                int j = new BCB_CHECKMAINBLL().UpdateAfterUpload(bCB_CHECKMAIN.GUID,isAllPassed,sbResult.ToString().Trim());


                //int j = new BCB_CHECKMAINBLL().UpdateIfAllCompleted(bCB_CHECKMAIN.GUID, isAllPassed, sbResult.ToString().Trim(), targetConnString);

            }
            return dicResult;
        }
    }

}
