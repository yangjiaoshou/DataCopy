using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCopy.ViewModels
{
    /// <summary>
    /// 细项对照信息
    /// </summary>
    public class ItemContrastInfo
    {
        /// <summary>
        /// 外部ID
        /// </summary>
        public string IDExternal { get; set; }
        /// <summary>
        /// 外部名称
        /// </summary>
        public string NameExternal { get; set; }
        /// <summary>
        /// 内部ID
        /// </summary>
        public string IDInternal { get; set; }
        /// <summary>
        /// 内部名称
        /// </summary>
        public string NameInternal { get; set; }
    }
}
