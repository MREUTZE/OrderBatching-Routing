using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBatching.Entity
{
    public class Rack
    {
        /// <summary>
        /// 货架横坐标
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// 货架纵坐标
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// 货架所处BlockID
        /// </summary>
        public int BlockID { get; set; }
        /// <summary>
        /// 货架自身ID
        /// </summary>
        public int RackID { get; set;}
        /// <summary>
        /// 货架所处巷道ID
        /// </summary>
        public int AsileID { get; set; }
        /// <summary>
        /// 货架处于巷道left或right
        /// </summary>
        public String Side { get; set; }
        /// <summary>
        /// 从下往上数第x个位置
        /// </summary>
        public int PickerLoc { get; set; }
        /// <summary>
        /// 所处Node(记录下面的Node)
        /// </summary>
        public MapNode CurNode { get; set; }
    }
}
