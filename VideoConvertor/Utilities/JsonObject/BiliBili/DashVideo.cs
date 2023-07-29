namespace VideoConvertor.Utilities.JsonObject.BiliBili;

public class DashVideo
{
    public class VideoUrl
    {
        public int id { get; set; }
        public string baseUrl { get; set; }
    }
    
    public class AudioUrl
    {
        public int id { get; set; }
        public string baseUrl { get; set; }
    }

    public class VideoDash
    {
        public List<VideoUrl> video { get; set; }
        public List<AudioUrl> audio { get; set; }
    }
    
    public class VideoData
    {
        public string[]? accept_description { get; set; }
        public int[]? accept_quality { get; set; }
        public VideoDash dash { get; set; }
    }

    public class Video
    {
        public int code { get; set; }
        public VideoData data { get; set; }
    }
    
    
}