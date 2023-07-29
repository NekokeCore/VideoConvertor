using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using Downloader;
using Newtonsoft.Json;
using ShellProgressBar;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

namespace VideoConvertor.Utilities.Downloader;

public class Downloaders
{
    private static string DownloadListContentInput;
    private static List<DownloadItem> DownloadList;
    private static ProgressBar ConsoleProgress;
    private static ConcurrentDictionary<string, ChildProgressBar> ChildConsoleProgresses;
    private static ProgressBarOptions ChildOption;
    private static ProgressBarOptions ProcessBarOption;
    private static DownloadService CurrentDownloadService;
    private static DownloadConfiguration CurrentDownloadConfiguration;
    private static CancellationTokenSource CancelAllTokenSource;
    private static string RefererInput;
    private static string UserAgentInput;

    public async Task DownloadTask(string downloadlistcontent,string? referer=null, string? userAgent=null)
    {
        RefererInput = referer;
        UserAgentInput = userAgent;
        DownloadListContentInput=downloadlistcontent;
        try
        {
            await Task.Delay(1000);
            Console.Clear();
            Initial();
            new Task(KeyboardHandler).Start();
            await DownloadQueue(DownloadList, CancelAllTokenSource.Token).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.Clear();
            Console.Error.WriteLine(e);
            Debugger.Break();
        }
    }
    private static List<DownloadItem> GetDownloadItems()
    {
        List<DownloadItem> downloadList = JsonConvert.DeserializeObject<List<DownloadItem>>(DownloadListContentInput);
        return downloadList;
    }
    private static DownloadConfiguration GetDownloadConfiguration()
    {
        var cookie1 = new CookieContainer();
        //Cookie_1.Add(new Cookie("name","value"){Domain = "xxxx.com"}); //设置Cookie
        return new DownloadConfiguration {
            BufferBlockSize = 10240, // 通常，主机最大支持8000字节，默认值为8000。
            ChunkCount = 8, // 要下载的文件分片数量，默认值为1
            MaximumBytesPerSecond = 1024*1024*0, // 下载速度限制为 MAX MB/s，默认值为零或无限制
            MaxTryAgainOnFailover = int.MaxValue, // 失败的最大次数
            ParallelDownload = true, // 下载文件是否为并行的。默认值为false
            Timeout = 1000, // 每个 stream reader  的超时（毫秒），默认值是1000
            RequestConfiguration = // 定制请求头文件
            {
                Accept = "*/*",
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer =  cookie1,
                Headers = new WebHeaderCollection(), // 添加自定义头字符
                //eg. Headers = new WebHeaderCollection() {"name:value"},
                Referer = RefererInput,
                KeepAlive = true,
                ProtocolVersion = HttpVersion.Version11, // 设置HTTP版本为1.1
                UseDefaultCredentials = false,
                UserAgent = UserAgentInput //"Mozilla/5.0"
            }
        };
    }
    private static async Task DownloadQueue(IEnumerable<DownloadItem> downloadList, CancellationToken cancelToken)
    {
        foreach (DownloadItem downloadItem in downloadList)
        {
            if (cancelToken.IsCancellationRequested)
                return;

            // begin download from url
            await Downloader(downloadItem).ConfigureAwait(false);
        }
    }
    private static async Task<DownloadService> Downloader(DownloadItem downloadItem)
    {
        CurrentDownloadConfiguration = GetDownloadConfiguration();
        CurrentDownloadService = CreateDownloadService(CurrentDownloadConfiguration);
        if (string.IsNullOrWhiteSpace(downloadItem.FileName))
        {
            await CurrentDownloadService.DownloadFileTaskAsync(downloadItem.Url, new DirectoryInfo(downloadItem.FolderPath)).ConfigureAwait(false);
        }
        else
        {
            await CurrentDownloadService.DownloadFileTaskAsync(downloadItem.Url, downloadItem.FileName).ConfigureAwait(false);
        }
        
        return CurrentDownloadService;
    }
    private static void Initial()
    {
        CancelAllTokenSource = new CancellationTokenSource();
        ChildConsoleProgresses = new ConcurrentDictionary<string, ChildProgressBar>();
        DownloadList = GetDownloadItems();

        ProcessBarOption = new ProgressBarOptions {
            ForegroundColor = ConsoleColor.Green,
            ForegroundColorDone = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.DarkGray,
            BackgroundCharacter = '\u2593',
            EnableTaskBarProgress = true,
            ProgressBarOnBottom = false,
            ProgressCharacter = '#'
        };
        ChildOption = new ProgressBarOptions {
            ForegroundColor = ConsoleColor.Yellow,
            BackgroundColor = ConsoleColor.DarkGray,
            ProgressCharacter = '-',
            ProgressBarOnBottom = true
        };
    }
    private static void KeyboardHandler()
    {
        ConsoleKeyInfo cki;
        Console.CancelKeyPress += CancelAll;

        while (true)
        {
            cki = Console.ReadKey(true);
            if (CurrentDownloadConfiguration != null)
            {
                switch (cki.Key)
                {
                    case ConsoleKey.P:
                        CurrentDownloadService?.Pause();
                        Console.Beep();
                        break;
                    case ConsoleKey.R:
                        CurrentDownloadService?.Resume();
                        break;
                    case ConsoleKey.Escape:
                        CurrentDownloadService?.CancelAsync();
                        break;
                }
            }
        }
    }
    private static void CancelAll(object sender, ConsoleCancelEventArgs e)
    {
        CancelAllTokenSource.Cancel();
        CurrentDownloadService?.CancelAsync();
    }
    private static void WriteKeyboardGuidLines()
    {
        Console.Clear();
        Console.WriteLine("按ESC键停止当前文件下载");
        Console.WriteLine("按下P键暂停当前任务下载，按下R键继续当前任务下载");
        Console.WriteLine();
    }
    private static DownloadService CreateDownloadService(DownloadConfiguration config)
    {
        var downloadService = new DownloadService(config);

        // Provide `FileName` and `TotalBytesToReceive` at the start of each downloads
        downloadService.DownloadStarted += OnDownloadStarted;

        // Provide any information about chunker downloads, 
        // like progress percentage per chunk, speed, 
        // total received bytes and received bytes array to live streaming.
        downloadService.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;

        // Provide any information about download progress, 
        // like progress percentage of sum of chunks, total speed, 
        // average speed, total received bytes and received bytes array 
        // to live streaming.
        downloadService.DownloadProgressChanged += OnDownloadProgressChanged;

        // Download completed event that can include occurred errors or 
        // cancelled or download completed successfully.
        downloadService.DownloadFileCompleted += OnDownloadFileCompleted;

        return downloadService;
    }
    private static void OnDownloadStarted(object sender, DownloadStartedEventArgs e)
    {
        WriteKeyboardGuidLines();
        ConsoleProgress = new ProgressBar(10000, $"正在下载 {Path.GetFileName(e.FileName)}   ", ProcessBarOption);
    }
    private static void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
        ConsoleProgress?.Tick(10000);

        if (e.Cancelled)
        {
            ConsoleProgress.Message += " 已取消";
        }
        else if (e.Error != null)
        {
            if (ConsoleProgress is null)
            {
                Console.Error.WriteLine(e.Error.Message);
            }
            else
            {
                ConsoleProgress.Message += " 错误";
            }
            Debugger.Break();
        }
        else
        {
            ConsoleProgress.Message += " 完成";
            Console.Title = "100%";
        }

        foreach (var child in ChildConsoleProgresses.Values)
            child.Dispose();

        ChildConsoleProgresses.Clear();
        ConsoleProgress?.Dispose();
    }
    private static void OnChunkDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        ChildProgressBar progress = ChildConsoleProgresses.GetOrAdd(e.ProgressId,
            id => ConsoleProgress?.Spawn(10000, $"分段 {id}", ChildOption));
        progress.Tick((int)(e.ProgressPercentage * 100));
        var activeChunksCount = e.ActiveChunks; // Running chunks count
    }
    private static void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        ConsoleProgress.Tick((int)(e.ProgressPercentage * 100));
        if (sender is DownloadService ds)
            e.UpdateTitleInfo(ds.IsPaused);
    }
}