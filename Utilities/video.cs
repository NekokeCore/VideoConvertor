using System.Diagnostics;

namespace VideoCov.Utilities
{
    public class Video
    {
        private void ffmpeg(string source, string output, string[] args)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = @"D:\TOOLS_PATH\ffmpeg-2022-05-26-git-0dcbe1c1aa-full_build\bin\ffmpeg.exe"; //运行程序
                p.StartInfo.Arguments = args[0].ToString() +" "+ source + " " + args[1].ToString()+ " " +output; 
                p.StartInfo.UseShellExecute = false; //不使用系统权限
                p.StartInfo.RedirectStandardOutput = false; //不输出信息
                p.StartInfo.RedirectStandardError = false; //不输出错误信息
                p.Start(); //启动
                p.WaitForExit();
                p.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public static void mkv2mp4(string source,string output)
        {
            var v = new Video();
            string[] args = {"-i", "-c:v copy -c:a aac" };
            v.ffmpeg(source,output,args);
        }
    }
}
