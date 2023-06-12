using OrderBatching.Entity;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace OrderBatching
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            int cout = 1;
            int cout2 = 1;
            foreach (List<Order> curorders in GlobalList.BatchedOrders)
            {

                StringBuilder sb = new StringBuilder();
                foreach (Order curorder in curorders)
                {
                    sb.Append(curorder.OrderID);
                    sb.Append(",");
                    cout2++;
                }
                Console.WriteLine("第" + cout2 + "批订单为:" + sb.ToString());
            }

            List<string> coorStr = new List<string>();
            foreach (List<MapNode> subRoute in GlobalList.RouteList)
            {
                //Queue<MapNode> queue = new Queue<MapNode>();
                Console.WriteLine("子路径" + cout + ":");
                cout++;
                StringBuilder sb = new StringBuilder();
                foreach (MapNode node in subRoute)
                {
                    sb.Append("(").Append(node.X).Append(",").Append(node.Y).Append(")").Append("|");
                }
                //GlobalList.RouteQueue.Add(queue);

                string res = sb.ToString();
                coorStr.Add(res);
                Console.WriteLine(res);
            }
            GenerateAGVRoute(coorStr);

            foreach (var originalQueue in GlobalList.RouteQueue)
            {
                Queue<Tuple<int, int>> deepCopyQueue = new Queue<Tuple<int, int>>();

                foreach (var originalTuple in originalQueue)
                {
                    Tuple<int, int> deepCopyTuple = new Tuple<int, int>(originalTuple.Item1, originalTuple.Item2);
                    deepCopyQueue.Enqueue(deepCopyTuple);
                }

                GlobalList.LocalRoute.Add(deepCopyQueue);
            }
            Thread agv1 = new Thread(UpdateAgv);
            Thread agv2 = new Thread(UpdateAgv);
            Thread agv3 = new Thread(UpdateAgv);
            Thread agv4 = new Thread(UpdateAgv);
            Thread agv5 = new Thread(UpdateAgv);
            agv1.Start(1);
            agv2.Start(2);
            agv3.Start(3);
            agv4.Start(4);
            agv5.Start(5);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        void Draw(Bitmap inimg)
        {
            Bitmap img = inimg;
            Graphics g = Graphics.FromImage(img);
            int w = 14;
            int move = 100;
            DrawMap(w, move, g);
            DrawRack(w, move, g);
            DrawRoute(w, move, g);
            g.Dispose();
        }
        private void DrawMap(int w, int move, Graphics g)
        {
            Pen pen = new Pen(Color.DarkMagenta, 2);
            foreach (MapNode curNode in GlobalList.MapNodesMap)
            {
                if (curNode.IsRackNode == false)
                {
                    int rectx = w * curNode.X + move;
                    int recty = w * curNode.Y + move;
                    int rectWidth = 1;
                    int rectHeight = 1;
                    g.DrawRectangle(pen, rectx, recty, rectWidth, rectHeight);
                }
            }
            pen.Dispose();
            //g.Dispose();
        }

        private void DrawRack(int w, int move, Graphics g)
        {
            Pen pen2 = new Pen(Color.MediumSeaGreen, 2);
            SolidBrush brush = new SolidBrush(Color.CornflowerBlue);
            foreach (Rack curRack in GlobalList.RacksMap)
            {

                int rectx = (int)(w * curRack.X - w * 0.5) + move;
                int recty = (int)(w * curRack.Y - w * 0.5) + move;
                int rectWidth = w * 1;
                int rectHeight = w * 2;
                g.DrawRectangle(pen2, rectx, recty, rectWidth, rectHeight);
                if (curRack.CurNode.CurDetOrder != null)
                {
                    g.FillRectangle(brush, rectx, recty, rectWidth, rectHeight);
                }
            }
            pen2.Dispose();
            //g.Dispose();
        }

        private void DrawRoute(int w, int move, Graphics g)
        {
            List<Color> colors = new List<Color>();
            Pen pen3 = new Pen(Color.DarkOrange, 3);
            colors.Add(Color.Orange);
            colors.Add(Color.Red);
            colors.Add(Color.Blue);
            colors.Add(Color.Purple);
            colors.Add(Color.Green);
            int sec = 0;
            

            foreach (Queue<Tuple<int, int>> que in GlobalList.LocalRoute)
            {
                int startX = 0 + move;
                int startY = 0 + move;
                int endX;
                int endY;
                int subSumDis = 0;
                pen3.Color = colors[sec];
                sec++;
                foreach (Tuple<int,int> coor in que)
                {
                    subSumDis += Math.Abs(coor.Item1 - startX-move) + Math.Abs(coor.Item2 - startY-move);
                    endX = w * coor.Item1 + move;
                    endY = w * coor.Item2 + move;
                    g.DrawLine(pen3, startX, startY, endX, endY);
                    startX = endX;
                    startY = endY;
                }
            }
            pen3.Dispose();
        }
        public static List<Tuple<int, int>> FillCoordinates(List<Tuple<int, int>> coordinates)
        {
            List<Tuple<int, int>> filledCoordinates = new List<Tuple<int, int>>();
            filledCoordinates.Add(coordinates[0]);

            for (int i = 0; i < coordinates.Count - 1; i++)
            {
                int x1 = coordinates[i].Item1;
                int y1 = coordinates[i].Item2;
                int x2 = coordinates[i + 1].Item1;
                int y2 = coordinates[i + 1].Item2;

                if (x1 == x2)  // 垂直移动
                {
                    int minY = Math.Min(y1, y2);
                    int maxY = Math.Max(y1, y2);
                    if (y1 <= y2)
                    {
                        for (int y = y1 + 1; y <= y2; y++)
                        {
                            filledCoordinates.Add(new Tuple<int, int>(x1, y));
                        }
                    }
                    else
                    {
                        for (int y = y1 - 1; y >= y2; y--)
                        {
                            filledCoordinates.Add(new Tuple<int, int>(x1, y));
                        }
                    }

                }
                else if (y1 == y2)  // 水平移动
                {
                    int minX = Math.Min(x1, x2);
                    int maxX = Math.Max(x1, x2);
                    if (x1 < x2)
                    {
                        for (int x = x1 + 1; x <= x2; x++)
                        {
                            filledCoordinates.Add(new Tuple<int, int>(x, y1));
                        }
                    }
                    else
                    {
                        for (int x = x1 - 1; x >= x2; x--)
                        {
                            filledCoordinates.Add(new Tuple<int, int>(x, y1));
                        }
                    }

                }
                else
                {
                    throw new ArgumentException("Invalid coordinates: " + string.Join("|", coordinates));
                }
            }

            return filledCoordinates;
        }
        public static List<Tuple<int, int>> ParseCoordinates(string coordinates)
        {
            List<Tuple<int, int>> coordinateList = new List<Tuple<int, int>>();

            // 移除多余的空格并按 "|" 分割字符串
            string[] coordinateArray = coordinates.Replace(" ", "").Split('|');

            foreach (var coordinate in coordinateArray)
            {
                // 提取坐标中的数字部分
                string[] values = coordinate.Trim('(', ')').Split(',');

                if (values.Length == 2 && double.TryParse(values[0], out double x) && double.TryParse(values[1], out double y))
                {
                    // 创建坐标元组并添加到列表中
                    coordinateList.Add(new Tuple<int, int>((int)x,(int) y));
                }
            }

            return coordinateList;
        }

        public static void GenerateAGVRoute(List<string> coorStr)
        {
            //测试仿真功能使用
            //string coordinates1 = "(0.0,0.0) | (6.0,0.0) | (6.0,73.0) | (18.0,73.0) | (18.0,81.5) | (18.0,73.0) | (21.0,73.0) | (24.0,73.0) | (24.0,100.0) | (27.0,100.0) | (30.0,100.0) | (30.0,95.5) | (30.0,100.0) | (33.0,100.0) | (36.0,100.0) | (39.0,100.0) | (39.0,95.5) | (39.0,100.0) | (42.0,100.0) | (45.0,100.0) | (48.0,100.0) | (48.0,73.0) | (51.0,73.0) | (51.0,70.5) | (51.0,73.0) | (48.0,73.0) | (48.0,46.0) | (45.0,46.0) | (42.0,46.0) | (42.0,48.5) | (42.0,46.0) | (39.0,46.0) | (36.0,46.0) | (33.0,46.0) | (33.0,73.0) | (30.0,73.0) | (27.0,73.0) | (24.0,73.0) | (21.0,73.0) | (18.0,73.0) | (15.0,73.0) | (12.0,73.0) | (9.0,73.0) | (9.0,46.0) | (9.0,46.0) | (9.0,39.5) | (9.0,46.0) | (12.0,46.0) | (15.0,46.0) | (18.0,46.0) | (21.0,46.0) | (24.0,46.0) | (27.0,46.0) | (30.0,46.0) | (33.0,46.0) | (36.0,46.0) | (36.0,43.5) | (36.0,46.0) | (39.0,46.0) | (42.0,46.0) | (42.0,23.0) | (48.0,23.0) | (48.0,0.0) | (45.0,0.0) | (42.0,0.0) | (39.0,0.0) | (36.0,0.0) | (33.0,0.0) | (30.0,0.0) | (27.0,0.0) | (24.0,0.0) | (21.0,0.0) | (18.0,0.0) | (15.0,0.0) | (12.0,0.0) | (9.0,0.0) | (9.0,4.5) | (9.0,0.0) | (0.0,0.0)";
            //string coordinates2 = "(0.0,0.0) | (0.0,0.0) | (0.0,73.0) | (18.0,73.0) | (18.0,95.5) | (18.0,73.0) | (18.0,73.0) | (18.0,46.0) | (27.0,46.0) | (27.0,39.5) | (27.0,46.0) | (30.0,46.0) | (33.0,46.0) | (36.0,46.0) | (36.0,31.5) | (36.0,46.0) | (39.0,46.0) | (39.0,43.5) | (39.0,46.0) | (42.0,46.0) | (45.0,46.0) | (45.0,23.0) | (42.0,23.0) | (42.0,10.5) | (42.0,23.0) | (39.0,23.0) | (36.0,23.0) | (33.0,23.0) | (30.0,23.0) | (27.0,23.0) | (24.0,23.0) | (21.0,23.0) | (18.0,23.0) | (15.0,23.0) | (15.0,0.0) | (0.0,0.0)";
            //string coordinates3 = "(0.0,0.0) | (3.0,0.0) | (3.0,73.0) | (3.0,73.0) | (3.0,95.5) | (3.0,73.0) | (12.0,73.0) | (12.0,46.0) | (6.0,46.0) | (6.0,23.0) | (9.0,23.0) | (12.0,23.0) | (12.0,29.5) | (12.0,23.0) | (15.0,23.0) | (18.0,23.0) | (18.0,27.5) | (18.0,23.0) | (21.0,23.0) | (24.0,23.0) | (24.0,29.5) | (24.0,23.0) | (27.0,23.0) | (27.0,25.5) | (27.0,23.0) | (33.0,23.0) | (33.0,12.5) | (33.0,23.0) | (30.0,23.0) | (30.0,12.5) | (30.0,23.0) | (27.0,23.0) | (27.0,18.5) | (27.0,23.0) | (24.0,23.0) | (21.0,23.0) | (18.0,23.0) | (15.0,23.0) | (12.0,23.0) | (12.0,20.5) | (12.0,23.0) | (9.0,23.0) | (6.0,23.0) | (6.0,0.0) | (0.0,0.0)";
            //string coordinates4 = "(0.0,0.0) | (0.0,0.0) | (0.0,73.0) | (0.0,73.0) | (0.0,83.5) | (0.0,73.0) | (3.0,73.0) | (3.0,64.5) | (3.0,73.0) | (6.0,73.0) | (6.0,46.0) | (9.0,46.0) | (12.0,46.0) | (15.0,46.0) | (18.0,46.0) | (18.0,56.5) | (18.0,46.0) | (21.0,46.0) | (24.0,46.0) | (24.0,48.5) | (24.0,46.0) | (27.0,46.0) | (30.0,46.0) | (30.0,64.5) | (30.0,46.0) | (33.0,46.0) | (36.0,46.0) | (36.0,52.5) | (36.0,46.0) | (39.0,46.0) | (42.0,46.0) | (45.0,46.0) | (45.0,48.5) | (45.0,46.0) | (48.0,46.0) | (48.0,39.5) | (48.0,46.0) | (45.0,46.0) | (42.0,46.0) | (39.0,46.0) | (36.0,46.0) | (33.0,46.0) | (33.0,23.0) | (39.0,23.0) | (39.0,0.0) | (36.0,0.0) | (33.0,0.0) | (30.0,0.0) | (27.0,0.0) | (24.0,0.0) | (21.0,0.0) | (21.0,2.5) | (21.0,0.0) | (18.0,0.0) | (18.0,2.5) | (18.0,0.0) | (15.0,0.0) | (12.0,0.0) | (9.0,0.0) | (6.0,0.0) | (6.0,4.5) | (6.0,0.0) | (0.0,0.0)";
            //string coordinates5 = "(0.0,0.0) | (15.0,0.0) | (15.0,73.0) | (15.0,73.0) | (15.0,89.5) | (15.0,73.0) | (18.0,73.0) | (21.0,73.0) | (21.0,81.5) | (21.0,73.0) | (24.0,73.0) | (27.0,73.0) | (27.0,83.5) | (27.0,73.0) | (30.0,73.0) | (33.0,73.0) | (33.0,83.5) | (33.0,73.0) | (36.0,73.0) | (36.0,83.5) | (36.0,73.0) | (39.0,73.0) | (42.0,73.0) | (45.0,73.0) | (45.0,77.5) | (45.0,73.0) | (39.0,73.0) | (39.0,46.0) | (36.0,46.0) | (33.0,46.0) | (33.0,54.5) | (33.0,46.0) | (30.0,46.0) | (30.0,48.5) | (30.0,46.0) | (27.0,46.0) | (24.0,46.0) | (24.0,48.5) | (24.0,46.0) | (21.0,46.0) | (21.0,23.0) | (39.0,23.0) | (39.0,20.5) | (39.0,23.0) | (36.0,23.0) | (33.0,23.0) | (33.0,0.0) | (30.0,0.0) | (27.0,0.0) | (27.0,12.5) | (27.0,0.0) | (0.0,0.0)";
            //List<string> coorStr = new List<string> { coordinates1, coordinates2, coordinates3, coordinates4, coordinates5 };
            foreach (string curStr in coorStr)
            {
                List<Tuple<int, int>> coordinateList = ParseCoordinates(curStr);
                coordinateList = FillCoordinates(coordinateList);
                Queue<Tuple<int, int>> RouteQueue = new Queue<Tuple<int, int>>();
                foreach (var coordinate in coordinateList)
                {
                    RouteQueue.Enqueue(coordinate);
                    //Console.WriteLine($"({coordinate.Item1},{coordinate.Item2})");
                }
                GlobalList.RouteQueue.Add(RouteQueue);
            }

        }

        void UpdateAgv(Object? obj)
        {
            while (true)
            {
                if (obj.Equals(1))
                {
                    if (GlobalList.RouteQueue[0].Count > 0)
                    {
                        var targetNode = GlobalList.RouteQueue[0].Dequeue();
                        int x = targetNode.Item1;
                        int y = targetNode.Item2;
                        this.Invoke(new Action(() =>
                        {
                            this.button1.BackColor = Color.Orange;
                            this.button1.Location = new Point(14 * x + 90, 14 * y + 90);
                        }));
                    }
                }
                else if (obj.Equals(2))
                {
                    if (GlobalList.RouteQueue[1].Count > 0)
                    {
                        var targetNode = GlobalList.RouteQueue[1].Dequeue();
                        int x = targetNode.Item1;
                        int y = targetNode.Item2;
                        this.Invoke(new Action(() =>
                        {
                            this.button2.BackColor = Color.Red;
                            this.button2.Location = new Point(14 * x + 90, 14 * y + 90);
                        }));
                    }
                }
                else if (obj.Equals(3))
                {
                    if (GlobalList.RouteQueue[2].Count > 0)
                    {
                        var targetNode = GlobalList.RouteQueue[2].Dequeue();
                        int x = targetNode.Item1;
                        int y = targetNode.Item2;
                        this.Invoke(new Action(() =>
                        {
                            this.button3.BackColor = Color.Blue;
                            this.button3.Location = new Point(14 * x + 90, 14 * y + 90);
                        }));
                    }
                }
                else if (obj.Equals(4))
                {
                    if (GlobalList.RouteQueue[3].Count > 0)
                    {
                        var targetNode = GlobalList.RouteQueue[3].Dequeue();
                        int x = targetNode.Item1;
                        int y = targetNode.Item2;
                        this.Invoke(new Action(() =>
                        {
                            this.button4.BackColor = Color.Purple;
                            this.button4.Location = new Point(14 * x + 90, 14 * y + 90);
                        }));
                    }
                }
                else if (obj.Equals(5))
                {
                    if (GlobalList.RouteQueue[4].Count > 0)
                    {
                        var targetNode = GlobalList.RouteQueue[4].Dequeue();
                        int x = targetNode.Item1;
                        int y = targetNode.Item2;
                        this.Invoke(new Action(() =>
                        {
                            this.button5.BackColor = Color.Green;
                            this.button5.Location = new Point(14 * x + 90, 14 * y + 90);
                        }));
                    }
                }
                Thread.Sleep(200);

            }
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Draw(img);
            pictureBox1.Image = img;
        }
    }
}