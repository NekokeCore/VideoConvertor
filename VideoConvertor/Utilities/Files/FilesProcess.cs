namespace VideoConvertor.Utilities.Files;

public class FilesProcess
{
    //文件或目录复制
    /// <summary>
    ///     复制指定文件或目录到指定位置
    /// </summary>
    /// <param name="source">目标</param>
    /// <param name="output">输出目标</param>
    /// <returns>布尔值</returns>
    public static bool Copy(string source, string output)
    {
        var done = true;
        Directory.CreateDirectory(output);
        File.Copy(source, output, true);
        if (Directory.Exists(source))
        {
            var files = Directory.GetFiles(source);
            foreach (var s in files)
            {
                var fileName = Path.GetFileName(s);
                var destFile = Path.Combine(output, fileName);
                File.Copy(s, destFile, true);
            }
        }
        else
        {
            Console.WriteLine("[文件处理]输入目录错误或不存在");
            done = false;
        }

        return done;
    }

    //文件或目录移动
    /// <summary>
    ///     文件或目录移动
    /// </summary>
    /// <param name="source">路径目标</param>
    /// <param name="output">输出目标</param>
    /// <returns>布尔值</returns>
    public static bool Move(string source, string output)
    {
        var done = false;
        if (File.Exists(source) || Directory.Exists(source))
        {
            if (File.Exists(source)) File.Move(source, output);

            if (Directory.Exists(source))
            {
                if (Directory.Exists(output))
                {
                    Directory.Move(source, output);
                }
                else
                {
                    Directory.CreateDirectory(output);
                    Directory.Move(source, output);
                }
            }

            done = true;
        }
        else
        {
            Console.WriteLine("[文件复制]文件或目录错误或不存在");
        }

        return done;
    }

    //删除文件或目录
    /// <summary>
    ///     删除文件或目录
    /// </summary>
    /// <param name="source">路径目标</param>
    /// <returns>布尔值</returns>
    public static bool Files_Delete(string source)
    {
        var done = true;
        if (File.Exists(source) || Directory.Exists(source))
        {
            if (File.Exists(source))
            {
                var fi = new FileInfo(source);
                try
                {
                    fi.Delete();
                }
                catch (IOException e)
                {
                    Console.WriteLine("[文件删除]" + e.Message);
                }
            }

            if (Directory.Exists(source))
            {
                var di = new DirectoryInfo(source);
                try
                {
                    di.Delete(true);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        else
        {
            Console.WriteLine("[文件删除]文件或目录错误或不存在");
            done = false;
        }

        return done;
    }
}