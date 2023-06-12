using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBatching.Entity
{
    public class GlobalList
    {
        public static List<Block> BlockList = new List<Block>();
        public static HashSet<MapNode> MapNodesMap = new HashSet<MapNode>();
        public static HashSet<Rack> RacksMap = new HashSet<Rack>();
        public static List<Order> OrdersList = new List<Order>();
        public static List<Aisle> AisleList = new List<Aisle>();
        public static List<List<MapNode>> RouteList = new List<List<MapNode>>();
        public static List<List<Order>> BatchedOrders = new List<List<Order>>();
        public static List<Queue<Tuple<int, int>>> RouteQueue = new List<Queue<Tuple<int, int>>>();
        //public static List<List<Tuple<int, int>>> DetailRoute = new List<List<Tuple<int, int>>>();

        public static List<Queue<Tuple<int, int>>> LocalRoute = new List<Queue<Tuple<int, int>>>();
    }
}
