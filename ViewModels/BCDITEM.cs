using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCopy.ViewModels
{
    public class BCDITEM
    {
        public BCD_ITEM BCD_ITEM { get; set; }
        public List<BCD_ITEMQUALDESC> listBCD_ITEMQUALDESC;
        public List<BCD_ITEMQUALDESC> ListBCD_ITEMQUALDESC
        {
            get {
                if (listBCD_ITEMQUALDESC == null)
                {
                    listBCD_ITEMQUALDESC = new List<BCD_ITEMQUALDESC>();
                }
                return listBCD_ITEMQUALDESC;
            }
            set
            {
                listBCD_ITEMQUALDESC = value;
            }
        }
    }
}
