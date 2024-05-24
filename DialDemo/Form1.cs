using Emgu.CV;
using Yolov5Net.Scorer.Models;
using Yolov5Net.Scorer;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using Basler.Pylon;
using System.Drawing;
using System.Drawing.Imaging;
using PUDemo;
using Emgu.CV.Face;

namespace DialDemo
{
    public partial class Form1 : Form
    {
        static readonly string assetsPath = @"./";//GetAbsolutePath(@"");
        static readonly string PUmodelFilePath = Path.Combine(assetsPath, "NanyaPU_Model", "NanyaPU_Model.onnx");
        YoloScorer<YoloCocoP5Model_NanyaPU> PUDefectscorer;
        public int DialCount = 0;
        public DateTime dTimeNow;
        public DateTime dTimeRecord;
        //private Image<Bgr, byte> IPImage; // 移至外部以減少物件建立次數
        BalserCam camera = new BalserCam();
        public Form1()
        {
            InitializeComponent();
            InitModel();
            camera.CameraImageEvent += Camera_CameraImageEvent;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void InitModel()
        {
            PUDefectscorer = new YoloScorer<YoloCocoP5Model_NanyaPU>(PUmodelFilePath);
        }

        private void Update(List<SensorNum> Results)
        {
            if (DailListView.InvokeRequired)
                this.Invoke(new Action<List<SensorNum>>(Update), Results);
            else
            {
                ClearDailListView("");
                foreach (SensorNum Result in Results)
                {
                    string[] row = { Result.BBox, Result.DefectClass, Result.Conf };
                    var listViewItem = new ListViewItem(row);
                    DailListView.Items.Add(listViewItem);
                }
            }
        }
        private void ClearDailListView(string text)
        {
            if (DailListView.InvokeRequired)
                this.Invoke(new Action<string>(ClearDailListView), text);
            else
            {
                // 保存第一行的值
                //ListViewItem firstItem = DailListView.Items[0];
                // 清除 ListView 的所有項目
                DailListView.Items.Clear();
                // 重新添加第一行
                //DailListView.Items.Add(firstItem);
            }
        }
        public static List<YoloPrediction> MergeOverlappingBoxes(List<YoloPrediction> predictions)
        {
            List<YoloPrediction> mergedPredictions = new List<YoloPrediction>();
            for (int i = 0; i < predictions.Count; i++)
            {
                YoloPrediction currentBox = predictions[i];
                bool hasOverlap = false;
                for (int j = 0; j < mergedPredictions.Count; j++)
                {
                    YoloPrediction mergedBox = mergedPredictions[j];
                    // 判斷是否有重疊
                    if (currentBox.Label == mergedBox.Label &&
                        currentBox.Rectangle.IntersectsWith(mergedBox.Rectangle))
                    {
                        System.Drawing.RectangleF currentBoxR = new System.Drawing.RectangleF(currentBox.Rectangle.X, currentBox.Rectangle.Y, currentBox.Rectangle.Width, currentBox.Rectangle.Height);
                        System.Drawing.RectangleF mergedBoxR = new System.Drawing.RectangleF(mergedBox.Rectangle.X, mergedBox.Rectangle.Y, mergedBox.Rectangle.Width, mergedBox.Rectangle.Height);
                        // 合併框
                        System.Drawing.RectangleF unionRect = System.Drawing.RectangleF.Union(currentBoxR, mergedBoxR);
                        mergedPredictions[j] = new YoloPrediction()
                        {
                            Label = mergedBox.Label,
                            Score = (currentBox.Score + mergedBox.Score) / 2f,
                            Rectangle = new SixLabors.ImageSharp.RectangleF(unionRect.X, unionRect.Y, unionRect.Width, unionRect.Height)
                        };
                        hasOverlap = true;
                        break;
                    }
                }
                if (!hasOverlap)
                    mergedPredictions.Add(currentBox);
            }
            return mergedPredictions;
        }
        private void Camera_CameraImageEvent(Bitmap bmp)
        {
            //var stopWatch = Stopwatch.StartNew(); //啟動Stopw
            //YoloImageTransform transform = new YoloImageTransform(bmp);
            //List<YoloPrediction> predictions = PUDefectscorer.Predict(transform.Rgba32Image);
            //stopWatch.Stop(); //停止Stopwatch
            //Trace.WriteLine("推論圖片時間：" + stopWatch.ElapsedMilliseconds); //印出執行時間(毫秒)
            //var stopWatch1 = Stopwatch.StartNew(); //啟動Stopw
            //using (Image<Bgr, byte> localIPImage = new Image<Bgr, byte>(bmp.Width, bmp.Height))
            //{
            //    Graphics graphics = Graphics.FromImage(transform.BitmapImage);
            //    List<SensorNum> SensorNums = new List<SensorNum>();
            //    int objectcount = 1;

            //    if (predictions.Count > 0)
            //    {
            //        foreach (YoloPrediction prediction in predictions)
            //        {
            //            double score = Math.Round(prediction.Score, 2);
            //            if (score > 0.5)
            //            {
            //                System.Drawing.Rectangle targetRect = new System.Drawing.Rectangle(
            //                    (int)prediction.Rectangle.X,
            //                    (int)prediction.Rectangle.Y,
            //                    (int)prediction.Rectangle.Width,
            //                    (int)prediction.Rectangle.Height
            //                );
            //                localIPImage.ROI = targetRect;
            //                SensorNum Result = new SensorNum();
            //                graphics.DrawRectangles(new Pen(prediction.Label.Color, 3), new[] { targetRect });
            //                var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);
            //                graphics.DrawString($"Box_{objectcount} ({score})",
            //                    new Font("Consolas", 16, GraphicsUnit.Pixel), new SolidBrush(prediction.Label.Color),
            //                    new System.Drawing.PointF(x, y));
            //                Result.BBox = $"Box_{objectcount}";
            //                Result.DefectClass = prediction.Label.Name;
            //                Result.Conf = score.ToString();
            //                SensorNums.Add(Result);
            //                objectcount++;
            //            }
            //        }
            //    }
            //    Update(SensorNums);
            //pictureBox1.Invoke(new MethodInvoker(delegate
            //{
            //    Bitmap? old = pictureBox1.Image as Bitmap;
            pictureBox1.Image = bmp;//transform.BitmapImage;
            //    if (old != null)
            //        old.Dispose();
            //}));
            //stopWatch1.Stop(); //停止Stopwatch
            //Trace.WriteLine("顯示推論結果時間：" + stopWatch1.ElapsedMilliseconds); //印出執行時間(毫秒)
        }
        //private void ProcessFrame(object sender, EventArgs e)
        //{
        //    Mat frame = new Mat();
        //    capture.Retrieve(frame);
        //    var stopWatch = Stopwatch.StartNew(); //啟動Stopw
        //    if (frame != null)
        //    {
        //        YoloImageTransform transform = new YoloImageTransform(frame);
        //        IPImage.ROI = System.Drawing.Rectangle.Empty; // 清除之前的 ROI
        //        //Image<Rgba32> targetImage = transform.Rgba32Image;
        //        List<YoloPrediction> predictions = PUDefectscorer.Predict(transform.Rgba32Image);


        //        //List<YoloPrediction> mergedPredictions = MergeOverlappingBoxes(predictions);
        //        Graphics graphics = Graphics.FromImage(transform.BitmapImage);
        //        List<SensorNum> SensorNums = new List<SensorNum>();
        //        int objectcount = 1;
        //        if (predictions.Count > 0)
        //        {
        //            Image<Bgr, byte> IPImage = transform.BitmapImage.ToImage<Bgr, byte>();
        //            foreach (YoloPrediction prediction in predictions)//foreach (var prediction in mergedPredictions)
        //            {
        //                double score = Math.Round(prediction.Score, 2);
        //                if (score > 0.5)
        //                {
        //                    // 將 System.Drawing.Rectangle 轉換成 SixLabors.ImageSharp.Rectangle
        //                    System.Drawing.Rectangle targetRect = new System.Drawing.Rectangle(
        //                        (int)prediction.Rectangle.X,
        //                        (int)prediction.Rectangle.Y,
        //                        (int)prediction.Rectangle.Width,
        //                        (int)prediction.Rectangle.Height
        //                    );
        //                    IPImage.ROI = targetRect;
        //                    SensorNum Result = new SensorNum();

        //                    graphics.DrawRectangles(new Pen(prediction.Label.Color, 3),new[] { targetRect });
        //                    var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);

        //                    graphics.DrawString($"{Result.BBox} ({score})",//graphics.DrawString($"{prediction.Label.Name} ({score})",
        //                        new Font("Consolas", 16, GraphicsUnit.Pixel), new SolidBrush(prediction.Label.Color),
        //                        new System.Drawing.PointF(x, y));
        //                    Result.BBox = $"Box_{objectcount}";
        //                    Result.DefectClass = prediction.Label.Name;
        //                    Result.Conf = score.ToString();
        //                    SensorNums.Add(Result);
        //                }
        //                objectcount++;
        //            }
        //        }
        //        Update(SensorNums);
        //        pictureBox1.Image = transform.BitmapImage;
        //        //Thread.Sleep(10);
        //        //Application.DoEvents(); // 刷新窗口以显示下一帧
        //        //targetImage.Dispose();
        //    }
        //    stopWatch.Stop(); //停止Stopwatch
        //    Trace.WriteLine(stopWatch.ElapsedMilliseconds); //印出執行時間(毫秒)
        //}
        private void btnStart_Click(object sender, EventArgs e)
        {
            //camera.OneShot();
            camera.KeepShot();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            camera.Stop();
        }
        private void Savebtn_Click(object sender, EventArgs e)
        {
            pictureBox1.Invoke(new Action(() =>
            {
                // Capture the current image from the pictureBox
                Bitmap currentImage = pictureBox1.Image as Bitmap;

                if (currentImage != null)
                {
                    // Set the default file name prefix
                    string classname = "Abnormalinner";
                    string fileNamePrefix = "captured_image_" + classname + "_count";
                    string fileExtension = ".png";

                    // Specify the path where you want to save the image
                    int count = 1;
                    string folderPath = Path.Combine(assetsPath, classname);
                    string filePath;

                    // Create the folder if it doesn't exist
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    do
                    {
                        // Generate the full file path with the incremented count
                        filePath = Path.Combine(folderPath, $"{fileNamePrefix}_{count}{fileExtension}");
                        count++;
                    } while (File.Exists(filePath));

                    // Save the image as a PNG file
                    currentImage.Save(filePath, ImageFormat.Png);
                    //MessageBox.Show($"Image saved successfully at: {filePath}", "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }));
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            camera.DestroyCamera();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (camera.CameraNumber > 0)
                camera.CameraInit();
            else
            {
                MessageBox.Show("未連接到相機");
                Unanble();
            }
        }
        void Unanble()
        {
            btnStop.Enabled = false;
            btnStart.Enabled = false;
        }


    }
}