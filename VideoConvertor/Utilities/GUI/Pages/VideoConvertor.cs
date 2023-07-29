using System.ComponentModel;
using NStack;
using Terminal.Gui;
using VideoConvertor.Utilities.VideosProcess;

namespace VideoConvertor.Utilities.GUI.Pages;

public class VideoConvertor
{
    private string? _files;
    private ustring[] _RadioLables;
    private ustring[] _RadioLablesCodec;
    private ustring[] _RadioLablesAudioCodec;
    private string _Target;
    private string _TargetCodec="h264";
    private string _TargetAudioCodec="aac";
    private string? _filessave;
    private Timer _timer;
    private const uint _timerTick = 100;
    private bool result;
    
    public FrameView GetWindow()
    {
        //右面板
        FrameView RightPane = new FrameView ("格式转换") {
            X = 25,
            Y = 1, // for menu
            Width = Dim.Fill (),
            Height = Dim.Fill (),
            CanFocus = false,
            Shortcut = Key.CtrlMask | Key.R
        };
        RightPane.Title = $"{RightPane.Title} ({RightPane.ShortcutTag})";
        RightPane.ShortcutAction = () => RightPane.SetFocus ();
        
        Label labelSelectFile = new Label("请选择要转码的文件:")
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
            OpenDialog openDialog = new OpenDialog()
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
            Application.Run(openDialog);
            if (!openDialog.Canceled && openDialog.FilePaths.Count > 0)
            {
                _files = openDialog.FilePaths[0];
                textFieldOpen.Text = _files;
                //DeBug(OpenDialog);
            }
        };
        RightPane.Add (buttonOpen);
        
        //选择目标格式标签
        Label labelSelectTarget = new Label("请选择目标格式:")
        {
            X = 1,
            Y = Pos.Y(textFieldOpen)+2,
        };
        RightPane.Add (labelSelectTarget);
        
        //选择目标格式组
        _RadioLables=new ustring[]{".mp4",".avi",".mov",".mkv"};
        RadioGroup radioGroup = new RadioGroup()
        {
            X = 2,
            Y = Pos.Y(labelSelectTarget)+2,
            RadioLabels = _RadioLables,
            SelectedItem = 0
        };
        RightPane.Add(radioGroup);
        radioGroup.SelectedItemChanged += OnSelectedItemChanged;
        
        
        Label labelSelectTargetCodec = new Label("请选择视频编码器:")
        {
            X = labelSelectTarget.Frame.Width+4,
            Y = Pos.Y(labelSelectTarget),
        };
        RightPane.Add (labelSelectTargetCodec);
        
        //选择视频编码组
        _RadioLablesCodec=new ustring[]{"h264","hevc","av1"};
        RadioGroup radioGroupCodec = new RadioGroup()
        {
            X = radioGroup.Frame.Width+13,
            Y = Pos.Y(labelSelectTargetCodec)+2,
            RadioLabels = _RadioLablesCodec,
            SelectedItem = 0
        };
        RightPane.Add(radioGroupCodec);
        radioGroupCodec.SelectedItemChanged += OnCodecSelectedItemChanged;
        
        Label labelSelectTargetAudioCodec = new Label("请选择音频编码器:")
        {
            X = labelSelectTargetCodec.Frame.Width+22,
            Y = Pos.Y(labelSelectTarget),
        };
        RightPane.Add (labelSelectTargetAudioCodec);
        
        //选择音频编码组
        _RadioLablesAudioCodec=new ustring[]{"aac"};
        RadioGroup radioGroupAudioCodec = new RadioGroup()
        {
            X = radioGroupCodec.Frame.Width+33,
            Y = Pos.Y(labelSelectTargetAudioCodec)+2,
            RadioLabels = _RadioLablesAudioCodec,
            SelectedItem = 0
        };
        RightPane.Add(radioGroupAudioCodec);
        radioGroupAudioCodec.SelectedItemChanged += OnAudioCodecSelectedItemChanged;
        
        
        //选择输出位置标签
        Label labelSaveFiles = new Label("请选择输出位置:")
        {
            X = 1,
            Y = Pos.Bottom(radioGroup)+1,
        };
        RightPane.Add (labelSaveFiles);
        
        //选择输出位置预览
        TextField textFieldSave = new TextField()
        {
            X = 2,
            Y= Pos.Y(labelSaveFiles)+2,
            Width = Dim.Fill()-2,
            Height = 1,
            Text = "请选择位置",
            Enabled = false,
            CanFocus = false
        };
        RightPane.Add(textFieldSave);
        
        //选择输出按钮
        Button buttonSave = new Button()
        {
            Text = "选择位置",
            Y = Pos.Y(labelSaveFiles),
            X = labelSaveFiles.Frame.Width+3,
            IsDefault = true,
        };
        RightPane.Add(buttonSave);
        buttonSave.Clicked += () =>
        {
            List<string> saveTypes = new List<string>(){_RadioLables[radioGroup.SelectedItem].ToString()};
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
                //DeBug(OpenDialog);
            }
        };

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

    void DeBug(string type)
    {
        if (type == "OpenDialog")
        {
            MessageBox.Query("DEBUG", _files, "OK");
        }
        
    }

    void OnSelectedItemChanged(SelectedItemChangedArgs e)
    {
        _Target = _RadioLables[e.SelectedItem].ToString();
    }
    
    void OnCodecSelectedItemChanged(SelectedItemChangedArgs e)
    {
        _TargetCodec = _RadioLablesCodec[e.SelectedItem].ToString();
    }
    
    void OnAudioCodecSelectedItemChanged(SelectedItemChangedArgs e)
    {
        _TargetAudioCodec = _RadioLablesAudioCodec[e.SelectedItem].ToString();
    }
    void Start(ProgressBar progressBar,Button start)
    {
        BackgroundWorker worker = new BackgroundWorker()
        {
            WorkerSupportsCancellation = true
        };
        worker.DoWork += (s, e) =>
        {
            Video.Video_format(_TargetCodec, _TargetAudioCodec, _files, _filessave /* + RandomName.GetRandomString(10) + _Target*/);
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