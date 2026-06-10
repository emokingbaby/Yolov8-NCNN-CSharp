//加载模型按钮
using Yolov8ChenZhe;
using Yolov8WindowsApp;

public class LoadModelButton : Form
{
    public GroupBox LoadModelGroupBox = new GroupBox();
    private Button LoadModelBtn = new Button();

    //Cpu推理单选按钮
    private RadioButton CpuReasoning = new RadioButton();

    //Gpu推理单选按钮
    private RadioButton GpuReasoning = new RadioButton();

    //显卡推理还是cpu推理的显卡设备id与Cpu线程数
    private NumericUpDown CpuOrGpuNumericUpDown = new NumericUpDown();
    private Label CpuOrGpulabel = new Label();
    private bool LoadOrUninstallModel = false;

    //是否加载成功
    public bool LoadOrUninstallModelSuccess = false;


    //存放提示文本与路径
    Label[] Pathlabel = { new Label(), new Label(), new Label() };
    TextBox[] PathTxtName = { new TextBox(), new TextBox(), new TextBox() };


    //加载提示
    Label Hint = new Label();


    //按钮原始尺寸
    private Size ButtonOrginalSize = new Size(80, 40);


    //委托
    public delegate void PublishModelLoadISsuccess(bool IsSuccess);
    public event PublishModelLoadISsuccess? publishModelLoadISsuccess;

    public LoadModelButton()
    {
        LoadModelGroupBox.Location = new Point(10, 10);
        LoadModelGroupBox.Size = new Size(280, 320);
        LoadModelGroupBox.AutoSize = true;
        LoadModelGroupBox.Text = "加载模型";

        Hint.Text = "";
        Hint.AutoSize = true;
        Hint.ForeColor = Color.Red;
        Hint.Location = new Point(10, 60);


        CpuOrGpulabel.Text = "Cpu线程数:";
        CpuOrGpulabel.Location = new Point(120, 50);
        CpuOrGpuNumericUpDown.Minimum = 1;
        CpuOrGpuNumericUpDown.Maximum = 12;
        CpuOrGpuNumericUpDown.Value = 1;
        CpuOrGpuNumericUpDown.Increment = 1;
        CpuOrGpuNumericUpDown.Location = new Point(220, 50);
        CpuOrGpuNumericUpDown.Size = new Size(50, 20);



        CpuReasoning.Text = "Cpu推理";
        CpuReasoning.Location = new Point(110, 20);
        CpuReasoning.Checked = true;
        CpuReasoning.CheckedChanged += CpuReasoningCheckedChanged;


        GpuReasoning.Text = "Gpu推理";
        GpuReasoning.Location = new Point(220, 20);
        GpuReasoning.CheckedChanged += GpuReasoningCheckedChanged;

        Pathlabel[0].Text = "bin文件路径:";
        Pathlabel[1].Text = "param文件路径:";
        Pathlabel[2].Text = "names文件路径:";

        PathTxtName[0].PlaceholderText = "bin文件路径";
        PathTxtName[1].PlaceholderText = "param文件路径";
        PathTxtName[2].PlaceholderText = "names文件路径";
        PathTxtName[0].Text = "./mod/Overwatch.bin";
        PathTxtName[1].Text = "./mod/Overwatch.param";
        PathTxtName[2].Text = "./mod/Overwatch.names";
        for (int i = 0; i < Pathlabel.Length; ++i)
        {
            Pathlabel[i].AutoSize = true;
            Pathlabel[i].Location = new Point(10, 120 + i * 50);
            PathTxtName[i].Location = new Point(150, 120 + i * 50);
            PathTxtName[i].Size = new Size(200, 50);
        }



        LoadModelBtn.Text = "加载模型";
        LoadModelBtn.Cursor = Cursors.Hand;
        LoadModelBtn.Location = new Point(10, 20);
        LoadModelBtn.Size = ButtonOrginalSize;

        LoadModelBtn.Click += LoadModel;



        LoadModelGroupBox.Controls.Add(LoadModelBtn);
        LoadModelGroupBox.Controls.Add(CpuReasoning);
        LoadModelGroupBox.Controls.Add(GpuReasoning);
        LoadModelGroupBox.Controls.Add(CpuOrGpuNumericUpDown);
        LoadModelGroupBox.Controls.Add(CpuOrGpulabel);
        LoadModelGroupBox.Controls.Add(Hint);
        for (int i = 0; i < Pathlabel.Length; ++i)
        {
            LoadModelGroupBox.Controls.Add(Pathlabel[i]);
            LoadModelGroupBox.Controls.Add(PathTxtName[i]);
        }
    }

    private void GpuReasoningCheckedChanged(object? sender, EventArgs e)
    {
        if (!GpuReasoning.Checked) return;

        CpuOrGpulabel.Text = "显卡id:";

        decimal oldValue = CpuOrGpuNumericUpDown.Value;

        CpuOrGpuNumericUpDown.Minimum = 0;
        CpuOrGpuNumericUpDown.Maximum = 3;
        CpuOrGpuNumericUpDown.Increment = 1;

        if (oldValue < CpuOrGpuNumericUpDown.Minimum)
            CpuOrGpuNumericUpDown.Value = CpuOrGpuNumericUpDown.Minimum;
        else if (oldValue > CpuOrGpuNumericUpDown.Maximum)
            CpuOrGpuNumericUpDown.Value = CpuOrGpuNumericUpDown.Maximum;
        else
            CpuOrGpuNumericUpDown.Value = oldValue;
    }

    private void CpuReasoningCheckedChanged(object? sender, EventArgs e)
    {
        if (!CpuReasoning.Checked) return;

        CpuOrGpulabel.Text = "Cpu线程数:";

        decimal oldValue = CpuOrGpuNumericUpDown.Value;

        CpuOrGpuNumericUpDown.Minimum = 1;
        CpuOrGpuNumericUpDown.Maximum = 12;
        CpuOrGpuNumericUpDown.Increment = 1;

        if (oldValue < CpuOrGpuNumericUpDown.Minimum)
            CpuOrGpuNumericUpDown.Value = CpuOrGpuNumericUpDown.Minimum;
        else if (oldValue > CpuOrGpuNumericUpDown.Maximum)
            CpuOrGpuNumericUpDown.Value = CpuOrGpuNumericUpDown.Maximum;
        else
            CpuOrGpuNumericUpDown.Value = oldValue;
    }
    private void LoadModel(object? sender, EventArgs e)
    {
        if (!LoadOrUninstallModel)//加载模型
        {
            LoadModelBtn.Text = "模型加载中...";
            LoadModelBtn.Size = new Size(120, 40);
            LoadModelBtn.Enabled = false;

            int deviceId = 0;
            int cpuThreads = 1;
            //显卡加载
            if (!CpuReasoning.Checked)
            {
                deviceId = (int)CpuOrGpuNumericUpDown.Value;
            }
            else //cpu加载
            {
                cpuThreads = (int)CpuOrGpuNumericUpDown.Value;
            }

            bool IsLoadSuccess = Program.czyolov8.LoadModelFromFile(
                        paramFile: Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathTxtName[1].Text)),
                        binFile: Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathTxtName[0].Text)),
                        namesFile: Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathTxtName[2].Text)),
                        cpuThreads: cpuThreads,
                        useGpu: !CpuReasoning.Checked,
                        deviceId: deviceId,
                        password: ""
                    );
            if (IsLoadSuccess)
            {
                LoadModelBtn.Text = "卸载模型";
                LoadModelBtn.Size = ButtonOrginalSize;
                LoadModelBtn.Enabled = true;
                Hint.Text = "模型加载成功";
                LoadOrUninstallModel = true;
                LoadOrUninstallModelSuccess = true;
                PublishModelLoad(LoadOrUninstallModelSuccess);
            }
            else
            {
                LoadModelBtn.Text = "加载模型";
                LoadModelBtn.Size = ButtonOrginalSize;
                LoadModelBtn.Enabled = true;
                Hint.Text = "模型加载失败";
                LoadOrUninstallModelSuccess = false;
                PublishModelLoad(LoadOrUninstallModelSuccess);
            }

        }
        else//卸载模型
        {
            LoadModelBtn.Text = "模型卸载中...";
            LoadModelBtn.Size = new Size(120, 40);
            LoadModelBtn.Enabled = false;
            bool UnloadModelSuccess = Program.czyolov8.UnloadModel();
            if (UnloadModelSuccess)
            {
                LoadModelBtn.Text = "加载模型";
                LoadModelBtn.Size = ButtonOrginalSize;
                LoadModelBtn.Enabled = true;
                LoadOrUninstallModel = false;
                Hint.Text = "模型卸载成功";
                LoadOrUninstallModelSuccess = false;
                PublishModelLoad(LoadOrUninstallModelSuccess);
            }
            else
            {
                LoadModelBtn.Text = "卸载模型";
                LoadModelBtn.Size = ButtonOrginalSize;
                LoadModelBtn.Enabled = true;
                LoadOrUninstallModel = true;
                Hint.Text = "模型卸载失败";
                LoadOrUninstallModelSuccess = true;
                PublishModelLoad(LoadOrUninstallModelSuccess);
            }
        }
    }
    public void PublishModelLoad(bool IsSuccess)
    {
        publishModelLoadISsuccess?.Invoke(IsSuccess);
    }
}

