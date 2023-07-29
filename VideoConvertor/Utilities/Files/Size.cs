namespace VideoConvertor.Utilities.Files;

internal class Size
{
    //单位大小换算
    /// <summary>
    ///     单位大小换算
    /// </summary>
    /// <param name="data">原始数据大小Bits</param>
    /// <returns>文件大小</returns>
    public static string Getsize(long data)
    {
        string size;
        if (data == 0)
        {
            size = data + "Bit";
            return size;
        }

        if (data > 1024)
        {
            var progress = data / 1024;
            if (progress > 1024)
            {
                var progress2 = progress / 1024;
                if (progress2 > 1024)
                {
                    var progress3 = progress2 / 1024;
                    size = progress3 + "GB";
                }
                else
                {
                    size = progress2 + "MB";
                }
            }
            else
            {
                size = progress + "Byte";
            }
        }
        else
        {
            size = data + "Bit";
        }

        return size;
    }
}