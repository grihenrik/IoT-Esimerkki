using Emmellsoft.IoT.Rpi.SenseHat;
using RichardsTech.Sensors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoT_Esimerkki
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            
            this.InitializeComponent();
            tstTxt.Text = "IoT-esimerkki";
            Debug.WriteLine("Iot-esimerkki");


        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await CollectSensorData(new Progress<Measurement>(m =>
            {
                tstTxt.Text = $"{m.Temperature:F2} degrees, {m.Humidity:F2} RH%, {m.Pressure:F2} mbar\n ACC: {m.Acceleration.ToString()} GYRO: {m.Gyroscope.ToString()} MagneticField: {m.MagneticField.ToString()} ";
                Debug.WriteLine(tstTxt.Text);
            }));
        }

        private async Task CollectSensorData(IProgress<Measurement> progress)
        {
            using (ISenseHat senseHat = await SenseHatFactory.GetSenseHat())
            {
                for(; ; )
                {
                    try
                    {
                        senseHat.Sensors.HumiditySensor.Update();
                        senseHat.Sensors.PressureSensor.Update();
                        senseHat.Sensors.ImuSensor.Update();

                        var measurement = new Measurement()
                        {
                            Temperature = senseHat.Sensors.Temperature ?? 0,
                            Humidity = senseHat.Sensors.Humidity ?? 0,
                            Pressure = senseHat.Sensors.Pressure ?? 0,
                            MagneticField =(Vector3) senseHat.Sensors.MagneticField,
                            Gyroscope = (Vector3) senseHat.Sensors.Gyro,
                            Acceleration = (Vector3) senseHat.Sensors.Acceleration
                        };
                        progress.Report(measurement);

                        await AzureIoTHub.SendDeviceToCloudMessageAsync(measurement);

                    } catch (Exception e)
                    {
                        Debug.WriteLine("Exception: " + e.Message);
                    }
                    await Task.Delay(1000);
                }
            }
        }
    }
}
