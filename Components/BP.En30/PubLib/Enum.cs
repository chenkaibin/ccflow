using System;
using System.Collections.Generic;
using System.Text;

namespace BP.En
{
    /// <summary>
    /// 图表类型
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// 柱状图
        /// </summary>
        Histogram,
        /// <summary>
        /// 丙状图
        /// </summary>
        Pie,
        /// <summary>
        /// 折线图
        /// </summary>
        Line
    }
    /// <summary>
    /// 分组方式
    /// </summary>
    public enum GroupWay
    {
        /// <summary>
        /// 求合
        /// </summary>
        BySum,
        /// <summary>
        /// 求平均
        /// </summary>
        ByAvg
    }
    /// <summary>
    /// 排序方式
    /// </summary>
    public enum OrderWay
    {
        /// <summary>
        /// 升序
        /// </summary>
        OrderByUp,
        /// <summary>
        /// 降序
        /// </summary>
        OrderByDown
    }
}
