//识别窗口(不绘图)
using System.Windows.Forms.VisualStyles;
using Yolov8ChenZhe;
using Yolov8WindowsApp;

public class IdentifyWindow
{
    public Button IdentifyWindowBtn = new Button();
    public List<YoloTarget> targets = new List<YoloTarget>();
    public string resultText;
    public int useTime = 0;

    public GroupBox IdentifyWindowGroupBox = new GroupBox();

    //置信度文本
    private Label confidenceLabel = new Label();
    private TrackBar confidenceTb = new TrackBar();

    //耗时
    private Label TimeConsuming = new Label();

    //识别是否已经开启
    private bool IdentifyStart = false;

    //线程锁
    public object RecognizeLock = new object();

    public IdentifyWindow()
    {
        IdentifyWindowGroupBox.Location = new Point(380, 10);
        IdentifyWindowGroupBox.Size = new Size(180, 120);

        IdentifyWindowBtn.Text = "开启识别";
        IdentifyWindowBtn.Enabled = false;
        IdentifyWindowBtn.Location = new Point(30, 10);
        IdentifyWindowBtn.Size = new Size(120, 40);
        IdentifyWindowBtn.Cursor = Cursors.Hand;
        IdentifyWindowBtn.Click += StartIdentifyWindow;

        confidenceLabel.Text = $"置信度:{40}%";
        confidenceLabel.Location = new Point(10, 50);

        TimeConsuming.Text = $"耗时:{useTime}ms";
        TimeConsuming.Location = new Point(105, 50);




        confidenceTb.Location = new Point(0, 70);
        confidenceTb.Size = new Size(180, 20);
        confidenceTb.Minimum = 0;
        confidenceTb.Maximum = 100;
        confidenceTb.Value = 40;
        confidenceTb.ValueChanged += ConfidenceTb_ValueChanged;


        IdentifyWindowGroupBox.Controls.Add(IdentifyWindowBtn);
        IdentifyWindowGroupBox.Controls.Add(confidenceLabel);
        IdentifyWindowGroupBox.Controls.Add(confidenceTb);
        IdentifyWindowGroupBox.Controls.Add(TimeConsuming);

    }

    private void ConfidenceTb_ValueChanged(object? sender, EventArgs e)
    {
        confidenceLabel.Text = $"置信度:{confidenceTb.Value}%";
    }

    private void StartIdentifyWindow(object? sender, EventArgs e)
    {
        if (IdentifyStart == true)
        {
            IdentifyStart = false;
            IdentifyWindowBtn.Text = "开始识别";
            MainForm.OpenDrawingForm.Enabled = false;
            return;
        }
        IdentifyStart = true;
        IdentifyWindowBtn.Text = "暂停识别";
        MainForm.OpenDrawingForm.Enabled = true;
        Task.Run(() =>
        {
            while (IdentifyStart)
            {
                lock(RecognizeLock)
                {
                    bool ExitResult = Program.czyolov8.DetectScreenArea(
                        left: 0,
                        top: 0,
                        width: Program.ScreenWidth,
                        height: Program.ScreenHeight,
                        targets: out targets,
                        resultText: out resultText,
                        useTime: out useTime,
                        confidence: (double)confidenceTb.Value / 100,
                        targetSize: 640
                    );
                }
                TimeConsuming.Text = $"耗时:{useTime}ms";
            }
            TimeConsuming.Text = $"耗时:{0}ms";
        });

    }

    public void IsLoadSuccess(bool IsLoadSuccess)
    {
        IdentifyWindowBtn.Enabled = IsLoadSuccess;
    }
}