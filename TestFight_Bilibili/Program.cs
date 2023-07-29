using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


namespace TestFight_Bilibili;



internal class Program
{
    private static List<DownloadItemByName> DownloadListByName;
    private static List<DownloadItemByPath> DownloadListByPath;
    private static string ListJson;
    private string HttpGetData(string url, string cookies, Dictionary<string, string> args)
    {
        var result = "";
        var builder = new StringBuilder();
        builder.Append(url);
        if (args.Count > 0)
        {
            builder.Append("?");
            var i = 0;
            foreach (var item in args)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
        }

        var myRequest = (HttpWebRequest)WebRequest.Create(builder.ToString());
        myRequest.Headers.Add(HttpRequestHeader.Cookie, cookies);
        myRequest.Method = "GET";
        var myResponse = (HttpWebResponse)myRequest.GetResponse();
        var stream = myResponse.GetResponseStream();
        try
        {
            //获取内容
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
        }
        finally
        {
            stream.Close();
        }

        return result;
    }
    private static string getBiliVideo_JSON(string bv,string cid)
    {
        var get = new Program();
        var myDictionary = new Dictionary<string, string>();
        myDictionary.Add("bvid", bv);
        myDictionary.Add("cid", cid);
        myDictionary.Add("qn", "0");
        myDictionary.Add("fnval", "80");
        myDictionary.Add("fnver", "0");
        myDictionary.Add("fourk", "1");
        var sessdata =
            "SESSDATA=";
        var getvideo = get.HttpGetData("https://api.bilibili.com/x/player/playurl", sessdata, myDictionary);
        return getvideo;
    }
    private static string getBiliVideo_Cid_Json(string bv)
    {
        var get = new Program();
        var myDictionary = new Dictionary<string, string>();
        myDictionary.Add("bvid", bv);
        var sessdata =
            "SESSDATA=";
        var getcid = get.HttpGetData("https://api.bilibili.com/x/player/pagelist", sessdata, myDictionary);
        return getcid;
    }
    private static string getBiliVideo_Cid(string bv)
    {
        string cid = "";
        VideoInfo.VideoInformation? bilivinfo =
            JsonConvert.DeserializeObject<VideoInfo.VideoInformation>(getBiliVideo_Cid_Json(bv));
        if (bilivinfo.code == 0)
        {
            Console.WriteLine("请选择分P");
            for (int i = 0; i < bilivinfo.data.Count; i++)
            {
                Console.WriteLine((bilivinfo.data[i].page-1)+"."+bilivinfo.data[i].part);
            }
            var menu_0 = int.TryParse(Console.ReadLine(), out var menu);
            cid = bilivinfo.data[menu].cid.ToString();
        }
        else
        {
            Console.WriteLine("获取视频信息失败");
        }
        return cid;
    }

    private static string DownloadListSerialize(string DownloadType)
    {
        if (DownloadType == "byName")
        {
            ListJson = JsonConvert.SerializeObject(DownloadListByName);
        }
        if (DownloadType == "byPath")
        {
            ListJson = JsonConvert.SerializeObject(DownloadListByPath);
        }
        
        return ListJson;
    }
    
    private static Boolean biliVideo(string url,string? filename=null,string? dir=null)
    {
        string bv;
        bv = Regex.Match(url, @"(?<=com/video/)(.*)(?=/)").ToString();
        dash_video.Video? biliJson= JsonConvert.DeserializeObject<dash_video.Video>(getBiliVideo_JSON(bv,getBiliVideo_Cid(bv)));
        if (biliJson.code == 0)
        {
            //清晰度索引
            var video_index = new Dictionary<int, string>();
            for (int i = 0;i<biliJson.data.accept_description.Length; i++)
            {
                video_index.Add(i,biliJson.data.accept_description[i]);
            }
            //清晰度ID索引
            var video = new Dictionary<string, int>();
            for (int i = 0; i < biliJson.data.accept_description.Length; i++)
            {
                video.Add(biliJson.data.accept_description[i],biliJson.data.accept_quality[i]);
            }
            //视频URL索引
            var video_url = new Dictionary<int, string>();
            for (int i = 0; i < biliJson.data.dash.video.Count; i++)
            {
                if (video_url.ContainsKey(biliJson.data.dash.video[i].id))
                {
                    //Console.WriteLine("已包含值，正在忽略");
                }
                else
                {
                    video_url.Add(biliJson.data.dash.video[i].id,biliJson.data.dash.video[i].baseUrl);
                    //Console.WriteLine(biliJson.data.dash.video[i].id);
                }
            }
            Console.WriteLine("请选择视频清晰度");
            Console.WriteLine("*----------------------------------*");
            for (int i = 0; i < biliJson.data.accept_description.Length; i++)
            {
                Console.WriteLine(i+"."+video_index[i]);
            }
            Console.WriteLine("*----------------------------------*");
            var menu = Console.ReadLine();
            int menu_1;
            var menu_2 = int.TryParse(menu, out menu_1);
            if (menu_2)
            {
                if (video_index.ContainsKey(menu_1))
                {
                    //downloader down = new downloader();
                    //down.Downloader(@video_url[video[video_index[menu_1]]], "https://www.bilibili.com",@".\test1.mp4","","Mozilla/5.0");
                    //必须先对List进行赋值
                    if (filename != null)
                    {
                        List<DownloadItemByName> dlbn = new List<DownloadItemByName>();
                        dlbn.Add(new DownloadItemByName(){FileName = filename,Url = @video_url[video[video_index[menu_1]]]});
                        DownloadListByName = dlbn;
                        return true;
                    }
                    if (dir != null)
                    {
                        List<DownloadItemByPath> dlbf = new List<DownloadItemByPath>();
                        dlbf.Add(new DownloadItemByPath(){FolderPath = dir,Url = @video_url[video[video_index[menu_1]]]});
                        DownloadListByPath = dlbf;
                        return true;
                    }
                }
                Console.WriteLine("错误的清晰度");
                return false;
            }
            Console.WriteLine("非数字输入！");
            return false;
        }
        Console.WriteLine("获取视频流失败！");
        return false;
    }
    private static Boolean biliAudio(string url,string? filename=null,string? dir=null)
    {
        string bv;
        bv = Regex.Match(url, @"(?<=com/video/)(.*)(?=/)").ToString();
        dash_video.Video? biliJson= JsonConvert.DeserializeObject<dash_video.Video>(getBiliVideo_JSON(bv,getBiliVideo_Cid(bv)));
        if (biliJson.code == 0)
        {
                    //音频索引
        var audio_Index = new Dictionary<int, int>();
        for (int i = 0; i < biliJson.data.dash.audio.Count; i++)
        {
            if (audio_Index.ContainsValue(biliJson.data.dash.audio[i].bandwidth))
            {
                //Console.WriteLine("已包含值，正在忽略");
            }
            else
            { 
                audio_Index.Add(i,biliJson.data.dash.audio[i].bandwidth);
                //Console.WriteLine($"第{i}次，键控为{i},内容为{biliJson.data.dash.audio[i].bandwidth}");
            }
        }
        //音频ID索引
        var audio = new Dictionary<int, int>();
        for (int i = 0; i < biliJson.data.dash.audio.Count; i++)
        {
            if (audio.ContainsKey(biliJson.data.dash.audio[i].bandwidth))
            {
                //Console.WriteLine("已包含值，正在忽略");
            }
            else
            { 
                audio.Add(biliJson.data.dash.audio[i].bandwidth,biliJson.data.dash.audio[i].id);
                //Console.WriteLine($"第{i}次，键控为{biliJson.data.dash.audio[i].bandwidth},内容为{biliJson.data.dash.audio[i].id}");
            }
        }
        //音频URL索引
        var audio_url = new Dictionary<int, string>();
        for (int i = 0; i < audio_Index.Count; i++)
        {
            audio_url.Add(audio[audio_Index[i]],biliJson.data.dash.audio[i].baseUrl);
            //Console.WriteLine($"第{i}次，键控为{biliJson.data.dash.audio[i].id},内容为{biliJson.data.dash.audio[i].baseUrl}");
        }
        Console.WriteLine("请选择音频质量");
        Console.WriteLine("*----------------------------------*");
        for (int i = 0; i < audio_Index.Count; i++)
        {
            Console.WriteLine(i+"."+audio_Index[i]);
        }
        Console.WriteLine("*----------------------------------*");
        var audio_menu = Console.ReadLine();
        int audio_menu_1;
        var audio_menu_2 = int.TryParse(audio_menu, out audio_menu_1);
        if (audio_menu_2)
        {
            if (audio_Index.ContainsKey(audio_menu_1))
            {
                //downloader down = new downloader();
                //down.Downloader(@audio_url[audio[audio_Index[audio_menu_1]]], "https://www.bilibili.com",@".\test1.m4s","","Mozilla/5.0");
                if (filename != null)
                {
                    DownloadListByName.Add(new DownloadItemByName(){FileName = filename,Url = @audio_url[audio[audio_Index[audio_menu_1]]]});
                    return true;
                }
                if (dir != null)
                {
                    DownloadListByPath.Add(new DownloadItemByPath(){FolderPath = dir,Url = @audio_url[audio[audio_Index[audio_menu_1]]]});
                    return true;
                }
            }
            Console.WriteLine("错误的清晰度！");
            return false;
        }
        Console.WriteLine("非数字输入！");
        return false;
        }
        Console.WriteLine("获取视频流失败！");
        return false;
    }
    
    private static void Main(string[] args)
    {
        Console.WriteLine("请输入B站视频链接");
        string url = Console.ReadLine();
        biliVideo(url,@".\test1.mp4");
        Console.WriteLine("是否下载音频？【回车确认】");
        Console.ReadKey();
        biliAudio(url,@".\test1.m4s");
        Console.WriteLine("是否开始下载？【回车确认】");
        Console.ReadKey();
        downloader down = new downloader();
        //Console.WriteLine(DownloadListSerialize("byName"));
        down.DownloadTask(DownloadListSerialize("byName"), "https://www.bilibili.com","Mozilla/5.0");
        
        
        //Console.WriteLine("是否合并视频和音频？【回车确认】");
        Console.ReadKey();
        //Console.WriteLine(Regex.Match("https://www.bilibili.com/video/BV15W4y19751/", @"(?<=com/video/)(.*)(?=/)").ToString());
    }
}
