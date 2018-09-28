using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHelper.Redis.CommEnum
{
    /// <summary>
    /// 对元素排序的方向
    /// </summary>
    public enum LexicalOrder
    {
        /// <summary>
        /// 从小值到大值
        /// </summary>
        Ascending = 0,
        
        /// <summary>
        /// 从大值到小值
        /// </summary>
        Descending = 1
    }
}
