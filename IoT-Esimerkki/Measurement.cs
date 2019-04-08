using RichardsTech.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT_Esimerkki
{
    class Measurement
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Gyroscope { get; set; }
        public Vector3 MagneticField { get; set; }
        public string DeviceId { get; set; }

    }
}
