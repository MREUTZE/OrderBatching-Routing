using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBatching.Entity
{
    public class Order
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public String OrderID { get; set; }
        /// <summary>
        /// 订单具体内容
        /// </summary>
        public List<DetailOrder> detailOrders { get; set; }
    }
}
