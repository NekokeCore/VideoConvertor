using System.Diagnostics;

namespace VideoCov.Utilities
{
    public class video
    {
        private void cmd(string source, string output)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe"; //运行程序
            p.StartInfo.UseShellExecute = false; //不使用系统权限
            p.StartInfo.RedirectStandardOutput = true; //输出信息
            p.StartInfo.RedirectStandardError = true; //输出信息
            p.StartInfo.CreateNoWindow = true; //不显示程序窗口
            p.Start(); //启动

            p.StandardInput.WriteLine("echo {0}",source + "&exit");
            p.StandardInput.AutoFlush = true;

            p.WaitForExit();
            p.Close();

            Console.WriteLine(p.StandardOutput.ReadToEnd());
            Console.WriteLine(source);
            Console.WriteLine(output);
        }
        public static void mkv2mp4(string source,string output)
        {
            var v = new video();
            v.cmd(source, output);
        }
    }
}
