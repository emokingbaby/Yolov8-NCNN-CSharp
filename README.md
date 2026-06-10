```markdown
# [Yolov8AI视觉-极简调用](https://github.com/emokingbaby/Yolov8-NCNN-CSharp/tree/main)

基于腾讯 **NCNN 框架**的 **YOLOv8 C# 封装**，只需几行代码即可快速运行 AI 视觉模型。**不需要你懂任何关于 AI 的算法知识！**

> YOLOv8 C# library based on Tencent NCNN, easy to run AI vision models with just a few lines of code. You don't need to know anything about AI algorithms!

<p align="center">
  <img src="https://github.com/emokingbaby/Yolov8-NCNN-CSharp/blob/main/ReadmeImgs/Yolov8NcnnCsharp.webp?raw=true" alt="Yolov8NcnnCsharp">
</p>

---

## 环境搭建与使用 🌁

### 1. <mark>下载 .NET SDK 10.0（推荐 10.0）</mark>

- 下载网址：[https://dotnet.microsoft.com/zh-cn/](https://dotnet.microsoft.com/zh-cn/)

> 你可以下载**运行时**或**便携版**，随便你。最后只需要在 `cmd` 窗口输入：  
> `dotnet --version`  
> 如果显示 `10.0.203` 就代表环境配置成功 🌱

---

### 2. 创建你的第一个 C# 控制台项目

在任意目录下，创建控制台项目：

```bash
dotnet new console
```

> 项目中应包含两个基础文件：`Program.cs` 和 `project.csproj`  
>
> <font color="red">⚠️ 注意：开源 DLL 均为 32 位，你应该使用 32 位环境去编写代码。如果你的 C# 环境是 64 位，可以在打包时选择 32 位运行。</font>

---

### 3. 复制两个 DLL 文件到项目根目录

将以下两个文件复制到项目根目录：

- `Yolov8ChenZhe.dll`
- `Yolov8Ncnn.dll`

---

### 4. 编写 `project.csproj` 文件

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="Yolov8ChenZhe">
      <HintPath>Yolov8ChenZhe.dll</HintPath>
    </Reference>
  </ItemGroup>

  <ItemGroup>
    <Content Include="Yolov8Ncnn.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
</Project>
```

---

### 5. <mark>运行与打包</mark>

- **运行（SDK 必须是 32 位）：**
  ```bash
  dotnet run
  ```

- **打包为独立 EXE（32 位）：**
  ```bash
  dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true
  ```

---

## 效果展示

<table align="center">
  <tr>
    <td align="center"><img src="https://github.com/emokingbaby/Yolov8-NCNN-CSharp/blob/main/ReadmeImgs/KLBQ.jpg?raw=true" alt="KLBQ" height="200"></td>
    <td align="center"><img src="https://github.com/emokingbaby/Yolov8-NCNN-CSharp/blob/main/ReadmeImgs/OverWatch.jpg?raw=true" alt="OverWatch" height="200"></td>
    <td align="center"><img src="https://github.com/emokingbaby/Yolov8-NCNN-CSharp/blob/main/ReadmeImgs/Result.jpg?raw=true" alt="Result" height="200"></td>
  </tr>
</table>

---

## API 介绍

> 具体详情请见开源文件：[Yolov8NcnnCSharp API 快速介绍]()

---

### 1. <mark>开始前的准备</mark>

```cs
using Yolov8ChenZhe;  // 使用开源项目命名空间

// 实例化该命名空间中的 CZyolov8 对象
public static CZyolov8 cZyolov8 = new CZyolov8();
```

---

### 2. <mark>加载模型 LoadModelFromFile</mark>

> <font color="red">本开源项目不包含模型训练与导出。</font>

NCNN 框架的 YOLOv8 有 **2 个模型文件** 和 **1 个自定义文件**：

- `.param` 文件和 `.bin` 文件：模型训练完后默认导出的官方文件
- `.names` 文件：将识别 ID 绑定一个名称（必须要有且不能为空）

#### .names 文件示例

```txt
apple
banana
```

> 当 AI 识别物体 ID = 0 时，返回 `apple`；ID = 1 时，返回 `banana`。

#### 加载代码示例

```cs
// 加载模型（成功返回 true，失败返回 false）
// 该函数为单线程（不推荐多线程加载，可能会崩）
bool IsLoadSuccess = cZyolov8.LoadModelFromFile(
    paramFile: Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./aaa.param")),
    binFile: Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./aaa.bin")),
    namesFile: Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./aaa.names")),
    cpuThreads: 1,           // CPU 线程推理，通常写 1 即可
    useGpu: true,            // 开启显卡推理（推荐英伟达显卡）
    deviceId: 0,             // 显卡设备 ID（0 通常为核显）
    password: ""             // 模型密码（若无则留空）
);
```

> 如果输入线程超过电脑最大线程数，会自动设为 1。  
> 如果开启显卡推理但显卡带不动，建议设为 `false`。

---

### 3. <mark>卸载模型 UnloadModel</mark>

```cs
// 卸载模型（单线程，不推荐多线程）
// 成功返回 true，失败返回 false
bool IsUninstallModelSuccess = cZyolov8.UnloadModel();
```

---

### 4. <mark>识别屏幕 DetectScreenArea</mark>

```cs
// 识别结果容器
public static List<YoloTarget> targets = new List<YoloTarget>();
public static string resultText = "";   // 识别结果字符串（以 | 分隔）
public static int useTime = 0;          // 识别耗时（通常 8ms ~ 14ms）

// 开启屏幕识别（需配合后台线程与循环执行）
Task.Run(() =>
{
    while (true)
    {
        bool IsDetectSuccess = cZyolov8.DetectScreenArea(
            left: 0, top: 0, width: 1920, height: 1080,
            targets: out targets,
            resultText: out resultText,
            useTime: out useTime,
            confidence: 0.4d,   // 置信度阈值（0~1）
            targetSize: 640     // 模型输入尺寸
        );

        if (IsDetectSuccess)
        {
            foreach (YoloTarget i in targets)
            {
                Console.WriteLine($"对象ID:{i.ObjId}--X:{i.X}--Y:{i.Y}--W:{i.W}--H:{i.H}--可信度:{i.Prob}--名称:{i.Name}");
            }
        }
    }
});
```

> `YoloTarget` 包含以下属性：`ObjId`（ID）、`X`、`Y`、`W`（宽度）、`H`（高度）、`Prob`（可信度）、`Name`（名称）

---

### 5. <mark>快速识别屏幕 DetectScreenAreaFast</mark>

```cs
Task.Run(() =>
{
    while (true)
    {
        // 只返回是否识别到目标，不返回具体结果
        bool IsDetectSuccess = cZyolov8.DetectScreenAreaFast(
            left: 0, top: 0, width: 1920, height: 1080,
            confidence: 0.4d,
            targetSize: 640
        );
    }
});
```

---

### 6. <mark>传入图片字节识别 DetectImage</mark>

```cs
byte[] imageBytes = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./1.jpg"));

bool IsDetectImageSuccess = cZyolov8.DetectImage(
    imageBytes: imageBytes,
    targets: out targets,
    resultText: out resultText,
    useTime: out useTime,
    confidence: 0.4d,
    targetSize: 640
);

if (IsDetectImageSuccess)
{
    foreach (YoloTarget i in targets)
    {
        Console.WriteLine($"对象ID:{i.ObjId}--X:{i.X}--Y:{i.Y}--W:{i.W}--H:{i.H}--可信度:{i.Prob}--名称:{i.Name}");
    }
}
```

---

### 7. <mark>传入图片路径识别 DetectFile</mark>

```cs
string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./1.jpg");

bool IsDetectImageSuccess = cZyolov8.DetectFile(
    imageFile: imagePath,
    targets: out targets,
    resultText: out resultText,
    useTime: out useTime,
    confidence: 0.4d,
    targetSize: 640
);

if (IsDetectImageSuccess)
{
    foreach (YoloTarget i in targets)
    {
        Console.WriteLine($"对象ID:{i.ObjId}--X:{i.X}--Y:{i.Y}--W:{i.W}--H:{i.H}--可信度:{i.Prob}--名称:{i.Name}");
    }
}
```

---

## 💡 小提示

- 所有识别函数建议放在 **后台线程**（如 `Task.Run`）中执行，避免阻塞主线程
- 屏幕识别区域 `(left, top, width, height)` 请根据你的实际屏幕分辨率调整
- 模型输入尺寸 `targetSize` 需与训练时导出的尺寸一致（通常为 640）
- 置信度 `confidence` 越高，识别结果越精准，但可能漏检

---

**Happy Coding! 🚀**
```

