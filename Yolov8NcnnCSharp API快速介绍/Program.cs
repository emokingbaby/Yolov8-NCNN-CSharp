using Yolov8ChenZhe; //引入该开源dll文件
//该dll为32位，任何程序调用该dll必须是32位程序

namespace Project
{
    internal class Program
    {
        //第一步:实例化Yolov8
        public static CZyolov8 cZyolov8 = new CZyolov8();


        // 以对象方式返回所有识别结果存入当前容器。识别结果会自动清空与填充
        //targets[i].ObjId : 识别对象id
        //targets[i].X :  识别对象在屏幕中的x坐标
        //targets[i].Y :  识别对象在屏幕中的Y坐标
        //targets[i].W :  识别对象的宽度
        //targets[i].H :  识别对象的高度
        //targets[i].Prob :  识别对象的可信度
        //targets[i].Name :  识别对象的名称
        public static List<YoloTarget> targets = new List<YoloTarget>();
        public static string resultText = "";//返回识别结果的字符串文本(识别物体名称,对象屏幕坐标...全部写进一个字符串，以|分隔)
        public static int useTime = 0;//识别耗时（通常在8ms ~ 14ms）

        private static void Main()
        {
            //第二步:加载模型(成功返回true，失败返回false)。该函数为单线程（不推荐多线程加载，可能会崩）
            bool IsLoadSuccess = cZyolov8.LoadModelFromFile(
                // Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "") : 获取exe文件所在目录
                paramFile: Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./aaa.param")),//.param文件路径(腾讯Yolov NCNN框架模型标准文件。可以使用ONNX文件转换)
                binFile: Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./aaa.bin")),//.bin文件路径(腾讯Yolov NCNN框架模型标准文件。可以使用ONNX文件转换)
                                                                                                            //.names文件路径(非标准文件，但是必须要有)
                                                                                                            //如果没有该文件可自己创建一个，参考Yolov8NcnnCsharp模型示例框架中mod文件夹中names文件写法
                                                                                                            //names文件作用:将ai识别id对应到字符串上。id = 0的识别结果对应names文件第一行的字符串
                namesFile: Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./aaa.names")),
                cpuThreads: 1,//Cpu线程推理。在任务资源管理器->性能->CPU->逻辑处理器数量。通常一台家用电脑都是8~12线程。
                              //这里通常写1就行。如果输入线程超过电脑最大线程，会自动设置为1
                              //如果开启显卡推理，这里写不写无所谓
                useGpu: true,//开启显卡推理。显卡推荐英伟达，核显也可以，需要显卡驱动。如果电脑显卡实在带不动，就选择false。
                deviceId: 0,//显卡设备id选择第一张显卡。显卡设备id也可在任务资源管理器中查看。通常第一张卡是intel的核显。如果你有外部显卡是最好
                password: ""//模型密码。因版权问题有些模型是加密的需要密码。如果没有则填空。
            );

            //模型加载成功返回true，失败返回false
            Console.WriteLine(IsLoadSuccess);


            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {

            }
        }

        private static void UnloadModel()
        {
            //功能1: 卸载模型(单线程,不推荐多线程卸载，可能会崩)
            // bool IsUninstallModelSuccess = cZyolov8.UnloadModel();
            //模型卸载成功返回true，失败返回false
            // Console.WriteLine(IsUninstallModelSuccess);
        }


        private static void DetectScreenArea()
        {
            //功能2: 开启屏幕识别,识别到内容则返回true，否则返回false(单线程函数，大家需要配合后台线程与循环去执行)
            Task.Run(() =>
            {
                while (true)
                {
                    //每调用1次就识别1次。后续使用需要在循环中执行。
                    bool IsDetectSuccess = cZyolov8.DetectScreenArea(
                        left: 0,//识别区域左上角在屏幕的X坐标
                        top: 0,//识别区域左上角在屏幕的Y坐标
                        width: 1920,//识别框宽度(单位像素)
                        height: 1080,//识别框高度(单位像素)
                        targets: out Program.targets,//识别结果存入容器
                        resultText: out Program.resultText, //识别结果字符串存入字符串
                        useTime: out Program.useTime,//识别结果耗时存入
                        confidence: 0.4d,//置信度阈值。低于阈值的不会被识别。阈值范围0~1。类型：双精度
                        targetSize: 640 // 模型输入尺寸 (Input Size),根据训练导出的数据集来定。通常是640
                    );

                    if (IsDetectSuccess)
                    {
                        foreach (YoloTarget i in Program.targets)
                        {
                            Console.WriteLine($"对象ID:{i.ObjId}--X坐标:{i.X}--Y坐标:{i.Y}--宽度:{i.W}--高度:{i.H}--可信度:{i.Prob}--名称:{i.Name}");
                        }
                    }
                }

            });
        }

        private static void DetectScreenAreaFast()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    //功能3：快速识别(只返回bool值。识别到返回true，否则返回false)
                    bool IsDetectSuccess = cZyolov8.DetectScreenAreaFast(
                        left: 0,//识别区域左上角在屏幕的X坐标
                        top: 0,//识别区域左上角在屏幕的Y坐标
                        width: 1920,//识别框宽度(单位像素)
                        height: 1080,//识别框高度(单位像素)
                        confidence: 0.4d,//置信度阈值。低于阈值的不会被识别。阈值范围0~1。类型：双精度
                        targetSize: 640// 模型输入尺寸 (Input Size),根据训练导出的数据集来定。通常是640
                    );
                    Console.WriteLine(IsDetectSuccess);
                }
            });
        }

        private static void DetectImage()
        {
            //exe所在目录放一张图片进行识别
            byte[] imageBytes = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./1.jpg"));

            //功能4：传入二进制图片信息，识别成功返回true，否则返回false。并返回识别结果
            bool IsDetectImageSuccess = cZyolov8.DetectImage(
                imageBytes: imageBytes,
                targets: out Program.targets,//识别结果存入容器
                resultText: out Program.resultText, //识别结果字符串存入字符串
                useTime: out Program.useTime,//识别结果耗时存入
                confidence: 0.4d,//置信度阈值。低于阈值的不会被识别。阈值范围0~1。类型：双精度
                targetSize: 640 // 模型输入尺寸 (Input Size),根据训练导出的数据集来定。通常是640
            );

            if (IsDetectImageSuccess == true)
            {
                foreach (YoloTarget i in Program.targets)
                {
                    Console.WriteLine($"对象ID:{i.ObjId}--X坐标:{i.X}--Y坐标:{i.Y}--宽度:{i.W}--高度:{i.H}--可信度:{i.Prob}--名称:{i.Name}");
                }
            }
        }

        private static void DetectFile()
        {
            //exe所在目录放一张图片进行识别
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./1.jpg");

            //功能5：传入图片路径，识别成功返回true，否则返回false。并返回识别结果
            bool IsDetectImageSuccess = cZyolov8.DetectFile(
                imageFile: imagePath,//图片路径
                targets: out Program.targets,//识别结果存入容器
                resultText: out Program.resultText, //识别结果字符串存入字符串
                useTime: out Program.useTime,//识别结果耗时存入
                confidence: 0.4d,//置信度阈值。低于阈值的不会被识别。阈值范围0~1。类型：双精度
                targetSize: 640 // 模型输入尺寸 (Input Size),根据训练导出的数据集来定。通常是640
            );

            if (IsDetectImageSuccess == true)
            {
                foreach (YoloTarget i in Program.targets)
                {
                    Console.WriteLine($"对象ID:{i.ObjId}--X坐标:{i.X}--Y坐标:{i.Y}--宽度:{i.W}--高度:{i.H}--可信度:{i.Prob}--名称:{i.Name}");
                }
            }
        }

    }
}