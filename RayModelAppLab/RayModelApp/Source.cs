using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RayModelApp
{
    
    public struct Point
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
    }
    [Description("Frequency")]
    public struct Frequency
    {
        [Description("Frequency")]
        public float Freq { get; set; }
        [Description("Phase")]
        public float Phase { get; set; }

        public override string ToString() 
        {
            return string.Format("{0} {1}", Freq, Phase);
        }
    }
    [Description("Source of sound")]
    public class Source
    {
        public List<Point> Points { get; set; }
        public List<Frequency> Frequencies { get; set; }
        public Source()
        {
            Points = new List<Point>();
            Frequencies = new List<Frequency>();
        }
        public Source(string line)
        {
            int n = 0;
            float f, ph;
            int x_, y_, z_;
            Points = new List<Point>();
            Frequencies = new List<Frequency>();
            string[] ars = line.Split(',');
            int nFreq = int.Parse(ars[0]);
            for (int i = 0; i < nFreq; i++)
            {
                f = float.Parse(ars[2 * i +1]);
                ph = float.Parse(ars[2 * i+ 2]);                
                Frequencies.Add(new Frequency() { Freq = f, Phase = ph });
                n += 2;
            }
            Console.WriteLine("After freq "+n.ToString());
            do
            {
                Console.WriteLine("{0} {1} {2}", n, n + 1, n + 2);
                x_ = int.Parse(ars[n+1]);
                y_ = int.Parse(ars[n+2]);
                z_ = int.Parse(ars[n+3]);
                Points.Add(new Point() { x = x_, y = y_, z = z_ });
                n += 3;
            } while (n <= ars.Length-3);

        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0},",Frequencies.Count));
            foreach (Frequency fr in Frequencies)
                sb.Append(string.Format("{0},{1},", fr.Freq, fr.Phase));
            if (Points.Count == 1)
                sb.Append(string.Format("{0},{1},{2}", Points[0].x, Points[0].y, Points[0].z));
            else
                if (Points.Count > 1)
            {                
                for(int i=0;i<Points.Count-1;i++)
                    sb.Append(string.Format("{0},{1},{2},", Points[i].x, Points[i].y, Points[i].z));
                sb.Append(string.Format("{0},{1},{2}", Points[Points.Count-1].x, Points[Points.Count - 1].y, Points[Points.Count - 1].z));
            }
            return sb.ToString();
        }
    }
}
