using System.Diagnostics;
using System.Runtime.InteropServices;
using Terminal.Gui;
using VideoConvertor.Utilities.GUI;

namespace VideoConvertor; // Note: actual namespace depends on the project name.

internal class Program
{
    private static int _cachedCategoryIndex;
    private static List<string> _categories;
    private static int Egg=0;

    private static void Main(string[] args)
    {
        _categories = GuiHelper.GetAllMenu();
        Application.Init ();
        Application.Run<MainUI> ();
        Application.Shutdown ();
    }
    class MainUI : Toplevel
    {
        private FrameView LeftPane;
        private ListView CategoryListView;
        private FrameView RightPane;

        public MainUI ()
        {
            ColorScheme = Colors.Base;
            MenuBar = new MenuBar (new MenuBarItem [] {
                new MenuBarItem("_VideoConvertor","",() => EasterEgg()),
                new MenuBarItem ("_帮助",new MenuItem [] {
                    new MenuItem ("_关于", "", () => MessageBox.Query ("关于", "sdaaaa", "_Ok"), null, null, Key.CtrlMask | Key.A),
                }),
            });
            
            LeftPane = new FrameView ("菜单") {
                X = 0,
                Y = 1, // for menu
                Width = 25,
                Height = Dim.Fill (),
                CanFocus = true,
                Shortcut = Key.CtrlMask | Key.M
            };
            LeftPane.Title = $"{LeftPane.Title} ({LeftPane.ShortcutTag})";
            LeftPane.ShortcutAction = () => LeftPane.SetFocus ();
            CategoryListView = new ListView (_categories) {
                X = 0,
                Y = 0,
                Width = Dim.Fill (),
                Height = Dim.Fill (),
                AllowsMarking = false,
                CanFocus = true,
            };
            CategoryListView.OpenSelectedItem += (a) => {
                RightPane.SetFocus ();
            };
            CategoryListView.SelectedItemChanged += CategoryListView_SelectedChanged;
            LeftPane.Add (CategoryListView);
            
            Add (MenuBar);
            Add (LeftPane);

            // Restore previous selections
            CategoryListView.SelectedItem = _cachedCategoryIndex;
            
        }
        
        void CategoryListView_SelectedChanged (ListViewItemEventArgs e)
        {
            var item = _categories [e.Item];
            GuiHelper gh = new GuiHelper();
            //MessageBox.Query("DEBUG",item,"Ok");
            RightPane = gh.SetWindow(item);
            Add (RightPane);
            _cachedCategoryIndex = e.Item;
        }
        
    }
    static void OpenUrl (string url)
    {
        try {
            if (RuntimeInformation.IsOSPlatform (OSPlatform.Windows)) {
                url = url.Replace ("&", "^&");
                Process.Start (new ProcessStartInfo ("cmd", $"/c start {url}") { CreateNoWindow = true });
            } else if (RuntimeInformation.IsOSPlatform (OSPlatform.Linux)) {
                using (var process = new Process {
                           StartInfo = new ProcessStartInfo {
                               FileName = "xdg-open",
                               Arguments = url,
                               RedirectStandardError = true,
                               RedirectStandardOutput = true,
                               CreateNoWindow = true,
                               UseShellExecute = false
                           }
                       }) {
                    process.Start ();
                }
            } else if (RuntimeInformation.IsOSPlatform (OSPlatform.OSX)) {
                Process.Start ("open", url);
            }
        } catch {
            MessageBox.Query("错误","无法打开指定链接！","确定");
            throw;
        }
    }

    static void DoNothing() { }

    static void EasterEgg()
    {
        if (Egg == 10)
        {
            MessageBox.Query("彩蛋","喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵~","确定");
            Egg = 0;
        }
        Egg++;
    }
}