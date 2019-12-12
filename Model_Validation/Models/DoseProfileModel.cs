using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_Validation.Models
{
    public class DoseProfileModel
    {
        public double FieldX { get; set; }
        public double FieldY { get; set; }
        public double SSD { get; set; }
        public string Energy { get; set; }
        public double Depth { get; set; }
        public double StartPos { get; set; }
        public List<DataPoint> DataPoints { get; set; }
        public DoseProfileModel()
        {
            DataPoints = new List<DataPoint>();
        }
    }   
}
