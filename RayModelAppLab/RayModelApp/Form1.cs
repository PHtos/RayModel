using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Media.Media3D;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using mc3vray;

namespace RayModelApp
{
    public partial class RayForm : Form
    {
        Characteristic Ch = new Characteristic();
        Sreda Sr = new Sreda();
        BindingList<Point4D> Points = new BindingList<Point4D>();
        List<Point3D> GraphPoints = new List<Point3D>();
        static ConcurrentQueue<Point4D> queue = new ConcurrentQueue<Point4D>();
        static List<Pnt> pnts = new List<Pnt>();
        static object locker = new object();
        Assembly asm;
        Type type;
        static MethodInfo method;
        static object obj;

        static int ErrPoint;

        double[] hobj = null;
        double[] lobj = null;
        double[] time = null;


        bool loaded = false;
        float x = 0;
        float y = 0;
        float z = -1f;
        int iProf = 0;
        public RayForm()
        {
            InitializeComponent();

            #region LoadRayModel

            asm = Assembly.LoadFrom("mc3vray.dll");
            type = asm.GetType("mc3vray.GObject", true, true);
            obj = Activator.CreateInstance(type);
            method = type.GetMethod("calcAmp");

            #endregion

            Points.Add(new Point4D() { X = 0.1, Y = 0.1, Z = 0, W = 0.4 });
            Points.Add(new Point4D() { X = 0.1, Y = -0.1, Z = 0, W = 0.5 });
            Points.Add(new Point4D() { X = -0.1, Y = -0.1, Z = 0, W = 0.2 });
            Points.Add(new Point4D() { X = -0.1, Y = 0.1, Z = 0, W = 0 });

            glC.MouseWheel += OnMouseWheel;
            dgw.AutoGenerateColumns = false;
            dgw.Columns.Add("X", "X");
            dgw.Columns.Add("Y", "Y");
            dgw.Columns.Add("Z", "Z");
            dgw.Columns.Add("V", "W");
            dgw.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgw.AllowUserToAddRows = true;
            foreach (DataGridViewTextBoxColumn c in dgw.Columns)
                c.Width = 60;
            dgw.Columns["X"].DataPropertyName = "X";
            dgw.Columns["Y"].DataPropertyName = "Y";
            dgw.Columns["Z"].DataPropertyName = "Z";
            dgw.Columns["V"].DataPropertyName = "W";

            dgw.DataSource = Points;
            Ch.Width = 200;
            Ch.Length = 200;
            Ch.Depth = 500;
            Ch.Up = 0.9;
            Ch.Bottom = 0.7;
            pg1.SelectedObject = Ch;
            pg1.SelectedObject = Sr;
            ChangeDescriptionHeight(pg1, 150);

            ofd.InitialDirectory = Environment.CurrentDirectory;
            ofd.Filter = "Environment file|*.env|Trajectory file|*.tr";
            sfd.InitialDirectory = ofd.InitialDirectory;
            sfd.Filter = ofd.Filter;
        }


        private static void ChangeDescriptionHeight(PropertyGrid grid, int height)
        {
            if (grid == null) throw new ArgumentNullException("grid");

            foreach (Control control in grid.Controls)
                if (control.GetType().Name == "DocComment")
                {
                    FieldInfo fieldInfo = control.GetType().BaseType.GetField("userSized",
                      BindingFlags.Instance |
                      BindingFlags.NonPublic);
                    fieldInfo.SetValue(control, true);
                    control.Height = height;
                    return;
                }
        }

        private void glC_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.Black);
            SetupViewport();
        }

        private void SetupViewport()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            int w = glC.Width;
            int h = glC.Height;
            float orthoW = w * (z + 1);
            float orthoH = h * (z + 1);
            GL.Ortho(0, orthoW, 0, orthoH, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        private void glC_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) // Play nice
                return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Translate(x, y, z); // position triangle according to our x variable

            GL.Color3(Color.SkyBlue);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(Ch.Length / 2, Ch.Width / 2);
            GL.Vertex2(Ch.Length / 2, -Ch.Width / 2);
            GL.Vertex2(-Ch.Length / 2, -Ch.Width / 2);
            GL.Vertex2(-Ch.Length / 2, Ch.Width / 2);
            GL.End();
            GL.Color3(Color.Gray);
            GL.Begin(PrimitiveType.Lines);
            for (int i = 0; i < 10; i++)
            {
                GL.Vertex2(i * 10, Ch.Width / 2);
                GL.Vertex2(i * 10, -Ch.Width / 2);
                GL.Vertex2(-i * 10, Ch.Width / 2);
                GL.Vertex2(-i * 10, -Ch.Width / 2);
            }
            for (int i = 0; i < 10; i++)
            {
                GL.Vertex2(Ch.Length / 2, i * 10);
                GL.Vertex2(-Ch.Length / 2, i * 10);
                GL.Vertex2(Ch.Length / 2, -i * 10);
                GL.Vertex2(-Ch.Length / 2, -i * 10);
            }
            GL.End();
            #region Pautinka
            GL.Color3(Color.Red);
            double r = Math.Max(Ch.Width, Ch.Length);
            GL.Begin(PrimitiveType.Lines);
            for (int i = 0; i < 12; i++)
            {
                GL.Vertex2(0, 0);
                GL.Vertex2(r * Math.Cos(i * Math.PI / 6) / 2.0, r * Math.Sin(i * Math.PI / 6) / 2.0);
            }

            #endregion
            GL.Color3(Color.Blue);
            /* for (int i = 0; i < Points.Count - 1; i++)
             {
                 GL.Vertex3(Points[i].X, Points[i].Y, Points[i].Z);
                 GL.Vertex3(Points[i + 1].X, Points[i + 1].Y, Points[i + 1].Z);
             }
             GL.End(); */
            if (GraphPoints.Count > 0)
            {
                GL.Color3(Color.Magenta);
                GL.PointSize(5.0f);
                GL.Begin(PrimitiveType.Points);
                foreach (Point3D p in GraphPoints)
                    GL.Vertex3(p.X, p.Y, 0);

                GL.End();
            }
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.DarkBlue);
            if (Sr.Sources.Count > 0)
                foreach (Source src in Sr.Sources)
                {
                    Console.WriteLine("Draw source");
                    for (int i = 0; i < src.Points.Count - 1; i++)
                    {
                        GL.Vertex2(src.Points[i].x, src.Points[i].y);
                        GL.Vertex2(src.Points[i + 1].x, src.Points[i + 1].y);
                    }
                }
            GL.End();

            glC.SwapBuffers();
        }

        private void glC_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            SetupViewport();
        }
        private void dgw_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            Point4D p = new Point4D();
            var oldValue = dgw[e.ColumnIndex, e.RowIndex].Value;
            var newValue = e.FormattedValue;
            Console.WriteLine(string.Format("{0} {1} {2}", "CellValidating", oldValue, newValue));
            switch (e.ColumnIndex)
            {
                case 0:
                    p = Points[e.RowIndex];
                    p.X = double.Parse(newValue.ToString());
                    Points[e.RowIndex] = p;
                    break;
                case 1:
                    p = Points[e.RowIndex];
                    p.Y = double.Parse(newValue.ToString());
                    Points[e.RowIndex] = p;
                    break;
                case 2:
                    p = Points[e.RowIndex];
                    p.Z = double.Parse(newValue.ToString());
                    Points[e.RowIndex] = p;
                    break;
                case 3:
                    p = Points[e.RowIndex];
                    p.W = double.Parse(newValue.ToString());
                    Points[e.RowIndex] = p;
                    break;

            }
            glC.Refresh();
            dgw.Refresh();
        }
        private void SetupCursorXYZ()
        {
            x = PointToClient(Cursor.Position).X * (z + 1);
            y = (-PointToClient(Cursor.Position).Y + glC.Height) * (z + 1);
        }
        private void glC_MouseMove(object sender, MouseEventArgs e)
        {
            double x1 = PointToClient(Cursor.Position).X * (z + 1);
            double y1 = (-PointToClient(Cursor.Position).Y + glC.Height) * (z + 1);
            stCoord.Text = string.Format("X={0} Y={1}", x1, y1);

            if (e.Button == MouseButtons.Right)
            {
                x = PointToClient(Cursor.Position).X * (z + 1);
                y = (-PointToClient(Cursor.Position).Y + glC.Height) * (z + 1);
                SetupCursorXYZ();
            }
            glC.Invalidate();
        }
        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0 && z > -1) z -= 0.0005f;
            if (e.Delta < 0 && z < 1) z += 0.0005f;
            SetupCursorXYZ();
            SetupViewport();
            glC.Invalidate();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Points.Remove(Points[dgw.CurrentRow.Index]);
            glC.Refresh();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Points.Add(new Point4D());
            dgw.Refresh();
            dgw.DataSource = Points;
            glC.Refresh();
        }

        private void pg1_Validating(object sender, CancelEventArgs e)
        {
            Console.WriteLine("validating");
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (sfd.FilterIndex)
                {
                    case 0:
                        using (TextWriter tw = File.CreateText(sfd.FileName))
                        {
                            foreach (Point4D p in Points)
                                tw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", p.X, p.Y, p.Z, p.W));
                        }
                        break;
                    case 1:
                        Sr.Save(sfd.FileName);
                        break;

                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                switch (ofd.FilterIndex)
                {
                    case 0:
                        Points.Clear();
                        using (TextReader tr = File.OpenText(ofd.FileName))
                        {
                            string line;
                            double px, py, pz, pw;
                            while ((line = tr.ReadLine()) != null)
                            {
                                Console.WriteLine(line);
                                string[] vals = line.Split('\t');
                                double.TryParse(vals[0], out px);
                                double.TryParse(vals[1], out py);
                                double.TryParse(vals[2], out pz);
                                double.TryParse(vals[3], out pw);
                                Points.Add(new Point4D() { X = px, Y = py, Z = pz, W = pw });
                            }
                        }
                        break;
                    case 1:
                        Sr.Load(ofd.FileName);
                        pg1.SelectedObject = Sr;
                        break;

                }
                glC.Invalidate();
            }
        }

        private double getDist(Point4D a, Point4D b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
        }

        private void btnMakePoints_Click(object sender, EventArgs e)
        {
            Point4D p4d;
            double dt, dx, dy, dz, px, py, pz;
            double path, len, currentTime;
            double.TryParse(cbTR.Text, out dt);
            GraphPoints.Clear();
            while (!queue.IsEmpty)
                queue.TryDequeue(out p4d);
            chart1.Series["sDist"].Points.Clear();
            chart1.Series["sDepth"].Points.Clear();
            currentTime = 0;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                path = getDist(Points[i], Points[i + 1]);
                px = Points[i + 1].X - Points[i].X;
                py = Points[i + 1].Y - Points[i].Y;
                pz = Points[i + 1].Z - Points[i].Z;

                dx = Points[i].W * px / Math.Sqrt(px * px + py * py + pz * pz);
                dy = Points[i].W * py / Math.Sqrt(px * px + py * py + pz * pz);
                dz = Points[i].W * pz / Math.Sqrt(px * px + py * py + pz * pz);

                px = Points[i].X;
                py = Points[i].Y;
                pz = Points[i].Z;
                GraphPoints.Add(new Point3D() { X = px, Y = py, Z = pz });
                queue.Enqueue(new Point4D() { X = px, Y = py, Z = pz, W = currentTime });
                chart1.Series["sDist"].Points.Add(Math.Sqrt(px * px + py * py));
                chart1.Series["sDepth"].Points.Add(pz);
                len = 0;
                currentTime += dt;
                do
                {
                    px += dt * dx;
                    py += dt * dy;
                    pz += dt * dz;
                    Console.WriteLine(string.Format("{0} {1} {2}", px, py, pz));
                    GraphPoints.Add(new Point3D() { X = px, Y = py, Z = pz });
                    queue.Enqueue(new Point4D() { X = px, Y = py, Z = pz, W = currentTime });
                    chart1.Series["sDist"].Points.Add(Math.Sqrt(px * px + py * py));
                    chart1.Series["sDepth"].Points.Add(pz);
                    len += dt;
                    currentTime += dt;
                } while (len < path / Points[i].W);
            }
            Console.WriteLine(GraphPoints.Count);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stCoord.Text = string.Format("Capacity {0}", GraphPoints.Count);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            iProf--;
            if (iProf < 0)
                iProf = Sr.Profiles.Count - 1;
            chart2.Series[0].Points.Clear();
            for (int i = 0; i < Sr.Profiles[iProf].Points.Count; i++)
                chart2.Series[0].Points.AddXY(Sr.Profiles[iProf].Points[i].c, Sr.Profiles[iProf].Points[i].z);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            iProf++;
            if (iProf > Sr.Profiles.Count - 1)
                iProf = 0;
            chart2.Series[0].Points.Clear();
            for (int i = 0; i < Sr.Profiles[iProf].Points.Count; i++)
                chart2.Series[0].Points.AddXY(Sr.Profiles[iProf].Points[i].c, Sr.Profiles[iProf].Points[i].z);
        }

        private void cmiAquatories_Click(object sender, EventArgs e)
        {
            using (FrmAquatories f = new FrmAquatories())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    Sr.Width = f.AWidth;
                    Sr.Depth = f.ADepth;
                    Sr.Length = f.ALength;
                    pg1.SelectedObject = Sr;
                }
            }
        }

        private void cmiObjects_Click(object sender, EventArgs e)
        {
            using (FrmSources f = new FrmSources())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    if (Sr.Sources.Count == 0)
                    {
                        Sr.Sources.Add(new Source());
                        foreach (Frequency fr in f.frequencies)
                        {
                            Sr.Sources[0].Frequencies.Add(fr);
                        }
                    }
                }
            }
        }

        private void cmiTraectories_Click(object sender, EventArgs e)
        {
            using (FrmTraectories f = new FrmTraectories())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    dgw.DataSource = null;
                    Points.Clear();
                    if (Sr.Sources.Count == 0)
                    {
                        Sr.Sources.Add(new Source());
                        Sr.Sources[0].Points.Clear();
                        foreach (var p in f.points)
                        {
                            Sr.Sources[0].Points.Add(new Point()
                            {
                                x = (int)p.X,
                                y = (int)p.Y,
                                z = (int)p.Z
                            });
                            Points.Add(new Point4D() { X = p.X, Y = p.Y, Z = p.Z, W = 0 });
                        }
                    }
                    dgw.DataSource = Points;
                }
            }
        }

        private void cmiProfiles_Click(object sender, EventArgs e)
        {
            using (FrmProfiles f = new FrmProfiles())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    Sr.Profiles.Add(f.p);
                }
            }
        }

        private void btnSequential_Click(object sender, EventArgs e)
        {
            double dt, dx, dy, dz, px, py, pz;
            double path, len;
            double.TryParse(cbTR.Text, out dt);
            GraphPoints.Clear();
            chart1.Series["sDist"].Points.Clear();
            chart1.Series["sDepth"].Points.Clear();
            for (int i = 0; i < Points.Count - 1; i++)
            {
                path = getDist(Points[i], Points[i + 1]);
                px = Points[i + 1].X - Points[i].X;
                py = Points[i + 1].Y - Points[i].Y;
                pz = Points[i + 1].Z - Points[i].Z;

                dx = Points[i].W * px / Math.Sqrt(px * px + py * py + pz * pz);
                dy = Points[i].W * py / Math.Sqrt(px * px + py * py + pz * pz);
                dz = Points[i].W * pz / Math.Sqrt(px * px + py * py + pz * pz);

                px = Points[i].X;
                py = Points[i].Y;
                pz = Points[i].Z;
                GraphPoints.Add(new Point3D() { X = px, Y = py, Z = pz });
                //chart1.Series["sDist"].Points.Add(Math.Sqrt(px * px + py * py));
                //chart1.Series["sDepth"].Points.Add(pz);
                len = 0;
                do
                {
                    px += dt * dx;
                    py += dt * dy;
                    pz += dt * dz;
                    Console.WriteLine(string.Format("{0} {1} {2}", px, py, pz));
                    GraphPoints.Add(new Point3D() { X = px, Y = py, Z = pz });
                    //chart1.Series["sDist"].Points.Add(Math.Sqrt(px * px + py * py));
                    //chart1.Series["sDepth"].Points.Add(pz);
                    len += dt;
                } while (len < path / Points[i].W);
            }
            //

            //double[] c = { 1545, 1540, 1550, 1565 };
            //double[] h = { 0, 40, 70, 100 };

            double[] c = Sr.Profiles[0].Points.Select(p => (double)p.c).ToArray();
            double[] h = Sr.Profiles[0].Points.Select(p => (double)p.z).ToArray();
            double hgas = Sr.ReceiverDepth;
            double hobj = 55;
            double lobj = 1000;
            double Ksrf = 0.9;
            double Kbtm = 0.7;
            double omega = 37;
            //
            Console.WriteLine(string.Format("Number points for calculation {0}", GraphPoints.Count));
            long ellapledTicks;
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            for (int i = 0; i < GraphPoints.Count; i++)
            {
                ellapledTicks = DateTime.Now.Ticks;
                hobj = GraphPoints[i].Z;
                lobj = Math.Sqrt(Math.Pow(GraphPoints[i].X, 2.0) + Math.Pow(GraphPoints[i].Y, 2.0));
                //object result = method.Invoke(obj, new object[] { hgas, hobj, lobj, Ksrf, Kbtm, omega, c, h });
                object result = method.Invoke(obj, new object[] { hgas, hobj, lobj, Ksrf, Kbtm, omega, c, h });
               /* Console.WriteLine(string.Format("{0} {1}", i, (result as GObject).ampry[0]));
                chart1.Series["sDist"].Points.Add((result as GObject).ampry[0]);
                ellapledTicks = DateTime.Now.Ticks - ellapledTicks;
                chart1.Series["sDepth"].Points.Add(ellapledTicks);*/
            }
            sWatch.Stop();
            MessageBox.Show(string.Format("Points {0} by {1} sec.", GraphPoints.Count, sWatch.ElapsedMilliseconds / 1000.0));
        }

        static void Calc()
        {
            double hgas;
            double hobj;
            double lobj;
            Point4D p4d;
            GObject go = new GObject();
            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out p4d);
                hobj = p4d.Z;
                hgas = 90;
                double Ksrf = 0.9;
                double Kbtm = 0.7;
                double omega = 37;
                double[] c = { 1545, 1540, 1550, 1565 };
                double[] h = { 0, 40, 70, 100 };
                lobj = Math.Sqrt(Math.Pow(p4d.X, 2.0) + Math.Pow(p4d.Y, 2.0));

                if (method != null)
                {

                    ///object result = method.Invoke(obj, new object[] { hgas, hobj, lobj, Ksrf, Kbtm, omega, c, h });
                    //object result = go.calcAmp(hgas, hobj, lobj, Ksrf, Kbtm, omega, c, h);
                    double[] tR;
                    double[] aR;
                    go.calcAmp(hgas, hobj, lobj, Ksrf, Kbtm, omega, c, h, out tR, out aR);
                    lock (locker)
                    {
                        //pnts.Add(new Pnt() { x = p4d.W, y = (double)result });
                        try
                        {                            
                            Console.WriteLine(string.Format("amp: {0}", aR[0]));
                            pnts.Add(new Pnt() { x = p4d.W, y = aR[0] });
                            Application.OpenForms[0].Text = queue.Count.ToString();
                        }
                        catch (Exception ex)
                        {
                            ErrPoint++;
                        }
                    }
                }
            }
        }
    
       
        private void btnParallel_Click(object sender, EventArgs e)
        {
            ConcurrentQueue<Point3D> query = new ConcurrentQueue<Point3D>();
            double dt, dx, dy, dz, px, py, pz;
            double path, len;
            double.TryParse(cbTR.Text, out dt);
            chart1.Series["sDist"].Points.Clear();
            chart1.Series["sDepth"].Points.Clear();
            pnts.Clear();
            for (int i = 0; i < Points.Count - 1; i++)
            {
                path = getDist(Points[i], Points[i + 1]);
                px = Points[i + 1].X - Points[i].X;
                py = Points[i + 1].Y - Points[i].Y;
                pz = Points[i + 1].Z - Points[i].Z;

                dx = Points[i].W * px / Math.Sqrt(px * px + py * py + pz * pz);
                dy = Points[i].W * py / Math.Sqrt(px * px + py * py + pz * pz);
                dz = Points[i].W * pz / Math.Sqrt(px * px + py * py + pz * pz);

                px = Points[i].X;
                py = Points[i].Y;
                pz = Points[i].Z;
                query.Enqueue(new Point3D() { X = px, Y = py, Z = pz });
                //chart1.Series["sDist"].Points.Add(Math.Sqrt(px * px + py * py));
                //chart1.Series["sDepth"].Points.Add(pz);
                len = 0;
                do
                {
                    px += dt * dx;
                    py += dt * dy;
                    pz += dt * dz;
                    Console.WriteLine(string.Format("{0} {1} {2}", px, py, pz));
                    query.Enqueue(new Point3D() { X = px, Y = py, Z = pz });
                    //chart1.Series["sDist"].Points.Add(Math.Sqrt(px * px + py * py));
                    //chart1.Series["sDepth"].Points.Add(pz);
                    len += dt;
                } while (len < path / Points[i].W);
            }
            //
            Console.WriteLine(string.Format("Queue :: Number points for calculation {0}", query.Count));
            Task[] tasks = new Task[16];
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ErrPoint = 0;
            for (int i = 0; i < 16; i++)
            {
                tasks[i] = new Task(Calc);
                tasks[i].Start();
            }
            Task.WaitAll(tasks);
            sw.Stop();
            Text = string.Format("Parallel {0} sec Count={1}", sw.ElapsedMilliseconds / 1000.0, pnts.Count);
            MessageBox.Show(string.Format("Points {0} by {1} sec.", GraphPoints.Count, sw.ElapsedMilliseconds / 1000.0));
            MessageBox.Show(string.Format("Error Points {0}", ErrPoint));          
            chart1.Series["sDist"].Points.Clear();
            var ps = pnts.OrderBy(p => p.x);
            foreach (Pnt p in ps)
                chart1.Series["sDist"].Points.AddXY(p.x, p.y);            
        }

        private void bArtem_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            double[] c = { 1545, 1540, 1550, 1565 };
            double[] h = { 0, 40, 70, 100 };
            double hgas = 80;
            double dt;
            double.TryParse(cbTR.Text, out dt);
            Console.WriteLine(GraphPoints.Count);
            hobj = new double[GraphPoints.Count];
            lobj = new double[GraphPoints.Count];
            time = new double[GraphPoints.Count];
            
            for (int i = 0; i < GraphPoints.Count; i++)
            {
                hobj[i] = GraphPoints[i].Z;
                lobj[i] = Math.Sqrt(Math.Pow(GraphPoints[i].X,2.0) + Math.Pow(GraphPoints[i].Y, 2.0));
                time[i] = i * dt;
            }

            double Ksrf = 0.9;
            double Kbtm = 0.7;
            double omega = 3.7;
            mc3vray.GObject gObj = new GObject();
            List<double> timeRays = new List<double>();
            List<double> ampRays = new List<double>();
            for (int i = 0; i < 1401; i++)
            {
                double[] tR;
                double[] aR;
                gObj.calcAmp(hgas, hobj[i], lobj[i], Ksrf, Kbtm, omega, c, h, out tR, out aR);
                for (int j = 0; j < tR.Length; j++)
                {
                    timeRays.Add(tR[j] + time[i]);
                    ampRays.Add(aR[j]);
                }
            }
            for (int minT = 0; minT < timeRays.Count - 1; minT++)
            {
                Console.WriteLine(string.Format("minT = {0}", minT));
                for (int j = minT + 1; j < timeRays.Count; j++)
                {
                    if (timeRays[minT] > timeRays[j])
                    {
                        double rT = timeRays[minT];
                        timeRays[minT] = timeRays[j];
                        timeRays[j] = rT;
                        rT = ampRays[minT];
                        ampRays[minT] = ampRays[j];
                        ampRays[j] = rT;
                    }
                }
            }
            List<double> resTime = new List<double>();
            List<double> resAmp = new List<double>();
            for (int i = 0; i < timeRays.Count; i++)
            {
                Console.WriteLine(string.Format("timeRays = {0}", i));
                if (i > 0 && timeRays[i - 1] == timeRays[i])
                    continue;
                double amp0 = 0;
                for (int k = i; k < timeRays.Count; k++)
                {
                    if (timeRays[k] == timeRays[i])
                    {
                        amp0 += ampRays[k] * Math.Sin(2 * Math.PI * omega * timeRays[k]);
                        resTime.Add(timeRays[k]);
                        resAmp.Add(ampRays[k]);
                    }
                }
            }
            for (int i = 0; i < resTime.Count; i++)
                chart1.Series[0].Points.AddXY(resTime[i], resAmp[i]);

        }
    }

    public struct Pnt
    {
        public double x;
        public double y;
    }
}
