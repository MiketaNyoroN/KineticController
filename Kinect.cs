using Microsoft.Kinect;
using System;

namespace KineticController
{
    /// <summary>
    /// Kinectセンサーを雑に扱うクラス
    /// </summary>
    public class Kinect : IDisposable
    {
        public EventHandler<AllFramesReadyEventArgs> AllFrameReady;
        public EventHandler<DepthImageFrameReadyEventArgs> DepthFrameReady;
        public EventHandler<ColorImageFrameReadyEventArgs> ColorFrameReady;
        public EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;

        private KinectSensor currentSensor;
        private string connectedSensorId;

        /// <summary>
        /// 繋がりそうなKinectセンサーを探して起動します
        /// </summary>
        public Kinect()
        {
            KinectSensor.KinectSensors.StatusChanged += SensorStatusChanged;
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status != KinectStatus.Connected) { continue; }
                Start(sensor);
            }
        }

        public void Dispose()
        {
            currentSensor.Stop();
        }

        public void EnableDepthSensor()
        {
            currentSensor.DepthStream.Enable();
        }

        private void SensorStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status == KinectStatus.Disconnected && IsCurrentDevice(e.Sensor)) { Stop(); }
            if (e.Status == KinectStatus.Connected && connectedSensorId == null) { Start(e.Sensor); }
        }

        private void Start(KinectSensor sensor)
        {
            currentSensor = sensor;
            connectedSensorId = currentSensor.DeviceConnectionId;

            currentSensor.ColorStream.Enable();

            currentSensor.AllFramesReady += AllFrameReady;
            currentSensor.DepthFrameReady += DepthFrameReady;
            currentSensor.ColorFrameReady += ColorFrameReady;
            currentSensor.SkeletonFrameReady += SkeletonFrameReady;

            currentSensor.Start();
        }

        private void Stop()
        {

            currentSensor = null;
            connectedSensorId = null;
            currentSensor.Stop();
        }

        private bool IsCurrentDevice(KinectSensor sensor)
        {
            return connectedSensorId != null && sensor.DeviceConnectionId != connectedSensorId;
        }
    }
}

