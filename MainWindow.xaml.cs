using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace KineticController
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor activeSensor;
        private WriteableBitmap outputImage;

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            KinectSensor.KinectSensors.StatusChanged += KinectStatusChanged;

            if (KinectSensor.KinectSensors.Count > 0)
            {
                ConnectSensor(KinectSensor.KinectSensors[0]);
            }
        }

        private void KinectStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Debug.WriteLine("Kinect status changed: {0}=>{1}", e.Sensor.DeviceConnectionId, e.Sensor.Status);

            switch (e.Sensor.Status)
            {
                case KinectStatus.Connected:
                    ConnectSensor(e.Sensor);
                    break;
                case KinectStatus.Disconnected:
                    DisconnectSensor(e.Sensor);
                    break;
            }
        }

        private void ConnectSensor(KinectSensor sensor)
        {
            if (activeSensor != null) { return; }

            activeSensor = sensor;

            outputImage = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
            ColorImage.Source = outputImage;
            activeSensor.ColorFrameReady += OnColorFrameReady;
            activeSensor.ColorStream.Enable();

            activeSensor.Start();
            
        }

        private void OnColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame == null) { return; }
                outputImage.WritePixels(
                    new Int32Rect(0, 0, 640, 480),
                    frame.GetRawPixelData(),
                    640 * frame.BytesPerPixel,
                    0);
            }
        }

        private void DisconnectSensor(KinectSensor sensor)
        {
            if (activeSensor == null) { return; }
            if (activeSensor.DeviceConnectionId != sensor.DeviceConnectionId) { return; }

            sensor.Stop();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            DisconnectSensor(activeSensor);
        }
    }
}
