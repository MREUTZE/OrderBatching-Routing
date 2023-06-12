using OrderBatching.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;
using static System.Net.Mime.MediaTypeNames;

namespace OrderBatching
{
    public class Routing
    {
        public static void SshapeRouting()
        {
            var getDepot = GlobalList.MapNodesMap.Where((p) => p.X == 0 & p.Y == 0);
            MapNode Depot = getDepot.FirstOrDefault();
            List<Order> curOrders = new List<Order>();

            //待接入batching

            foreach (List<Order> cur in GlobalList.BatchedOrders)
            {
                curOrders = cur;
                List<MapNode> subRoute = GenerateRoute(curOrders, Depot);

                GlobalList.RouteList.Add(subRoute);
            }

        }

        public static List<MapNode> GenerateRoute(List<Order> Orders,MapNode Depot)
        {
            //初始化该任务批下Aisle的未分配任务信息
            foreach (Order order in Orders)
            {
                foreach (DetailOrder tempDetail in order.detailOrders)
                {
                    GlobalList.AisleList[tempDetail.TargetAsile - 1].UnhandledOrders.Add(tempDetail);
                }
            }
            List<MapNode> subRoute = new List<MapNode>();
            //Step 1. Determine the leftmost pick aisle that contains at least one pick location
            //and determine the block farthest from the depot
            int AisleID = int.MaxValue;
            int BlockID = int.MinValue;
            foreach(Order temporder in Orders)
            {
                foreach(DetailOrder tempDetail in temporder.detailOrders)
                {
                    string curSide = tempDetail.TargetSide;
                    int curAisle = tempDetail.TargetAsile;
                    int curBlock = tempDetail.TargetBlock;
                    BlockID = Math.Max(BlockID, curBlock);
                    AisleID = Math.Min(AisleID, curAisle);
                }
            }
            Aisle lefrAisle = GlobalList.AisleList.Find((p)=>p.Id == AisleID);
            Block farBlock = GlobalList.BlockList.Find((p)=>p.Id == BlockID);
            subRoute.Add(Depot);
            // Step 2: Go from the depot to the front of the left pick aisle
            MapNode startNode = GlobalList.MapNodesMap.Where(n => n.X == lefrAisle.StartX).FirstOrDefault();
            subRoute.Add(startNode);
            // Step 3: Traverse the left pick aisle to the front cross aisle of the farthest block
            MapNode frontCrossAisle = GlobalList.MapNodesMap.Where(n => n.X == lefrAisle.StartX && n.Y == farBlock.StartY).FirstOrDefault();
            subRoute.Add(frontCrossAisle);
            UpdateOrderInfo(lefrAisle, startNode, frontCrossAisle);
            // Step 4: Traverse the front cross aisle and subaisles of the farthest block
            List<MapNode> subsubroute = GetSubAisleWithPick(frontCrossAisle);

            foreach (MapNode node in subsubroute)
            {
                subRoute.Add(node);
            }
            //测试用，需要加更加具体的判断（当前Block）
            bool hasOrdertoP = false;
            foreach (Aisle curAis in GlobalList.AisleList)
            {
                if(curAis.UnhandledOrders.Count > 0)
                {
                    hasOrdertoP = true;
                }
            }
            while(hasOrdertoP == true)
            //for(int i = 0; i < 3; i++)
            {
                MapNode curNode = subsubroute.Last();
                if(curNode != null)
                {
                    curNode = curNode.bottom;
                }
                if(curNode != null)
                {
                    subRoute.Add(curNode);
                    subsubroute.RemoveAll((p) => p != null);
                    subsubroute = GetSubAisleWithPick(curNode);
                }
                
                foreach (MapNode node in subsubroute)
                {
                    if(node != null)
                    {
                        subRoute.Add(node);
                    }
                   
                }
                hasOrdertoP = false;
                foreach (Aisle curAis in GlobalList.AisleList)
                {
                    if (curAis.UnhandledOrders.Count > 0)
                    {
                        hasOrdertoP = true;
                    }
                }
            }


            if(subRoute.Last().Y != 0)
            {
                int lastx = subRoute.Last().X;
                MapNode node = GlobalList.MapNodesMap.Where((p)=>p.X == lastx && p.Y ==0).FirstOrDefault();
                subRoute.Add(node);
            }
            subRoute.Add(Depot);
            return subRoute;
        }
        
        public static List<MapNode> GetSubAisleWithPick(MapNode curNode)
        {
            List<MapNode> subsubRoute = new List<MapNode>();
            Block curBlock = new Block();
            //确定当前巷道
            Aisle curAisle = GlobalList.AisleList.Find((p)=>p.StartX == curNode.X);
            //确定当前所处Block
            foreach(Block tempBlock in GlobalList.BlockList)
            {
                int StartY = tempBlock.StartY;
                int EndY = tempBlock.EndY;
                if(curNode.Y>=StartY && curNode.Y<=EndY)
                {
                    curBlock = tempBlock;
                }
            }
            List<Aisle> SuitAisle = new List<Aisle>();
            //找出符合条件的Aisle
            foreach(Aisle tempAisle in GlobalList.AisleList)
            {
                foreach(DetailOrder tempOrder in tempAisle.UnhandledOrders)
                {
                    int x = tempOrder.PickNode.X;
                    int y = tempOrder.PickNode.Y;
                    if(y <= curBlock.EndY && y >= curBlock.StartY)
                    {
                        if(!SuitAisle.Contains(tempAisle))
                        {
                            SuitAisle.Add(tempAisle);
                        }
                    }
                }
            }
            MapNode subAisleNode = new MapNode();
            // 需要修改 应该是leftmost和rightmost最近的而不是第一个
            int LeftX = int.MaxValue;
            int RightX = int.MinValue;
            int CurX = curNode.X;
            int CurY = curNode.Y;
            foreach (Aisle tempAisle in SuitAisle)
            {
                LeftX = Math.Min(LeftX, tempAisle.StartX); //leftmost的X
                RightX = Math.Max(RightX, tempAisle.StartX); //rightmost的X
            }
            //选取最近的Aisle
            if (Math.Abs(CurX - LeftX) < Math.Abs(CurX - RightX))
            {
                CurX = LeftX;
            }
            else
            {
                CurX = RightX;
            }
            int nextX = CurX;
            int nextY = curNode.Y;
            subAisleNode = GlobalList.MapNodesMap.Where((p) => p.X == nextX && p.Y == nextY).FirstOrDefault();
            subsubRoute.Add(subAisleNode);
            MapNode farNode = new MapNode();
            //Step 5 
            if(SuitAisle.Count() >0) 
            {
                SearchBlock(SuitAisle, curBlock, subsubRoute, subAisleNode, nextX); // 需要先确定search的block至少存在一个任务
            }
            else //当前block空，前往下一个block
            {
                int curBlockId = curBlock.Id;
                int nextBlockId = curBlockId - 1;
                if(nextBlockId > 0)
                {
                    Block nextBlock = new Block();
                    nextBlock = GlobalList.BlockList.Find((p)=> p.Id == nextBlockId);
                    int nextBlockSy = nextBlock.StartY;
                    int nextBlockEy = nextBlock.EndY;
                    if(curNode.Y != nextBlockSy)
                    {
                        var nextNode = GlobalList.MapNodesMap.Where(p => p.X == curNode.X && p.Y == nextBlockSy).FirstOrDefault();
                        //curNode.Y = nextBlockSy;
                        curNode = nextNode;
                    }
                    else
                    {
                        var nextNode = GlobalList.MapNodesMap.Where(p => p.X == curNode.X && p.Y == nextBlockEy).FirstOrDefault();
                        //curNode.Y = nextBlockEy;
                        curNode = nextNode;
                    }
                    return GetSubAisleWithPick(curNode);
                }
            }
            return subsubRoute;
        }

        public static void SearchBlock(List<Aisle> SuitAisle,Block curBlock, List<MapNode> subsubRoute, MapNode subAisleNode,int nextX)
        {
            MapNode farNode = new MapNode();
            //Step 5.2
            if (SuitAisle.Count() == 1)
            {
                Aisle OnlyAisle = SuitAisle.First();
                int maxY = int.MinValue;
                foreach (DetailOrder tempOrder in OnlyAisle.UnhandledOrders)
                {
                    int x = tempOrder.PickNode.X;
                    int y = tempOrder.PickNode.Y;
                    if (y >= curBlock.StartY && y <= curBlock.EndY)
                    {
                        maxY = Math.Max(maxY, y);
                    }
                }
                farNode = GlobalList.MapNodesMap.Where((p) => p.X == nextX && p.Y == maxY).FirstOrDefault();
                subsubRoute.Add(farNode);
                subsubRoute.Add(subAisleNode);
                UpdateOrderInfo(SuitAisle[0], subAisleNode, farNode);
                SuitAisle.RemoveAt(0);
            }
            //Step 5.1
            else if(SuitAisle.Count() >1)
            {
                int Y1 = curBlock.EndY;
                int Y2 = curBlock.StartY;
                Aisle curAis = GlobalList.AisleList.Find((p) => p.StartX == subAisleNode.X);
                if (Y1 == subAisleNode.Y)
                {
                    farNode = GlobalList.MapNodesMap.Where((p) => p.X == nextX && p.Y == Y2).FirstOrDefault();
                    UpdateOrderInfo(SuitAisle.Find((p) => p == curAis), farNode, subAisleNode);
                }
                else
                {
                    farNode = GlobalList.MapNodesMap.Where((p) => p.X == nextX && p.Y == Y1).FirstOrDefault();
                    UpdateOrderInfo(SuitAisle.Find((p) => p == curAis), subAisleNode, farNode);
                }
                subsubRoute.Add(farNode);
                SuitAisle.RemoveAll((p)=>p==curAis);
                //当合适的巷道大于1的时候就一直through历遍(选择最近的）
                //Step 6 考虑分离出来
                while (SuitAisle.Count > 1)
                {
                    int LeftX = int.MaxValue;
                    int RightX = int.MinValue;
                    int CurX = subsubRoute.Last().X;
                    int CurY = subsubRoute.Last().Y;
                    foreach (Aisle tempAisle in SuitAisle)
                    {
                        LeftX = Math.Min(LeftX, tempAisle.StartX);
                        RightX = Math.Max(RightX, tempAisle.StartX);
                    }
                    //选取最近的Aisle
                    if (Math.Abs(CurX - LeftX) < Math.Abs(CurX - RightX))
                    {
                        CurX = LeftX;
                    }
                    else
                    {
                        CurX = RightX;
                    }
                    Aisle curTargetAisle = GlobalList.AisleList.Find((p) => p.StartX == CurX);
                    MapNode curTNode = new MapNode();
                    //横移
                    curTNode = GlobalList.MapNodesMap.Where((p) => p.X == CurX && p.Y == CurY).First();
                    subsubRoute.Add(curTNode);
                    //记录竖直方向起点
                    MapNode UpNode1 = curTNode;
                    //确定终点
                    if (CurY >= curBlock.EndY)
                    {
                        CurY = curBlock.StartY;
                    }
                    else
                    {
                        CurY = curBlock.EndY;
                    }
                    curTNode = GlobalList.MapNodesMap.Where((p) => p.X == CurX && p.Y == CurY).FirstOrDefault();
                    subsubRoute.Add(curTNode);
                    //记录竖直方向终点
                    MapNode UpNode2 = curTNode;
                    //更新任务信息
                    if (UpNode1.Y < UpNode2.Y)
                    {
                        UpdateOrderInfo(curTargetAisle, UpNode1, UpNode2);
                    }
                    else
                    {
                        UpdateOrderInfo(curTargetAisle, UpNode2, UpNode1);
                    }
                    SuitAisle.Remove(curTargetAisle);
                }
                //Step7
                //此时只剩一个Aisle进行 Step7
                int LastCurX = subsubRoute.Last().X;
                int LastCurY = subsubRoute.Last().Y;
                Aisle OnlyAisle = SuitAisle.First();
                int aisX = OnlyAisle.StartX;
                LastCurX = aisX;
                MapNode LastCurNode = new MapNode();
                LastCurNode = GlobalList.MapNodesMap.Where((p) => p.X == LastCurX && p.Y == LastCurY).FirstOrDefault();
                subsubRoute.Add(LastCurNode);
                //前往当前区块起始横道
                int maxY = int.MinValue;
                //找最远到达点
                foreach (DetailOrder tempOrder in OnlyAisle.UnhandledOrders)
                {
                    int x = tempOrder.PickNode.X;
                    int y = tempOrder.PickNode.Y;
                    if (y >= curBlock.StartY && y <= curBlock.EndY)
                    {
                        maxY = Math.Max(maxY, y);
                    }
                }
                MapNode curfarNode = GlobalList.MapNodesMap.Where((p) => p.X == LastCurX && p.Y == maxY).FirstOrDefault();
                //添加最远到达点再返回
                subsubRoute.Add(curfarNode);
                //确认Block起始横道的Y
                int curBlockY = curBlock.StartY;
                MapNode curBlockStart = GlobalList.MapNodesMap.Where((p) => p.X == LastCurX && p.Y == curBlockY).FirstOrDefault();
                subsubRoute.Add(curBlockStart);
                //更新信息
                if (curfarNode.Y < curBlockStart.Y)
                {
                    UpdateOrderInfo(OnlyAisle, curfarNode, curBlockStart);
                }
                else
                {
                    UpdateOrderInfo(OnlyAisle, curBlockStart, curfarNode);
                }
                SuitAisle.RemoveAt(0);

            }
        }

        public static void UpdateOrderInfo(Aisle curAisle, MapNode StartNode, MapNode EndNode)
        {
            List<DetailOrder> DelOrder = new List<DetailOrder>();
            foreach(DetailOrder curOrder in curAisle.UnhandledOrders)
            {
                if(curOrder.PickNode.Y>= StartNode.Y && curOrder.PickNode.Y <= EndNode.Y)
                {
                    DelOrder.Add(curOrder);
                }
            }
            curAisle.UnhandledOrders.RemoveAll((p) => DelOrder.Contains(p));
        }
    }
}
