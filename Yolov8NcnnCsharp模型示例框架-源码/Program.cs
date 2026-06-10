using System;
using System.Windows.Forms;
using Yolov8ChenZhe;
using System.Reflection;

//主程序
namespace Yolov8WindowsApp
{
    static class Program
    {

        public static CZyolov8 czyolov8 = new CZyolov8();

        public static int ScreenWidth = Screen.PrimaryScreen!.Bounds.Width;
        public static int ScreenHeight = Screen.PrimaryScreen!.Bounds.Height;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.Run(new MainForm());
        }
    }
    // 主窗口
    public class MainForm : Form
    {
        //识别
        public static IdentifyWindow identifyWindow = new IdentifyWindow();

        //按钮打开绘图窗口
        public static Button OpenDrawingForm = new Button();


        //是否打开子窗口
        public static bool IsOpenDrawingForm;

        private DrawingForm drawingForm = new DrawingForm(identifyWindow);

        //当前窗口是否最小化
        public static bool WinDowMin = false;



        public MainForm()
        {
            this.Text = "Yolov8识别框架演示-QQ交流群:173887003";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            LoadModelButton loadModelButton = new LoadModelButton();
            Controls.Add(loadModelButton.LoadModelGroupBox);


            Controls.Add(identifyWindow.IdentifyWindowGroupBox);

            //模型加载完成后才可以识别
            loadModelButton.publishModelLoadISsuccess += identifyWindow.IsLoadSuccess;



            OpenDrawingForm.Text = "开启辅助绘图";
            OpenDrawingForm.Enabled = false;
            OpenDrawingForm.Location = new Point(420, 200);
            OpenDrawingForm.Cursor = Cursors.Hand;
            OpenDrawingForm.AutoSize = true;
            OpenDrawingForm.Click += (s, e) =>
            {
                if (IsOpenDrawingForm == true)
                {
                    drawingForm.Hide();
                    IsOpenDrawingForm = false;
                    OpenDrawingForm.Text = "开启辅助绘图";
                    return;
                }
                OpenDrawingForm.Text = "关闭辅助绘图";
                drawingForm.Show();
                IsOpenDrawingForm = true;
            };
            Controls.Add(OpenDrawingForm);
            this.Resize += (r, e) =>
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    WinDowMin = true;
                }
                else if (this.WindowState == FormWindowState.Normal)
                {
                    WinDowMin = false;
                }
            };
        }
    }

    //绘图窗口
    public class DrawingForm : Form
    {

        private IdentifyWindow identifyWindow;
        public DrawingForm(IdentifyWindow identifyWindow)
        {
            this.identifyWindow = identifyWindow;
            this.Size = new Size(Program.ScreenWidth, Program.ScreenHeight);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.BackColor = Color.FromArgb(0, 0, 0);
            this.TransparencyKey = Color.FromArgb(0, 0, 0);
            this.DoubleBuffered = true;
            this.TopMost = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            FlushWindow();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Pen Redpen = new Pen(Color.Red, 3);
            Pen Grrenpen = new Pen(Color.LightGreen, 3);
            SolidBrush font = new SolidBrush(Color.SkyBlue);
            Pen pen = new Pen(Color.FromArgb(0, 0, 0, 0), 1); ;

            lock (identifyWindow.RecognizeLock)
            {
                foreach (YoloTarget i in identifyWindow.targets)
                {
                    if (i.Name == "Enemy")
                    {
                        pen = Redpen;
                    }
                    else if (i.Name == "Our")
                    {
                        pen = Grrenpen;
                    }
                    g.DrawString(i.Prob.ToString(), new Font("微软雅黑", 12), font, new Point(i.X, i.Y));
                    g.DrawRectangle(pen, i.X, i.Y, i.W, i.H);
                }
            }
        }

        private async Task FlushWindow()
        {
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(30));

            while (await timer.WaitForNextTickAsync())
            {
                if (MainForm.IsOpenDrawingForm && MainForm.WinDowMin == false)
                {
                    this.Invalidate();
                }
            }
        }
    }
}