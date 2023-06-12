using OrderBatching.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderBatching
{
    public class InitialData
    {
        public static void readCsv()
        {
            FileStream fs = new FileStream("../../../benchmarks/Order.csv", System.IO.FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
            string tempText = "";
            while ((tempText = sr.ReadLine()) != null)
            {
                string[] arr = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Order tempOrder = new Order();
                tempOrder.OrderID = arr[0];
                DetailOrder tempDetail = new DetailOrder();
                string[] OrderInfo = arr[1].Split("-");
                tempDetail.Content = arr[1];
                tempDetail.TargetBlock = Int32.Parse(OrderInfo[0]);
                tempDetail.TargetAsile = Int32.Parse(OrderInfo[1]);
                tempDetail.TargetLoc = Int32.Parse(OrderInfo[2]);
                tempDetail.TargetSide = OrderInfo[3];
                var targetRack = GlobalList.RacksMap.Where((p)=> p.BlockID == tempDetail.TargetBlock && p.AsileID == tempDetail.TargetAsile && p.PickerLoc == tempDetail.TargetLoc && p.Side == tempDetail.TargetSide);
                foreach (Rack curRack in targetRack) 
                {
                    tempDetail.targetNode = curRack.CurNode;
                    curRack.CurNode.CurDetOrder = tempDetail;
                    if (curRack.Side == "L")
                    {
                        tempDetail.PickNode = curRack.CurNode.right;
                    }
                    else
                    {
                        tempDetail.PickNode = curRack.CurNode.left;
                    }
                }
                if (!GlobalList.OrdersList.Exists((p)=>p.OrderID == tempOrder.OrderID))
                {
                    tempOrder.detailOrders = new List<DetailOrder>();
                    GlobalList.OrdersList.Add(tempOrder);
                    tempOrder.detailOrders.Add(tempDetail);
                    tempDetail.OrderID = tempOrder;
                }
                else
                {
                    var curOrder = GlobalList.OrdersList.Find((p)=>p.OrderID == tempOrder.OrderID);
                    curOrder.detailOrders.Add(tempDetail);
                    tempDetail.OrderID = curOrder;
                }
                //GlobalList.AisleList[tempDetail.TargetAsile - 1].UnhandledOrders.Add(tempDetail);
            }
            //关闭流
            sr.Close(); 
            fs.Close();
        }
        
    }
}
