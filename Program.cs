using System.Runtime.InteropServices;

namespace OrderBatching
{
    internal static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AllocConsole();//调用系统API，调用控制台窗口
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //for (int i = 0; i < 58; i++)
            //{
            //    for(int j = 0; j < 99; j++)
            //    {
            //        Console.WriteLine("x:" + i + "y:" + j);
            //    }
            //}
            InitialSystem.InitialNode();
            InitialSystem.InitialAisle();
            InitialSystem.InitialRack();
            InitialData.readCsv();
            Batching.OrderBatch();
            Routing.SshapeRouting();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
            FreeConsole();//释放控制台
        }
    }
}