using System;

namespace WOP.Util {
    public class WayPoint {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public DateTime Time { get; set; }
        public DateTime LocalTime { get { return Time.ToLocalTime(); } }

        public override string ToString()
        {
            return string.Format("lon:{0}, lat:{1}, ele:{2}, time:{3}");
        }
    }

    public class BogenMass {
        public byte Grad { get; set; }
        public byte Minuten { get; set; }
        public double Sekunden { get; set; }
        public bool Plus { get; set; }
    }
}