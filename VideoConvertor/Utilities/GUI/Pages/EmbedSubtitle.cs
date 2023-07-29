using System.ComponentModel;
using Terminal.Gui;
using VideoConvertor.Utilities.VideosProcess;

namespace VideoConvertor.Utilities.GUI.Pages;

public class EmbedSubtitle
{
    private string? _files;
    private string? _filesAss;
    private string? _filessave;
    private Timer _timer;
    private const uint _timerTick = 100;

    public FrameView GetWindow()
    {
        FrameView RightPane = new FrameView ("嵌入字幕") {
            X = 25,
            Y = 1, // for menu
            Width = Dim.Fill (),
            Height = Dim.Fill (),
            CanFocus = false,
            Shortcut = Key.CtrlMask | Key.R
        };
        RightPane.Title = $"{RightPane.Title} ({RightPane.ShortcutTag})";
        RightPane.ShortcutAction = () => RightPane.SetFocus ();
        
        Label labelSelectFile = new Label("请选择要添加字幕的视频:")
        {
            X = 1,
            Y = 1,
        };
        RightPane.Add (labelSelectFile);
        
        //文件选择预览
        TextField textFieldOpen = new TextField()
        {
            X = 2,
            Y= Pos.Y(labelSelectFile)+2,
            Width = Dim.Fill()-2,
            Height = 1,
            Text = "请选择文件",
            Enabled = false,
            CanFocus = false
        };
        RightPane.Add(textFieldOpen);
        
        //选择文件按钮
        Button buttonOpen = new Button()
        {
            Text = "选择文件",
            Y = Pos.Y(labelSelectFile),
            X = labelSelectFile.Frame.Width+3,
            IsDefault = true,
        };
        buttonOpen.Clicked += () =>
        {
            List<string> aTypes = new List<string>() { ".mp4;.avi;.mov;.mkv", ".mp4", ".avi", ".mov", ".mkv", ".*" };
            OpenDialog openDialogFile = new OpenDialog()
            {
                Title = "选择文件",
                NameDirLabel = "目录",
                NameFieldLabel = "文件",
                Message = "请选择要转码的文件",
                Prompt = "确认",
                Cancel = "取消",
                AllowsMultipleSelection = false,
                AllowedFileTypes = aTypes.ToArray(),
            };
            Application.Run(openDialogFile);
            if (!openDialogFile.Canceled && openDialogFile.FilePaths.Count > 0)
            {
                _files = openDialogFile.FilePaths[0];
                textFieldOpen.Text = _files;
                //DeBug(OpenDialog);
            }
        };
        RightPane.Add (buttonOpen);
        
        //字幕选择
        Label labelSelectAss = new Label("请选择要添加的字幕:")
        {
            X = 1,
            Y = Pos.Y(textFieldOpen)+2,
        };
        RightPane.Add (labelSelectAss);
        //文件选择预览
        TextField textFieldOpenAss = new TextField()
        {
            X = 2,
            Y= Pos.Y(labelSelectAss)+2,
            Width = Dim.Fill()-2,
            Height = 1,
            Text = "请选择文件",
            Enabled = false,
            CanFocus = false
        };
        RightPane.Add(textFieldOpenAss);
        //选择文件按钮
        Button buttonOpenAss = new Button()
        {
            Text = "选择文件",
            Y = Pos.Y(labelSelectAss),
            X = labelSelectAss.Frame.Width+3,
            IsDefault = true,
        };
        buttonOpenAss.Clicked += () =>
        {
            List<string> aTypes = new List<string>() { ".mp4;.avi;.mov;.mkv", ".mp4", ".avi", ".mov", ".mkv", ".*" };
            OpenDialog openDialogAss = new OpenDialog()
            {
                Title = "选择文件",
                NameDirLabel = "目录",
                NameFieldLabel = "文件",
                Message = "请选择要转码的文件",
                Prompt = "确认",
                Cancel = "取消",
                AllowsMultipleSelection = false,
                AllowedFileTypes = aTypes.ToArray(),
            };
            Application.Run(openDialogAss);
            if (!openDialogAss.Canceled && openDialogAss.FilePaths.Count > 0)
            {
                _filesAss = openDialogAss.FilePaths[0];
                textFieldOpenAss.Text = _filesAss;
                //DeBug(OpenDialog);
            }
        };
        RightPane.Add (buttonOpenAss);
        
        //保存位置选择
        Label labelSelectSave = new Label("请选择输出位置:")
        {
            X = 1,
            Y = Pos.Y(textFieldOpenAss)+2,
        };
        RightPane.Add (labelSelectSave);
        //文件选择预览
        TextField textFieldSave = new TextField()
        {
            X = 2,
            Y= Pos.Y(labelSelectSave)+2,
            Width = Dim.Fill()-2,
            Height = 1,
            Text = "请选择位置",
            Enabled = false,
            CanFocus = false
        };
        RightPane.Add(textFieldSave);
        //选择文件按钮
        Button buttonSave = new Button()
        {
            Text = "选择位置",
            Y = Pos.Y(labelSelectSave),
            X = labelSelectSave.Frame.Width+3,
            IsDefault = true,
        };
        buttonSave.Clicked += () =>
        {
            List<string> saveTypes = new List<string>(){".mp4"};
            SaveDialog saveDialog = new SaveDialog()
            {
                Title = "保存位置",
                Message = "请选择保存位置",
                Prompt = "保存",
                Cancel = "取消",
                NameDirLabel = "目录",
                NameFieldLabel = "文件名",
                AllowedFileTypes = saveTypes.ToArray()
            };
            Application.Run(saveDialog);
            if (!saveDialog.Canceled && saveDialog.FilePath.Length > 0)
            {
                _filessave = saveDialog.FilePath.ToString();
                textFieldSave.Text =  _filessave;
            }
        };
        RightPane.Add (buttonSave);
        
        Button start = new Button()
        {
            Text = "执行",
            Y = Pos.Bottom(RightPane)-9,
            X = Pos.Right(textFieldSave)-15,
            IsDefault = true
        };
        RightPane.Add(start);

        ProgressBar progressBar = new ProgressBar()
        {
            X=2,
            Y = Pos.Y(start),
            Width = Dim.Fill(20),
            ProgressBarStyle = ProgressBarStyle.MarqueeContinuous
        };
        RightPane.Add(progressBar);
        Label progressLabel = new Label()
        {
            Text = "状态:",
            X=1,
            Y=Pos.Y(progressBar)-2,
        };
        RightPane.Add(progressLabel);
        start.Clicked += () =>
        {
            //TODO:文件转码操作
            start.Enabled = false;
            progressBar.Fraction = 0F;
            _timer = new Timer(state =>
            {
                Application.MainLoop?.Invoke(() => progressBar.Pulse());
            }, null, 0, _timerTick);
            Start(progressBar,start);
        };
        
        
        return RightPane;
    }

    void Start(ProgressBar progressBar, Button start)
    {
        BackgroundWorker worker = new BackgroundWorker()
        {
            WorkerSupportsCancellation = true
        };
        worker.DoWork += (s, e) =>
        {
            Video.subtitle(_files, _filessave, _filesAss);
        };
        worker.RunWorkerCompleted += (s, e) =>
        {
            //处理完毕=1
            progressBar.Fraction = 1;
            _timer.Dispose();
            start.Enabled = true;
            MessageBox.Query("结果", "操作已成功完成！", "确认");
        };
        worker.RunWorkerAsync();
    }
}