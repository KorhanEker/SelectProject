using Microsoft.Expression.Encoder.ScreenCapture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Profiles;
using System.Drawing;
using System.Windows.Forms;
using System.Windows;

namespace SelectTest.RecordingHelper
{
    public class ScreenRecorder
    {
        private ScreenCaptureJob screenCaptureJob;
        private const string VideoExtension = ".wmv";
        public  string resourcePath_VideoLocation;
        private const int FrameRate = 5;
        private const int Quality = 95;
        private int width = SystemInformation.VirtualScreen.Width;
        private int height = SystemInformation.VirtualScreen.Height;

        public void SetVideoOutputLocation(string testName = "")
        {
            try
            {
                screenCaptureJob = new ScreenCaptureJob();
                screenCaptureJob.CaptureRectangle = Screen.PrimaryScreen.Bounds;
                screenCaptureJob.ScreenCaptureVideoProfile.Force16Pixels = true;
                screenCaptureJob.ShowFlashingBoundary = true;
                screenCaptureJob.ScreenCaptureVideoProfile.FrameRate = FrameRate;
                screenCaptureJob.CaptureMouseCursor = true;
                screenCaptureJob.ScreenCaptureVideoProfile.Quality = Quality;
                screenCaptureJob.ScreenCaptureVideoProfile.AutoFit = true;
                var path = Assembly.GetCallingAssembly().CodeBase;
                var projectPath = new Uri(path.Substring(0, path.LastIndexOf("bin"))).LocalPath;
                resourcePath_VideoLocation = Path.Combine(projectPath, "VideoRecordings");
                if (string.IsNullOrEmpty(testName))
                    testName = "AutomationTest";
                screenCaptureJob.OutputScreenCaptureFileName = Path.Combine(resourcePath_VideoLocation, string.Format("{0}{1}{2}", testName, DateTime.UtcNow.ToString("MMddyyyy_Hmm"), VideoExtension));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error received : " + ex.Message + " - " + ex.StackTrace);
            }
            
        }

        private void DeleteOldRecordings()
        {
            Directory.GetFiles(resourcePath_VideoLocation)
                            .Select(f => new FileInfo(f))
                            .Where(f => f.FullName.Contains(VideoExtension))
                            .ToList()
                            .ForEach(f => f.Delete());
        }
        public void StartRecording()
        {
            DeleteOldRecordings();
            screenCaptureJob.Start();
        }
        public void StopRecording()
        {
            screenCaptureJob.Stop();
        }
    }
}

