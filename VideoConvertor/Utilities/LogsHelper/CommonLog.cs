using System.Configuration;
using System.Globalization;

namespace VideoConvertor.Utilities.LogsHelper;

public class CommonLog
{
    /// <summary>
    ///     日志记载
    /// </summary>
    /// <param name="tag">标签</param>
    /// <param name="app">应用名称</param>
    /// <param name="message">日志内容</param>
    public static async Task Log(int tag, string app, string message)
    {
        string[] tags = { "[INFO]", "[WARRING]", "[ERROR]", "[PANIC]" };
        var logs = tags[tag] + "[" + app + "]" + message;
        if (tags[tag] == "[INFO]") info(logs);

        if (tags[tag] == "[WARRING]") warring(logs);

        if (tags[tag] == "[ERROR]") error(logs);

        if (tags[tag] == "[PANIC]") panic(logs);
        await Log_File(logs);
    }

    /// <summary>
    ///     日志归档
    /// </summary>
    /// <param name="log">日志内容</param>
    private static async Task Log_File(string log)
    {
        var Time = DateTime.Now;
        var logs = "[" + Time.ToLocalTime().ToString(CultureInfo.CurrentCulture) + "]" + log;
        if (File.Exists(ConfigurationManager.AppSettings.Get("LOG")))
        {
            using StreamWriter file =
                new(ConfigurationManager.AppSettings.Get("LOG") ?? throw new InvalidOperationException(), true);
            await file.WriteLineAsync(logs);
        }
        else
        {
            await File.WriteAllTextAsync(
                ConfigurationManager.AppSettings.Get("LOG") ?? throw new InvalidOperationException(), logs + '\n');
        }
    }

    private static void info(string log)
    {
        Console.ResetColor();
        Console.WriteLine(log);
    }

    private static void warring(string log)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(log);
        Console.ResetColor();
    }

    private static void error(string log)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(log);
        Console.ResetColor();
    }

    private static void panic(string log)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(log);
        Console.ResetColor();
    }
}