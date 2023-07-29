namespace TestFight_Bilibili;

public class VideoInfo
{
    public class VideoInformationData
    {
        public int cid { get; set; }
        public int page { get; set; }
        public string part { get; set; }
    }
    
    public class VideoInformation
    {
        public int code { get; set; }
        public List<VideoInformationData> data { get; set; }
    }
    
}