using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBatching.Entity
{
    public class Aisle
    {
        /// <summary>
        /// 巷道ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 巷道起点X坐标
        /// </summary>
        public int StartX { get; set; }
        /// <summary>
        /// 巷道起点Y坐标
        /// </summary>
        public int StartY { get; set; }
        /// <summary>
        /// 巷道终点Y坐标
        /// </summary>
        public int EndY { get; set; }

        public List<DetailOrder> UnhandledOrders { get; set;}
        public List<Rack> Racks { get; set;}
    }
}
