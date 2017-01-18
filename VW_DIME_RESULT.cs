using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCopy
{
    public class VW_DIME_RESULT
    {
        public string CHECKCODE { get; set; }
        public string ITEMEID { get; set; }
        public string LIS_ITEMNO { get; set; }
        public string ITEMNAME { get; set; }
        public string RESULT { get; set; }
        public string UNIT { get; set; }
        public string VALSTANDARD { get; set; }
        public string DOCTORNAME { get; set; }
        public DateTime DATECHECK { get; set; }
        public DateTime RECHECKDATE { get; set; }
        public string RECHECKDOCTORNAME { get; set; }
        public string RESULTTAG { get; set; }
        public string ExternalCode2 { get; set; }
        public string ExternalCOde3 { get; set; }
        public int cp_uncheckflag { get; set; }
    }
}
