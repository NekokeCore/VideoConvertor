namespace TestFight_Bilibili;

public class dash_video
{
    public class Video_Url
    {
        public int id { get; set; }
        public string baseUrl { get; set; }
    }
    
    public class Audio_Url
    {
        public int id { get; set; }
        public string baseUrl { get; set; }
        public int bandwidth { get; set; }
    }

    public class Video_Dash
    {
        public List<Video_Url> video { get; set; }
        public List<Audio_Url> audio { get; set; }
    }
    
    public class Video_Data
    {
        public string[]? accept_description { get; set; }
        public int[]? accept_quality { get; set; }
        public Video_Dash dash { get; set; }
    }

    public class Video
    {
        public int code { get; set; }
        public Video_Data data { get; set; }
    }
    
    
}