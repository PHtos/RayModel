using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RayModelApp
{

    public class Sreda
    {
        [Description("Number of irregularity")]
        // public int NerCount { get; set; }
        public int NerCount
        {
            get { return irList.Count; }
        } 
        [Description("Number of sound profiles")]
        public int ProfileCount 
        {
            get 
            {
                return Profiles.Count;
            }
        }
        [Description("Number of sources")]
        public int SourceCount
        { 
            get { return Sources.Count; }
        }
        [Description("Length of water area")]
        public int Length { get; set; }
        [Description("Width of water area")]
        public int Width { get; set; }
        [Description("Depth of water area")]
        [DefaultValue(200)]
        private int depth;
        public int Depth 
        { get 
            { 
                return depth; 
            }
            set 
            {
                if ((value < 50 || value > 800))
                    MessageBox.Show("Depth is invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                {
                    depth = value;
                }
            }
        }
        [Description("Grid step")]
        public int Step { get; set; }
        [Description("Depth of receiver")]
        public int ReceiverDepth { get; set; }

        
       
        public List<Irregularity> irList { get; set; }
        public List<Source> Sources { get; set; }
        public List<Profile> Profiles { get; set; }

        public void Save(string filename)
        {
            using (TextWriter tw = File.CreateText(filename))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("{0},", NerCount));
                sb.Append(string.Format("{0},", ProfileCount));
                sb.Append(string.Format("{0},", SourceCount));
                sb.Append(string.Format("{0},", Length));
                sb.Append(string.Format("{0},", Width));
                sb.Append(string.Format("{0},", Depth));
                sb.Append(string.Format("{0},", Step));
                sb.Append(string.Format("{0}", ReceiverDepth));
                tw.WriteLine(sb.ToString());
                Console.WriteLine(irList.Count);
                foreach (Irregularity ir in irList)
                    tw.WriteLine(ir.ToString());
                foreach(Profile p in Profiles)
                    tw.WriteLine(p.ToString());
                foreach (Source src in Sources)
                    tw.WriteLine(src.ToString());
                tw.Close();
            }
        }

        public Sreda()
        {
            this.irList = new List<Irregularity>();
            this.Sources = new List<Source>();
            this.Profiles = new List<Profile>();
        }
        public void Load(string filename)
        {
            using (TextReader tr = File.OpenText(filename))
            {
                int nNer,nSources,nProf;
                string s = tr.ReadLine();
                string[] pars = s.Split(',');
                nNer = int.Parse(pars[0]);
                nProf = ushort.Parse(pars[1]);
                nSources = ushort.Parse(pars[2]);
                Length = int.Parse(pars[3]);
                Width = int.Parse(pars[4]);
                Depth = int.Parse(pars[5]);
                Step = int.Parse(pars[6]);
                ReceiverDepth = int.Parse(pars[7]);
                #region Irregularity
                irList.Clear();
                for (int i = 0; i < nNer; i++)
                    irList.Add(new Irregularity(tr.ReadLine()));
                #endregion
                #region Profiles
                Profiles.Clear();
                for (int i = 0; i < nProf; i++)
                    Profiles.Add(new Profile());
                #endregion
                #region Sources
                Sources.Clear();
                for (int i = 0; i < nSources; i++)
                    Sources.Add(new Source(tr.ReadLine()));
                #endregion
                tr.Close();
            }
        }

    }

    public class IrregularityCollectionEditor : CollectionEditor
    {
        public IrregularityCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(Irregularity);
        }
    }

    public class SourceCollectionEditor : CollectionEditor
    {
        public SourceCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(Source);
        }
    }

    public class ProfileCollectionEditor : CollectionEditor
    {
        public ProfileCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(Profile);
        }
    }
}
