using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBatching.Entity
{
    public class MapNode
    {
        /// <summary>
        /// NodeID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// X坐标
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// 是否被货架占用
        /// </summary>
        public bool IsRackNode { get; set; }
        /// <summary>
        /// 该节点上的货架
        /// </summary>
        public Rack CurRack { get; set; }

        public DetailOrder CurDetOrder { get; set; }

        #region 四周的节点
        /// <summary>
        /// 上方的节点
        /// </summary>
        public MapNode top { get; set; }
        /// <summary>
        /// 下方的节点
        /// </summary>
        public MapNode bottom { get; set; }
        /// <summary>
        /// 左边的节点
        /// </summary>
        public MapNode left { get; set; }
        /// <summary>
        /// 右边的节点
        /// </summary>
        public MapNode right { get; set; }
        #endregion
    }
}
