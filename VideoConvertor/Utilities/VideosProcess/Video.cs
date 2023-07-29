using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VideoConvertor.Utilities.Files;
using VideoConvertor.Utilities.Logs;

namespace VideoConvertor.Utilities.VideosProcess;

public class Video
{
    //ffmpeg进程处理
    /// <summary>
    ///     ffmpeg进程输入
    /// </summary>
    /// <param name="source">源文件</param>
    /// <param name="output">输出文件</param>
    /// <param name="args">参数</param>
    private void ffmpeg(string source, string output, string[] args)
    {
        try
        {
            var p = new Process();
            p.StartInfo.FileName = ConfigurationManager.AppSettings.Get("FFMPEG_PATH"); //运行程序
            p.StartInfo.Arguments = args[0] + " " + source + " " + args[1] + " " + output;
            p.StartInfo.UseShellExecute = false; //不使用系统权限
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //隐藏窗体
            p.StartInfo.CreateNoWindow = true; //不显示新窗口
            p.Start(); //启动
            p.WaitForExit();
            p.Close();
        }
        catch (Win32Exception exc)
        {
            CommonLog.Log(3, "FFMPEG", "找不到ffmpeg的路径：" + exc);
        }
    }

    //万能格式转换
    /// <summary>
    ///     万能格式转换
    /// </summary>
    /// <param name="v_format">视频编码器</param>
    /// <param name="a_format">音频编码器</param>
    /// <param name="source">源文件</param>
    /// <param name="output">输出文件</param>
    public static void Video_format(string v_format, string a_format, string source, string output)
    {
        CommonLog.Log(0, "格式转换", "已开始转码");
        var beforDT = DateTime.Now;
        var v = new Video();
        string[] args = { "-i", "-c:v " + v_format + " -c:a " + a_format };
        v.ffmpeg("\"" + source + "\"", "\"" + output + "\"", args);
        var afterDT = DateTime.Now;
        var time = afterDT.Subtract(beforDT);
        CommonLog.Log(0, "格式转换", "转码结束，用时" + Math.Round(time.TotalSeconds, 2) + "s");
    }

    //视频压缩
    /// <summary>
    ///     视频压缩
    /// </summary>
    /// <param name="source">源文件</param>
    /// <param name="output">输出文件</param>
    /// <param name="args">参数</param>
    public static void compress(string source, string output, string args)
    {
        Console.WriteLine("[视频压缩]已开始视频压缩");
        if (args == null) Console.WriteLine("[视频压缩]未提交参数，将以内置参数压制视频");
        var beforDT = DateTime.Now;
        var befor_size = Size.Getsize(Checker.GetFileLength(source));
        var v = new Video();
        var Video_Cos = true;
        switch (args)
        {
            case "极限":
                string[] ffmpeg_args_jx = { "-i", "-c:a aac -crf 51" };
                v.ffmpeg(source, output, ffmpeg_args_jx);
                Video_Cos = false;
                break;
            case "标清":
                string[] ffmpeg_args_bq = { "-i", "-c:a aac -crf 35" };
                v.ffmpeg(source, output, ffmpeg_args_bq);
                Video_Cos = false;
                break;
            case "高清":
                string[] ffmpeg_args_gq = { "-i", "-c:a aac -crf 28" };
                v.ffmpeg(source, output, ffmpeg_args_gq);
                Video_Cos = false;
                break;
            case "超清":
                string[] ffmpeg_args_cq = { "-i", "-c:a aac -crf 20" };
                v.ffmpeg(source, output, ffmpeg_args_cq);
                Video_Cos = false;
                break;
            case null:
            case "无损":
                string[] ffmpeg_args_ws = { "-i", "-c:a aac -crf 18" };
                v.ffmpeg(source, output, ffmpeg_args_ws);
                Video_Cos = false;
                break;
        }

        if (Video_Cos)
            if (args != null)
            {
                string[] ffmpeg_args_cos = { "-i", "-c:a aac -crf " + args };
                v.ffmpeg(source, output, ffmpeg_args_cos);
            }

        var after_size = Size.Getsize(Checker.GetFileLength(output));
        var ratio = Math.Round(
            (double)(Checker.GetFileLength(source) - Checker.GetFileLength(output)) / Checker.GetFileLength(source) *
            100, 2) + "%";
        var afterDT = DateTime.Now;
        var time = afterDT.Subtract(beforDT);
        Console.WriteLine("[压缩视频] 压缩完毕，用时{0}s,压缩前文件大小{1}，压缩后文件大小{2},压缩比率{3}", Math.Round(time.TotalSeconds, 2),
            befor_size, after_size, ratio);
    }

    //内嵌字幕
    /// <summary>
    ///     内嵌字幕
    /// </summary>
    /// <param name="source">源文件</param>
    /// <param name="output">输出文件</param>
    /// <param name="subtitle">字幕文件</param>
    public static void subtitle(string source, string output, string subtitle)
    {
        Console.WriteLine("[嵌入字幕]已开始嵌入字幕");
        var beforDT = DateTime.Now;
        var v = new Video();
        string subtitle_processed_1, subtitle_processed_2;
        subtitle_processed_1 = Regex.Replace(subtitle, @"\\", "\\\\");
        subtitle_processed_2 = Regex.Replace(subtitle_processed_1, @":", "\\:");
        string[] args =
            { "-i", "-c:v h264 -c:a copy -vf subtitles=" + "\"" + " '" + subtitle_processed_2 + " '" + "\"" + " -y" };
        v.ffmpeg(source, output, args);
        var afterDT = DateTime.Now;
        var time = afterDT.Subtract(beforDT);
        Console.WriteLine("[嵌入字幕] 嵌入完毕，用时{0}s", Math.Round(time.TotalSeconds, 2));
    }
}