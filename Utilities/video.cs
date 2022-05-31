using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;

namespace VideoCov.Utilities
{
    public class Video
    {
        private void ffmpeg(string source, string output, string[] args)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = ConfigurationManager.AppSettings.Get("FFMPEG_PATH"); //运行程序
                p.StartInfo.Arguments = args[0].ToString() +" "+ source + " " + args[1].ToString()+ " " +output; 
                p.StartInfo.UseShellExecute = false; //不使用系统权限
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //隐藏窗体
                p.StartInfo.CreateNoWindow = true; //不显示新窗口
                p.Start(); //启动
                p.WaitForExit();
                p.Close();
            }
            catch (System.ComponentModel.Win32Exception exc)
            {
                Console.WriteLine("找不到ffmpeg的路径：" + exc);
                return;
            }
        }
        public static void mkv2mp4(string source,string output)
        {
            Console.WriteLine("[mkv转mp4] 已开始转码");
            DateTime beforDT = System.DateTime.Now;
                var v = new Video();
                string[] args = {"-i", "-c:v copy -c:a aac" };
                v.ffmpeg(source,output,args);
            DateTime afterDT = System.DateTime.Now;
            TimeSpan time = afterDT.Subtract(beforDT);
            Console.WriteLine("[mkv转mp4] 转码结束，用时{0}",time);
        }
    }
}
