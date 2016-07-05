using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleExifReader
{
    class DecimalLocation
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public DecimalLocation(decimal lat, decimal lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public DecimalLocation(DmsLocation dmsLocation)
        {
            Latitude = this._convertDmsToDecimal(dmsLocation.Latitude);
            Longitude = this._convertDmsToDecimal(dmsLocation.Longitude);
        }

        private decimal _convertDmsToDecimal(DmsPoint dmsPoint)
        {
            if (dmsPoint == null)
                return default(decimal);
            else
                return (decimal)dmsPoint.Degrees + (decimal)dmsPoint.Minutes / 60 + (decimal)dmsPoint.Seconds / 3600;
        }

        public override string ToString()
        {
            return String.Format("{0:f5}, {1:f5}", Latitude, Longitude);
        }
    }

    class DmsLocation
    {
        public DmsPoint Latitude { get; set; }
        public DmsPoint Longitude { get; set; }

        public DmsLocation(DmsPoint lat, DmsPoint lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", Latitude, Longitude);
        }
    }

    class DmsPoint
    {
        public double Degrees { get; set; }
        public double Minutes { get; set; }
        public double Seconds { get; set; }
        public PointType Type { get; set; }

        public DmsPoint(double degrees, double minutes, double seconds, PointType type)
        {
            Degrees = degrees;
            Minutes = minutes;
            Seconds = seconds;
            Type = type;
        }

        public DmsPoint(string dmsString, PointType type)
        {
            double[] parsed = this._getDmsFromString(dmsString);
            if (parsed != null)
            {
                Degrees = parsed[0];
                Minutes = parsed[1];
                Seconds = parsed[2];
            }
            else
            {
                throw new ArgumentOutOfRangeException("Unable to parse degrees, minutes, and seconds from provided data.");
            }

            Type = type;
        }

        private double[] _getDmsFromString(string dmsString)
        {
            double[] dms = new double[3];

            int degIndex = dmsString.IndexOf("°");
            int minIndex = dmsString.IndexOf("'");
            int secIndex = dmsString.IndexOf("\"");

            try
            {
                dms[0] = Double.Parse(dmsString.Substring(0, degIndex).Trim());
                dms[1] = Double.Parse(dmsString.Substring(degIndex+1, (minIndex-degIndex-1)).Trim());
                dms[2] = Double.Parse(dmsString.Substring(minIndex+1, (secIndex-minIndex-1)).Trim());
            }
            catch (FormatException)
            {
                return null;
            }

            return dms;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}",
                Math.Abs(Degrees),
                Minutes,
                Seconds,
                Type == PointType.Lat
                    ? Degrees < 0 ? "S" : "N"
                    : Degrees < 0 ? "W" : "E");
        }
    }

    enum PointType
    {
        Lat,
        Lon
    }
}
