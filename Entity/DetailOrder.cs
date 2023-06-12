using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBatching.Entity
{
    public class DetailOrder
    {
        /// <summary>
        /// 隶属的OrderID
        /// </summary>
        public Order OrderID { get; set; }
        /// <summary>
        /// 详细指令内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 订单目标Block
        /// </summary>
        public int TargetBlock { get; set; }
        /// <summary>
        /// 订单目标巷道
        /// </summary>
        public int TargetAsile { get; set; }
        /// <summary>
        /// 订单目标位置（从下往上数）
        /// </summary>
        public int TargetLoc { get; set; }
        /// <summary>
        /// 订单目标边
        /// </summary>
        public string TargetSide { get; set; }
        /// <summary>
        /// 订单目标站点
        /// </summary>
        public MapNode targetNode { get; set; }
        /// <summary>
        /// 拣选点
        /// </summary>
        public MapNode PickNode { get; set; }
    }
}
