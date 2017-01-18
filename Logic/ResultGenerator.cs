using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCopy.Enum;
using DataCopy.ViewModels;

namespace DataCopy.Logic
{
    /// <summary>
    /// 体检小组结论生成器
    /// </summary>
    public class ResultGenerator
    {
        public static string GenerateResult(List<BCBCHECKSUB> listBCBCHECKSUB, List<BCD_ITEMQUALDESC> listALLBCD_ITEMQUALDESC)
        {
            StringBuilder sbcheckResult = new StringBuilder(1024);
            foreach (var item in listBCBCHECKSUB)
            {
                BCD_ITEM bcd_itemEntity = item.BCDITEM.BCD_ITEM;
                string itemName = string.Empty;
                if (bcd_itemEntity.HIDEITEMNAME != "1")
                {
                    itemName = bcd_itemEntity.ITEMNAME;
                }
                bool isShow = bcd_itemEntity.SHOWINMAINCHECK == "1" ? true : false;
                if (!isShow)
                {
                    continue;
                }

                switch (bcd_itemEntity.JUDGEPARTEN)
                {
                    case "01"://定量
                        QuantityResult quantityResult =GetQuantityResult(item.BCB_CHECKSUB.RESULT, item.BCB_CHECKSUB.REFVALMAX,item.BCB_CHECKSUB.REFVALMIN);
                        if (quantityResult == QuantityResult.WRONGINPUT)
                        {
                            break;
                        }
                        else if (quantityResult == QuantityResult.HIGH)
                        {
                            string toohigh = string.IsNullOrEmpty(bcd_itemEntity.HISTR) ? "偏高" : bcd_itemEntity.HISTR;
                            sbcheckResult.AppendFormat("{0}{1}:{2}{3};", itemName, toohigh, item.BCB_CHECKSUB.RESULT, bcd_itemEntity.UNIT);
                        }
                        else if (quantityResult == QuantityResult.LOW)
                        {
                            string toolow = string.IsNullOrEmpty(bcd_itemEntity.LWSTR) ? "偏低" : bcd_itemEntity.LWSTR;
                            sbcheckResult.AppendFormat("{0}{1}:{2}{3};", itemName, toolow, item.BCB_CHECKSUB.RESULT, bcd_itemEntity.UNIT);
                        }
                        else if (quantityResult == QuantityResult.UNCHECK)
                        {
                            sbcheckResult.AppendFormat("{0}:{1};", itemName, item.BCB_CHECKSUB.RESULT);
                        }
                        break;
                    case "00"://定性
                        bool isQualified = false;
                        List<BCD_ITEMQUALDESC> listBCD_ITEMQUALDESC = listALLBCD_ITEMQUALDESC.Where(u => u.ITEMGUID == item.BCB_CHECKSUB.ITEMGUID&&u.PASS=="1").ToList();
                        if (listBCD_ITEMQUALDESC.Count > 0)
                        {
                            foreach (var BCD_ITEMQUALDESC in listBCD_ITEMQUALDESC)
                            {
                                if (BCD_ITEMQUALDESC.DESCRIPTION == item.BCB_CHECKSUB.RESULT)
                                {
                                    //sbcheckResult.AppendFormat("{0}:{1}",itemName,item.BCB_CHECKSUB.RESULT);
                                    //break;
                                    isQualified = true;
                                    break;
                                }
                            }
                        }
                        if (!isQualified)
                        {
                            sbcheckResult.AppendFormat("{0}:{1};",itemName,item.BCB_CHECKSUB.RESULT);
                        }
                        break;
                    default:
                        break;
                }
            }

            return string.IsNullOrEmpty(sbcheckResult.ToString()) ? "未见明显异常" : sbcheckResult.ToString();
        }

        private static QuantityResult GetQuantityResult(object originalValue,double? maxValue,double? minValue)
        {
            double cellValueFloat = 0.0;
            if (originalValue == null) return QuantityResult.EMPTY;
            if (string.IsNullOrEmpty(originalValue.ToString())) return QuantityResult.EMPTY;
            if (originalValue.ToString() == "未检") return QuantityResult.UNCHECK;
            if (!double.TryParse(originalValue.ToString(), out cellValueFloat))
            {
                return QuantityResult.WRONGINPUT;
            }
            if (cellValueFloat > maxValue)
            {
                return QuantityResult.HIGH;
            }
            if (cellValueFloat < minValue)
            {
                return QuantityResult.LOW;
            }
            return QuantityResult.OK;
        }
    }
}
