using OrderBatching.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrderBatching
{
    public class InitialSystem
    {
        public static int BlockNum = 4;
        public static int curLeftX = 0;
        public static int curRightX = 17*2+18;
        public static int curLeftY = 0;
        public static int curRightY = 0;
        public static int rackLen = GlobalInfo.rackLen;
        public static void InitialNode()
        {
            //初始化Block
            for (int i = 0; i < BlockNum; i++)
            {
                Block tempBlock = new Block();
                tempBlock.Id = i+1;
                GlobalList.BlockList.Add(tempBlock);
            }
            int coutID = 0;
            for(int i = 0; i <= 51;i++)
            {
                for(int j = 0; j <99 ; j++)
                {
                    MapNode tempNode = new MapNode();
                    tempNode.Id = coutID;
                    coutID++;
                    int curX = i;
                    int curY = j;
                    tempNode.X = curX;
                    tempNode.Y = curY;
                    tempNode.IsRackNode = false;
                    GlobalList.MapNodesMap.Add(tempNode);

                }

            }
            //更新Node上下左右关系信息
            foreach (MapNode curNode in GlobalList.MapNodesMap)
            {
                int curX = curNode.X;
                int curY = curNode.Y;
                int lx = curX - 1;
                int ly = curY;
                int tx = curX;
                int ty = curY + 1;
                int rx = curX + 1;
                int ry = curY;
                int bx = curX;
                int by = curY - 1;
                var lNode = GlobalList.MapNodesMap.Where((p) => p.X == lx && p.Y == ly);
                var rNode = GlobalList.MapNodesMap.Where((p) => p.X == rx && p.Y == ry);
                var tNode = GlobalList.MapNodesMap.Where((p) => p.X == tx && p.Y == ty);
                var bNode = GlobalList.MapNodesMap.Where((p) => p.X == bx && p.Y == by);
                foreach(MapNode temp in lNode)
                {
                    curNode.left = temp;
                }
                foreach (MapNode temp in rNode)
                {
                    curNode.right = temp;
                }
                foreach (MapNode temp in tNode)
                {
                    curNode.top = temp;
                }
                foreach (MapNode temp in bNode)
                {
                    curNode.bottom = temp;
                }

            }

        }

        public static void InitialRack()
        {
            int rackID = 0;
            int leftX = 0;
            int leftY = 0;
            int tempYmax = 0;
            //生成block1和block2
            for(int k = 0; k<2;k++)
            {
                for (int j = 0; j < 18; j++)
                {
                    if (j < 17)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Rack tempRack = new Rack();
                            tempRack.RackID = rackID;
                            rackID++;
                            tempRack.PickerLoc = i + 1;
                            tempRack.BlockID = k+1;
                            tempRack.X = leftX + 1 + 3 * j;
                            tempRack.Y = leftY + 1 + i * rackLen;
                            tempRack.AsileID = j + 1;
                            GlobalList.AisleList[j].Racks.Add(tempRack);
                            tempRack.Side = "R";
                            GlobalList.RacksMap.Add(tempRack);

                            var tempNode1 = GlobalList.MapNodesMap.Where((p) => p.X == tempRack.X && p.Y == tempRack.Y);
                            var tempNode2 = GlobalList.MapNodesMap.Where((p) => p.X == tempRack.X && p.Y == tempRack.Y+1);
                            foreach (MapNode temp in tempNode1)
                            {
                                temp.IsRackNode = true;
                                temp.CurRack = tempRack;
                                tempRack.CurNode = temp;
                            }
                            foreach (MapNode temp in tempNode2)
                            {
                                temp.IsRackNode = true;
                                temp.CurRack = tempRack;
                            }

                            tempYmax = Math.Max(tempYmax, tempRack.Y);
                        }
                    }

                    if (j > 0)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Rack tempRack = new Rack();
                            tempRack.RackID = rackID;
                            rackID++;
                            tempRack.PickerLoc = i + 1;
                            tempRack.BlockID = k+1;
                            tempRack.X = leftX - 1 + 3 * j;
                            tempRack.Y = leftY + 1 + i * rackLen;
                            tempRack.AsileID = j + 1;
                            GlobalList.AisleList[j].Racks.Add(tempRack);
                            tempRack.Side = "L";
                            GlobalList.RacksMap.Add(tempRack);

                            var tempNode1 = GlobalList.MapNodesMap.Where((p) => p.X == tempRack.X && p.Y == tempRack.Y);
                            var tempNode2 = GlobalList.MapNodesMap.Where((p) => p.X == tempRack.X && p.Y == tempRack.Y + 1);
                            foreach (MapNode temp in tempNode1)
                            {
                                temp.IsRackNode = true;
                                temp.CurRack = tempRack;
                                tempRack.CurNode = temp;
                            }
                            foreach (MapNode temp in tempNode2)
                            {
                                temp.IsRackNode = true;
                                temp.CurRack = tempRack;
                            }
                        }
                    }

                }

                leftY = tempYmax + 4;
            }
            //生成block3和block4
            for (int k = 0; k < 2; k++)
            {
                for (int j = 0; j < 18; j++)
                {
                    if (j < 17)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            Rack tempRack = new Rack();
                            tempRack.RackID = rackID;
                            rackID++;
                            tempRack.PickerLoc = i + 1;
                            tempRack.BlockID = k + 3;
                            tempRack.X = leftX + 1 + 3 * j;
                            tempRack.Y = leftY + 1 + i * rackLen;
                            tempRack.AsileID = j + 1;
                            GlobalList.AisleList[j].Racks.Add(tempRack);
                            tempRack.Side = "R";
                            GlobalList.RacksMap.Add(tempRack);
                            tempYmax = Math.Max(tempYmax, tempRack.Y);

                            var tempNode1 = GlobalList.MapNodesMap.Where((p) => p.X == tempRack.X && p.Y == tempRack.Y);
                            var tempNode2 = GlobalList.MapNodesMap.Where((p) => p.X == tempRack.X && p.Y == tempRack.Y + 1);
                            foreach (MapNode temp in tempNode1)
                            {
                                temp.IsRackNode = true;
                                temp.CurRack = tempRack;
                                tempRack.CurNode = temp;
                            }
                            foreach (MapNode temp in tempNode2)
                            {
                                temp.IsRackNode = true;
                                temp.CurRack = tempRack;
                            }
                        }
                    }

                    if (j > 0)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            Rack tempRack = new Rack();
                            tempRack.RackID = rackID;
                            rackID++;
                            tempRack.PickerLoc = i + 1;
                            tempRack.BlockID = k + 3;
                            tempRack.X = leftX - 1 + 3 * j;
                            tempRack.Y = leftY + 1 + i * rackLen;
                            tempRack.AsileID = j + 1;
                            GlobalList.AisleList[j].Racks.Add(tempRack);
                            tempRack.Side = "L";
                            GlobalList.RacksMap.Add(tempRack);

                            var tempNode1 = GlobalList.MapNodesMap.Where((p) => p.X == tempRack.X && p.Y == tempRack.Y);
                            var tempNode2 = GlobalList.MapNodesMap.Where((p) => p.X == tempRack.X && p.Y == tempRack.Y + 1);
                            foreach (MapNode temp in tempNode1)
                            {
                                temp.IsRackNode = true;
                                temp.CurRack = tempRack;
                                tempRack.CurNode = temp;
                            }
                            foreach (MapNode temp in tempNode2)
                            {
                                temp.IsRackNode = true;
                                temp.CurRack = tempRack;
                            }
                        }
                    }

                }

                leftY = tempYmax + 4;
            }
            //Block添加Racklist
            foreach (Block tempBlock in GlobalList.BlockList)
            {
                tempBlock.RackList = new List<Rack>();
                int BlockID = tempBlock.Id;
                var tempRacks = GlobalList.RacksMap.Where((p) => p.BlockID == BlockID);
                foreach (Rack temp in tempRacks)
                {
                    tempBlock.RackList.Add(temp);
                    tempBlock.StartY = Math.Min(tempBlock.StartY, temp.Y);
                    tempBlock.EndY = Math.Max(tempBlock.EndY, temp.Y);
                }
                if(tempBlock.Id != 4)
                {
                    tempBlock.EndY += 3;
                }
                else
                {
                    tempBlock.EndY += 2;
                }
                tempBlock.StartY -= 1;
            }
        }

        public static void InitialAisle()
        {
            for(int i = 0; i < 18; i++)
            {
                Aisle tempAisle = new Aisle();
                tempAisle.UnhandledOrders = new List<DetailOrder>();
                tempAisle.Racks = new List<Rack>();
                tempAisle.Id = i+1;
                tempAisle.StartX = 0 + 3 * i;
                tempAisle.StartY = 0;
                tempAisle.EndY = 3 * 3 + 2 + 10 * 2 * 2 + 12 * 2 * 2;
                GlobalList.AisleList.Add(tempAisle);
            }
        }
    }
}
