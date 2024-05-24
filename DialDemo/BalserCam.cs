using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUDemo
{
    class BalserCam
    {
        public int CameraNumber = CameraFinder.Enumerate().Count;
        public delegate void CameraImage(Bitmap bmp);
        public event CameraImage CameraImageEvent;
        Camera camera;
        PixelDataConverter pxConvert = new PixelDataConverter();
        bool GrabOver = false;
        //public IGrabResult grabResult;

        public void CameraInit()
        {
            camera = new Camera();
            camera.CameraOpened += Configuration.AcquireContinuous;
            camera.ConnectionLost += Camera_ConnectionLost;
            camera.StreamGrabber.GrabStarted += StreamGrabber_GrabStarted;
            camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            camera.StreamGrabber.GrabStopped += StreamGrabber_GrabStopped;
            camera.Open();
            // 設定相機的分辨率
            //camera.Parameters[PLCamera.Width].SetValue(1920); // 設定寬度
            //camera.Parameters[PLCamera.Height].SetValue(1080); // 設定高度

        }
        
        private void StreamGrabber_GrabStarted(object sender, EventArgs e)
        {
            GrabOver = true;
        }
        private void StreamGrabber_ImageGrabbed(object sender, ImageGrabbedEventArgs e)
        {
            IGrabResult grabResult = e.GrabResult;
            if (grabResult.IsValid)
            {
                if (GrabOver)
                    CameraImageEvent(GrabResult2Bmp(grabResult));


            }
        }

        private void StreamGrabber_GrabStopped(object sender, GrabStopEventArgs e)
        {
            GrabOver = false;
        }

        private void Camera_ConnectionLost(object sender, EventArgs e)
        {
            camera.StreamGrabber.Stop();
            DestroyCamera();
        }

        public void OneShot()
        {
            if (camera != null)
            {
                camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.SingleFrame);
                camera.StreamGrabber.Start(1, GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
        }

        public void KeepShot()
        {
            if (camera != null)
            {
                camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
        }

        public void Stop()
        {
            if (camera != null)
            {
                camera.StreamGrabber.Stop();
            }
        }

        public Bitmap GrabResult2Bmp(IGrabResult grabResult)
        {
            Bitmap b = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);
            BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            pxConvert.OutputPixelFormat = PixelType.BGRA8packed;
            IntPtr bmpIntpr = bmpData.Scan0;
            pxConvert.Convert(bmpIntpr, bmpData.Stride * b.Height, grabResult);
            b.UnlockBits(bmpData);
            return b;
        }

        public void DestroyCamera()
        {
            if (camera != null)
            {
                camera.Close();
                camera.Dispose();
                camera = null;
            }
        }
    }
}
