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
    public partial class FrmAquatories : Form
    {
        public int AWidth;
        public int ALength;
        public int ADepth;
        public FrmAquatories()
        {
            InitializeComponent();
            string[] fs = Directory.GetFiles(Environment.CurrentDirectory, "*.akv");
            listBox1.Items.Clear();
            foreach (string s in fs)
                listBox1.Items.Add(Path.GetFileName(s));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (TextReader tr = File.OpenText(listBox1.SelectedItem.ToString()))
            {
                string line;                
                line = tr.ReadLine();
                string[] ss = line.Split(';');

                textBox1.Text = ss[0];
                textBox2.Text = ss[1];
                textBox3.Text = ss[2];
                AWidth = int.Parse(textBox1.Text);
                ALength = int.Parse(textBox2.Text); 
                ADepth = int.Parse(textBox3.Text);
            }
        }
    }
}
