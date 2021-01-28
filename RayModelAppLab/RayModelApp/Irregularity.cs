using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RayModelApp
{
    public class Irregularity
    {
        public Irregularity()
        { 
        }

        public Irregularity(string s)
        {
            string[] arr = s.Split(',');
            x = float.Parse(arr[0]);
            y = float.Parse(arr[1]);
            z = float.Parse(arr[2]);
            ns = float.Parse(arr[3]);
            nb1 = float.Parse(arr[4]);
            nb2 = float.Parse(arr[5]);
        }

        [Description("X"), Category("Coordinates")]
        public float x { get; set; }
        [Description("Y"), Category("Coordinates")]
        public float y { get; set; }
        [Description("Z"), Category("Coordinates")]
        public float z { get; set; }
        [Description("Sea ​​sickness or wave height")]
        public float ns { get; set; }
        [Description("Medium density")]
        public float nb1 { get; set; }
        [Description("Speed of sound in the environment")]
        public float nb2 { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", x,y,z,ns,nb1,nb2);
        }

    }
}
