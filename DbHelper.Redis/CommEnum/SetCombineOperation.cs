using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHelper.Redis.CommEnum
{

    /// <summary>
    /// Set集合之间的操作
    /// </summary>
    public enum SetCombineOperation
    {
        /// <summary>
        /// 取集合的并集
        /// </summary>
        Union = 0,
        
        /// <summary>
        /// 取集合的交集
        /// </summary>
        Intersect = 1,
        
        /// <summary>
        /// 取集合的差集
        /// </summary>
        Difference = 2
    }
}
