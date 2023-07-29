using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using VideoConvertor.Utilities.LogsHelper;

namespace VideoConvertor.Utilities;

public class VideoDownloader
{
    /// <summary>
    ///     Youtube-Dl调用
    /// </summary>
    /// <param name="url">下载地址</param>
    /// <param name="proxy_option">代理参数</param>
    /// <param name="proxy_url">代理地址</param>
    private void youtube_dl(string url, string proxy_option, string proxy_url)
    {
        try
        {
            var p = new Process();
            p.StartInfo.FileName = ConfigurationManager.AppSettings.Get("YOUTUBER_DL_PATH"); //运行程序
            p.StartInfo.Arguments = url;
            p.StartInfo.UseShellExecute = false; //不使用系统权限
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //隐藏窗体
            p.StartInfo.CreateNoWindow = true; //不显示新窗口
            p.Start(); //启动
            p.WaitForExit();
            p.Close();
        }
        catch (Win32Exception exc)
        {
            CommonLog.Log(3, "Youtube-DL", "找不到youtube-dl的路径：" + exc);
        }
    }

    /// <summary>
    ///     Youtube视频下载
    /// </summary>
    /// <param name="url">视频地址</param>
    /// <param name="proxy_option">代理类型</param>
    /// <param name="proxy_url">代理地址</param>
    public static void yt_download(string url, string proxy_option = null, string proxy_url = null)
    {
        var v = new VideoDownloader();
        CommonLog.Log(0, "视频解析", "正在下载Youtube视频，请稍后");
        var beforDT = DateTime.Now;
        v.youtube_dl(url, proxy_option, proxy_url);
        var afterDT = DateTime.Now;
        var time = afterDT.Subtract(beforDT);
        CommonLog.Log(0, "视频解析", "视频下载完成，用时" + Math.Round(time.TotalSeconds, 2) + "s");
    }
    
}