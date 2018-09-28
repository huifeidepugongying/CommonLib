using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHelper.Redis.CommEnum
{
    /// <summary>
    /// 区间范围类型
    /// </summary>
    public enum RangeType
    {
        /// <summary>
        /// 闭区间，开始值与结束值都包含
        /// </summary>
        None = 0,

        /// <summary>
        /// 不包含开始值，包含结束值
        /// </summary>
        Start = 1,

        /// <summary>
        /// 不包含结束值，包含开始值
        /// </summary>
        Stop = 2,

        /// <summary>
        /// 开区间，开始值与结束值都不包含
        /// </summary>
        Both = 3
    }
}
