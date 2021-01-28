using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RayModelApp
{
    public partial class FrmSources : Form
    {
        public List<Frequency> frequencies = new List<Frequency>();
        public FrmSources()
        {
            InitializeComponent();
            string[] fs = Directory.GetFiles(Environment.CurrentDirectory, "*.src");
            listBox1.Items.Clear();
            foreach (string s in fs)
                listBox1.Items.Add(Path.GetFileName(s));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            frequencies.Clear();
            using (TextReader tr = File.OpenText(listBox1.SelectedItem.ToString()))
            {
                int i = 0;
                float f=-1, p=-1;
                frequencies.Clear();                
                string line;
                dataGridView1.DataSource = null;
                while ((line = tr.ReadLine()) != null)
                {
                    switch (i % 2) 
                    {
                        case 0:
                            f = float.Parse(line);
                            break;
                        case 1:
                            p=float.Parse(line);
                            frequencies.Add(new Frequency() { Freq = f, Phase = p });
                            break;
                    }
                    Console.WriteLine(string.Format("{0} {1}", line, i));
                    i++;
                }
                foreach (Frequency g in frequencies)
                    Console.WriteLine(g);
                dataGridView1.DataSource = frequencies;
            }
        }
    }
}
