using Terminal.Gui;
using TestFight_Gui.GUI;

namespace TestFight_Gui;

public class GuiHelper
{
    internal static List<string> GetAllMenu ()
    {
        List<string> menu = new List<string>();
        menu.Add("格式转换");
        menu.Add("嵌入字幕");
        menu.Add("媒体压缩");
        menu.Add("视频解析");
        return menu;
    }

    public FrameView SetWindow(string menu)
    {
        FrameView view = new FrameView();
        VideoConvertor vc = new VideoConvertor();
        EmbedSubtitle es = new EmbedSubtitle();
        MediaCompress mc = new MediaCompress();
        VideoDownloader vd = new VideoDownloader();
        if (menu == "格式转换")
        {
            view = vc.GetWindow();
        }
        
        if (menu == "嵌入字幕")
        {
            view = es.GetWindow();
        }
        if (menu == "媒体压缩")
        {
            view = mc.GetWindow();
        }
        if (menu == "视频解析")
        {
            view = vd.GetWindow();
        }

        return view;
    }
}