using Terminal.Gui;

namespace VideoConvertor.Utilities.GUI.Pages;

public class VideoDownloader
{
    public FrameView GetWindow()
    {
        FrameView RightPane = new FrameView ("视频解析") {
            X = 25,
            Y = 1, // for menu
            Width = Dim.Fill (),
            Height = Dim.Fill (),
            CanFocus = false,
            Shortcut = Key.CtrlMask | Key.R
        };
        RightPane.Title = $"{RightPane.Title} ({RightPane.ShortcutTag})";
        RightPane.ShortcutAction = () => RightPane.SetFocus ();
        Button button = new Button()
        {
            Text = "视频解析",
            Y = Pos.Center (),
            X = Pos.Center (),
            IsDefault = true,
        };
        button.Clicked += ()=>MessageBox.Query("视频解析","正在合并到本程序中，请座和放宽！","Ok");
        RightPane.Add (button);
        return RightPane;
    }
}