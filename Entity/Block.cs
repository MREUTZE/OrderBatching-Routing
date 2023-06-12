using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBatching.Entity
{
    public class Block
    {
        /// <summary>
        /// BlockID
        /// </summary>
        public int Id { get; set; }
        public List<Rack> RackList { get; set; }
        /// <summary>
        /// Block起始Y
        /// </summary>
        public int StartY { get; set; } = int.MaxValue;
        /// <summary>
        /// Block终点Y
        /// </summary>
        public int EndY { get; set; } = int.MinValue;
    }
}
