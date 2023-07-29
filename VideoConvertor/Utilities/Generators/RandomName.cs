using System.Text;

namespace VideoConvertor.Utilities.Generators;

public class RandomName
{
    public static string GetRandomString(int iLength)
    {
        var buffer = "0123456789abcdefghijklmnopqistuvwxyzABCDEFGHIJKLMNOPQISTUVWXYZ@#$%^&()"; // 随机字符中也可以为汉字（任何）
        var sb = new StringBuilder();
        var r = new Random();
        var range = buffer.Length;
        for (var i = 0; i < iLength; i++) sb.Append(buffer.Substring(r.Next(range), 1));
        return sb.ToString();
    }
}