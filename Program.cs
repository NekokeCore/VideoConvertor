using System.Resources;

#pragma warning disable CS8600 // 禁用将 null 字面量或可能为 null 的值转换为非 null 类型。
namespace VideoCov // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Boolean debug = false;
            ResourceManager resManager = new ResourceManager(typeof(Resource1));
            Console.WriteLine(resManager.GetString("banner"));
            Console.WriteLine(resManager.GetString("info"));
            Console.WriteLine(resManager.GetString("auther"));
            if (args.Length == 0) {
                Console.WriteLine("[信息] {0}",resManager.GetString("argsifempty"));
            }
            else
            {
                if (args[0].Equals("调试") || args[0].Equals("--v"))
                {
                    debug = true;
                };

                if (args[0].Equals("帮助") || args[0].Equals("-h"))
                {
                    Console.WriteLine(resManager.GetString("help"));
                };

                if (args[0].Equals("mkv转mp4") || args[0].Equals("-mkv2mp4"))
                {
                    Utilities.Video.mkv2mp4(args[1].ToString(), args[2].ToString());
                };
            } ;
            if (debug)
            {
                Console.WriteLine(string.Format("[DEBUG]接收到了{0}个小可爱", args.Length));//Debug 便于调试，显示接受了几个参数
                foreach (var item in args)//Debug 便于显示输入了什么参数
                {
                    Console.WriteLine(item);
                }
            };
        }
    }
}