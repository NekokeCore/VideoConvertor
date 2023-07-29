using System.Runtime.InteropServices;

//导入dll

namespace VideoConvertor.Utilities.Files;

#pragma warning disable CS8602

internal class Checker
{
    //调用windows API获取磁盘空闲空间
    //导入库
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetDiskFreeSpace([MarshalAs(UnmanagedType.LPTStr)] string rootPathName,
        ref int sectorsPerCluster, ref int bytesPerSector, ref int numberOfFreeClusters, ref int totalNumbeOfClusters);

    //获取指定目录下所有文件的大小
    /// <summary>
    ///     获取指定目录本身大小
    /// </summary>
    /// <param name="dirPath">路径</param>
    /// <returns>字节</returns>
    public static long GetDirectoryLength(string? dirPath)
    {
        //判断给定的路径是否存在,如果不存在则退出
        if (!Directory.Exists(dirPath))
            return 0;
        long len = 0;

        //定义一个DirectoryInfo对象
        var di = new DirectoryInfo(dirPath);

        //通过GetFiles方法,获取di目录中的所有文件的大小
        foreach (var fi in di.GetFiles()) len += fi.Length;

        //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
        var dis = di.GetDirectories();
        if (dis.Length > 0)
            for (var i = 0; i < dis.Length; i++)
                len += GetDirectoryLength(dis[i].FullName);
        return len;
    }

    //获取指定文件的大小
    /// <summary>
    ///     获取指定文件本身大小
    /// </summary>
    /// <param name="filePath">路径</param>
    /// <returns>字节</returns>
    public static long GetFileLength(string? filePath)
    {
        //判断当前路径所指向的是否为文件
        if (File.Exists(filePath))
        {
            //定义一个FileInfo对象,使之与filePath所指向的文件向关联,
            //以获取其大小
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        return -1;
    }

    //根据给定的目录或文件名获取其大小（自动识别）
    /// <summary>
    ///     自动识别目录或文件大小
    /// </summary>
    /// <param name="filePath">路径</param>
    /// <returns>字节</returns>
    public static long GetDirectOrFileSize(string? filePath)
    {
        //判断当前路径所指向的是否为文件
        if (Directory.Exists(filePath)) return GetDirectoryLength(filePath);
        if (File.Exists(filePath)) return GetFileLength(filePath);
        return -1;
    }

    //自动识别目录或文件占用空间大小
    /// <summary>
    ///     自动识别目录或文件占用空间大小
    /// </summary>
    /// <param name="filePath">目标</param>
    /// <returns>字节</returns>
    public static long GetDirectOrFileSpace(string? filePath)
    {
        if (Directory.Exists(filePath)) return GetDirectorySpace(filePath);
        if (File.Exists(filePath)) return GetDirectorySpace(filePath);
        return -1;
    }

    //得到磁盘信息
    /// <summary>
    ///     获取磁盘信息
    /// </summary>
    /// <param name="rootPathName">盘符</param>
    /// <returns>数据</returns>
    private static DiskInfo GetDiskInfo(string rootPathName)
    {
        var diskInfo = new DiskInfo();
        int sectorsPerCluster = 0, bytesPerSector = 0, numberOfFreeClusters = 0, totalNumberOfClusters = 0;
        GetDiskFreeSpace(rootPathName, ref sectorsPerCluster, ref bytesPerSector, ref numberOfFreeClusters,
            ref totalNumberOfClusters);

        //每簇的扇区数
        diskInfo.SectorsPerCluster = sectorsPerCluster;
        //每扇区字节
        diskInfo.BytesPerSector = bytesPerSector;
        diskInfo.NumberOfFreeClusters = numberOfFreeClusters;
        diskInfo.TotalNumberOfClusters = totalNumberOfClusters;
        diskInfo.RootPathName = rootPathName;

        return diskInfo;
    }

    //文件占用空间计算
    /// <summary>
    ///     获取每簇的字节
    /// </summary>
    /// <param name="dir">路径</param>
    /// <returns>字节</returns>
    private static long GetClusterSize(DirectoryInfo dir)
    {
        long clusterSize;
        DiskInfo diskInfo;
        diskInfo = GetDiskInfo(dir.Root.FullName);
        clusterSize = diskInfo.BytesPerSector * diskInfo.SectorsPerCluster;
        return clusterSize;
    }

    //所给路径中所对应的文件占用空间
    /// <summary>
    ///     文件占用空间大小
    /// </summary>
    /// <param name="filePath">目录</param>
    /// <returns>字节</returns>
    public static long GetFileSpace(string? filePath)
    {
        long temp = 0;
        //定义一个FileInfo对象，是指与filePath所指向的文件相关联，以获取其大小
        if (filePath != null)
        {
            var fileInfo = new FileInfo(filePath);
            var clusterSize = GetClusterSize(fileInfo);
            if (fileInfo.Length % clusterSize != 0)
            {
                decimal res = fileInfo.Length / clusterSize;
                var clu = Convert.ToInt32(Math.Ceiling(res)) + 1;
                temp = clusterSize * clu; //每簇的字节数 * 簇数
            }
            else
            {
                return fileInfo.Length;
            }
        }

        return temp;
    }

    //计算文件的占用空间
    /// <summary>
    ///     获取每簇的字节
    /// </summary>
    /// <param name="file">指定文件</param>
    /// <returns>簇大小</returns>
    private static long GetClusterSize(FileInfo file)
    {
        long clusterSize;
        var diskInfo = new DiskInfo();
        diskInfo = GetDiskInfo(file.Directory.Root.FullName);
        clusterSize = diskInfo.BytesPerSector * diskInfo.SectorsPerCluster;
        return clusterSize;
    }

    /// <summary>
    ///     获取目录的占用空间大小
    /// </summary>
    /// <param name="dirPath">路径</param>
    /// <returns>字节</returns>
    public static long GetDirectorySpace(string? dirPath)
    {
        //返回值
        long len = 0;
        //判断该路径是否存在（是否为文件夹）
        if (!Directory.Exists(dirPath))
        {
            //如果是文件，则调用
            len = GetFileSpace(dirPath);
        }
        else
        {
            //定义一个DirectoryInfo对象
            var di = new DirectoryInfo(dirPath);
            //本机的簇值
            var clusterSize = GetClusterSize(di);
            //遍历目录下的文件，获取总占用空间
            foreach (var fi in di.GetFiles())
                //文件大小除以簇，余若不为0
                if (fi.Length % clusterSize != 0)
                {
                    decimal res = fi.Length / clusterSize;
                    //文件大小除以簇，取整数加1。为该文件占用簇的值
                    var clu = Convert.ToInt32(Math.Ceiling(res)) + 1;
                    var result = clusterSize * clu;
                    len += result;
                }
                else
                {
                    //余若为0，则占用空间等于文件大小
                    len += fi.Length;
                }

            //获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
            var dis = di.GetDirectories();
            if (dis.Length > 0)
                for (var i = 0; i < dis.Length; i++)
                    len += GetDirectorySpace(dis[i].FullName);
        }

        return len;
    }

    /// <summary>
    ///     结构。硬盘信息
    /// </summary>
    private struct DiskInfo
    {
        public string RootPathName;

        //每簇的扇区数
        public int SectorsPerCluster;

        //每扇区字节
        public int BytesPerSector;
        public int NumberOfFreeClusters;
        public int TotalNumberOfClusters;
    }
}