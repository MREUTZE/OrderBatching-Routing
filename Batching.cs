using OrderBatching.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace OrderBatching
{
    internal class Batching
    {
        public static void OrderBatch()
        {
            //
            GlobalList.BatchedOrders = InitialBatching(GlobalList.OrdersList,5,10);
            //GenInitialBatch();
            var result = VnsAlg(GlobalList.BatchedOrders);
            GlobalList.BatchedOrders = result;
        }

        static List<List<Order>> VnsAlg(List<List<Order>> orderBatches)
        {
            int kMax = 10000; // 最大邻域搜索次数

            // 复制原始数据结构
            List<List<Order>> bestSolution = CopyOrderBatches(orderBatches);
            double bestDistance = GetTotalDistance(bestSolution);

            int k = 1; // 邻域搜索步长

            while (k <= kMax)
            {
                Console.WriteLine("VNS第" + k + "次迭代");
                bool improved = true;

                while (improved)
                {
                    improved = false;
                    //简单的交换策略验证用
                    for (int i = 0; i < orderBatches.Count; i++)
                    {
                        List<List<Order>> newSolution = CopyOrderBatches(bestSolution);

                        // 对当前子列表进行邻域搜索（或者说这里作为扰动）
                        newSolution = Exchange(newSolution);
                        double newDistance = GetTotalDistance(newSolution);
                        //实际是VND
                        //TODO：实现对邻域结构的进一步VND

                        if (newDistance < bestDistance)
                        {
                            bestSolution = newSolution;
                            bestDistance = newDistance;
                            improved = true;
                        }
                    }
                }
                Console.WriteLine("当前最佳总距离" +bestDistance);
                k++;
            }

            return bestSolution;
        }

        private static List<Order> LocalSearch(List<Order> batch, int k)
        {
            // 根据具体需求实现本地搜索算子，例如2-opt、relocate、exchange等
            // 这里只是示例代码，需要根据实际情况进行自定义

            // 例如，使用2-opt算子进行邻域搜索
            bool improved = true;

            while (improved)
            {
                improved = false;

                for (int i = 1; i < batch.Count - 1; i++)
                {
                    for (int j = i + k; j < batch.Count; j++)
                    {
                        List<Order> newBatch = TwoOptSwap(batch, i, j);
                        double newDistance = GetDistance(newBatch);

                        if (newDistance < GetDistance(batch))
                        {
                            batch = newBatch;
                            improved = true;
                        }
                    }
                }
            }

            return batch;
        }

        private static List<Order> TwoOptSwap(List<Order> batch, int i, int j)
        {
            List<Order> newBatch = new List<Order>();

            // 交换i到j之间的订单顺序
            for (int x = 0; x < i; x++)
            {
                newBatch.Add(batch[x]);
            }

            for (int x = j; x >= i; x--)
            {
                newBatch.Add(batch[x]);
            }

            for (int x = j + 1; x < batch.Count; x++)
            {
                newBatch.Add(batch[x]);
            }

            return newBatch;
        }

        public static List<List<Order>> Exchange(List<List<Order>> orders)
        {
            Random random = new Random();

            // 选择要交换的两个位置
            int firstIndex = random.Next(0, orders.Count);
            int secondIndex = random.Next(0, orders.Count);

            // 选择要交换的具体订单
            List<Order> firstList = orders[firstIndex];
            List<Order> secondList = orders[secondIndex];
            int firstOrderIndex = random.Next(0, firstList.Count);
            int secondOrderIndex = random.Next(0, secondList.Count);

            // 执行订单交换
            Order firstOrder = firstList[firstOrderIndex];
            Order secondOrder = secondList[secondOrderIndex];
            firstList[firstOrderIndex] = secondOrder;
            secondList[secondOrderIndex] = firstOrder;

            return orders;
        }

        private static double GetDistance(List<Order> orders)
        {
            // 实现根据订单的属性计算距离的逻辑
            // 这里只是示例代码，需要根据实际情况进行自定义
            double tempdis = 0;
            var getDepot = GlobalList.MapNodesMap.Where((p) => p.X == 0 & p.Y == 0);
            MapNode Depot = getDepot.FirstOrDefault();
            List<MapNode> subRoute = Routing.GenerateRoute(orders, Depot);
            int subSumDis = 0;
            MapNode lastNode = Depot;
            foreach (MapNode curNode in subRoute)
            {
                subSumDis += Math.Abs(curNode.X - lastNode.X) + Math.Abs(curNode.Y - lastNode.Y);
                lastNode = curNode;
            }
            return subSumDis;
        }

        private static double GetTotalDistance(List<List<Order>> orderBatches)
        {
            double totalDistance = 0;
            foreach(List<Order> batch in orderBatches)
            {
                double tempdis = 0;
                var getDepot = GlobalList.MapNodesMap.Where((p) => p.X == 0 & p.Y == 0);
                MapNode Depot = getDepot.FirstOrDefault();
                List<MapNode> subRoute = Routing.GenerateRoute(batch, Depot);
                int subSumDis = 0;
                MapNode lastNode = Depot;
                foreach (MapNode curNode in subRoute)
                {
                    subSumDis += Math.Abs(curNode.X - lastNode.X) + Math.Abs(curNode.Y - lastNode.Y);
                    lastNode = curNode;
                }
                totalDistance += subSumDis;
            }

            return totalDistance;
        }

        private static List<List<Order>> CopyOrderBatches(List<List<Order>> orderBatches)
        {
            List<List<Order>> copiedBatches = new List<List<Order>>();

            for (int i = 0; i < orderBatches.Count; i++)
            {
                List<Order> batch = orderBatches[i];
                List<Order> copiedBatch = new List<Order>(batch);

                copiedBatches.Add(copiedBatch);
            }

            return copiedBatches;
        }

        static List<List<Order>> InitialBatching(List<Order> orders, int numberOfBatches, int batchSize)
        {
            // 随机打乱订单列表
            Random random = new Random();
            List<Order> shuffledOrders = orders.OrderBy(x => random.Next()).ToList();

            List<List<Order>> batches = new List<List<Order>>();

            // 将订单随机分成指定数量的组，并且每组包含指定数量的订单
            for (int i = 0; i < numberOfBatches; i++)
            {
                List<Order> batch = shuffledOrders.Skip(i * batchSize).Take(batchSize).ToList();
                batches.Add(batch);
            }

            return batches;
        }
    }
}
