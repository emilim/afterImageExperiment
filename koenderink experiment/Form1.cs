using Microsoft.VisualBasic;
using System.Text;

namespace koenderink_experiment
{
    public partial class Form1 : Form
    {
        int x, y, radius;
        double h, s, v;
        double hA, sA, vA;
        int n, selected;
        double[] hues, huesPredicted, huesExperimental;
        double[,] lms, suppLMS, expLMS;

        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        int seconds;
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            t.Interval = 1000;
            t.Enabled = true;
            t.Tick += new EventHandler(timer1_Tick);
        }
        private void initialize()
        {
            s = 100;
            v = 100;
            sA = s;
            vA = v;
            n = Int32.Parse(Interaction.InputBox("How many afterimages you want to see?", "Experiment number sample", "10"));
            hues = new double[n + 1];
            huesPredicted = new double[n + 1];
            huesExperimental = new double[n + 1];

            lms = new double[3, n + 1];
            suppLMS = new double[3, n + 1];
            expLMS = new double[3, n + 1];

            int count = 0;
            for (double i = 0; i <= 360; i += (360.0 / n))
            {
                hues[count] = i;
                huesPredicted[count] = (i + 180) % 360;
                double[] tempLms = rgb2lms(ColorFromHSV(hues[count], s, v).R, ColorFromHSV(hues[count], s, v).G, ColorFromHSV(hues[count], s, v).B);
                lms[0, count] = tempLms[0];
                lms[1, count] = tempLms[1];
                lms[2, count] = tempLms[2];

                //suppLMS[0, count] = 255 - lms[0, count];
                //suppLMS[1, count] = 255 - lms[1, count];
                //suppLMS[2, count] = 255 - lms[0, count];
                double[] supplms = rgb2lms(ColorFromHSV(huesPredicted[count], sA, vA).R, ColorFromHSV(huesPredicted[count], sA, vA).G, ColorFromHSV(huesPredicted[count], sA, vA).B);
                suppLMS[0, count] = supplms[0];
                suppLMS[1, count] = supplms[1];
                suppLMS[2, count] = supplms[2];

                count++;
            }
            //int r = rnd.Next();
            //hues = hues.OrderBy(x => r).ToArray();
            //huesPredicted = huesPredicted.OrderBy(x => r).ToArray();
        }

        private void colourWheelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Colour_Wheel form2 = new Colour_Wheel(hues, huesPredicted, huesExperimental, n);
            form2.Tag = this;
            form2.Show(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void loadCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //open file dialog and load csv file 
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV Files|*.csv";
            ofd.Title = "Select a CSV File";
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                string[] lines = System.IO.File.ReadAllLines(ofd.FileName);
                string[] line = lines[0].Split(',');
                n = line.Length - 1;
                hues = new double[n + 1];
                huesPredicted = new double[n + 1];
                huesExperimental = new double[n + 1];
                lms = new double[3, n + 1];
                suppLMS = new double[3, n + 1];
                expLMS = new double[3, n + 1];
                int count = 0;
                for (int i = 1; i < lines.Length; i++)
                {
                    line = lines[i].Split(',');
                    hues[count] = double.Parse(line[0]);
                    huesPredicted[count] = double.Parse(line[1]);
                    huesExperimental[count] = double.Parse(line[2]);
                    lms[0, count] = double.Parse(line[3]);
                    lms[1, count] = double.Parse(line[4]);
                    lms[2, count] = double.Parse(line[5]);
                    suppLMS[0, count] = double.Parse(line[6]);
                    suppLMS[1, count] = double.Parse(line[7]);
                    suppLMS[2, count] = double.Parse(line[8]);
                    expLMS[0, count] = double.Parse(line[9]);
                    expLMS[1, count] = double.Parse(line[10]);
                    expLMS[2, count] = double.Parse(line[11]);
                    count++;
                }
                randomColor();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initialize();
            randomColor();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            radius = (Width + Height) / 7;
            x = Width / 3 - (radius / 2);
            y = Height / 2 - (radius / 2);

            if (seconds <= 10)
            {
                var image = ColorFromHSV(h, s, v);
                e.Graphics.FillEllipse(new SolidBrush(image), x, y, radius, radius);
            }

            if (seconds > 10)
            {
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, (byte)220, (byte)220, (byte)220)), x, y, radius, radius);

                var afterImage = new SolidBrush(ColorFromHSV(hA, sA, vA));
                e.Graphics.FillEllipse(afterImage, x + Width / 3, y, radius, radius);
            }
            e.Graphics.DrawLine(Pens.Black, Width / 2 - 20, Height / 2, Width / 2 + 20, Height / 2);
            e.Graphics.DrawLine(Pens.Black, Width / 2, Height / 2 - 20, Width / 2, Height / 2 + 20);

            using (Font myFont = new Font("Arial", 14))
            {
                e.Graphics.DrawString(seconds.ToString(), myFont, Brushes.Black, new Point(2, 2));
                //e.Graphics.DrawString(huesExperimental[selected - 1].ToString(), myFont, Brushes.Black, new Point(2, 50));
                e.Graphics.DrawString("Prova N: " + selected.ToString(), myFont, Brushes.Black, new Point(2, 30));
            }
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && seconds > 10)
            {
                hA -= 5;
            }
            if (e.KeyCode == Keys.Right && seconds > 10)
            {
                hA += 5;
            }
            if (e.KeyCode == Keys.Up && seconds > 10)
            {
                sA += 10;
            }
            if (e.KeyCode == Keys.Down && seconds > 10)
            {
                sA -= 10;
            }
            hA = hA % 360;
            sA = sA % 100 == 0 ? sA : sA % 100;

            if (e.KeyCode == Keys.Return && seconds > 10)
            {
                DrawGrid(pictureBox1.CreateGraphics());
                saveImage();
                randomColor();
            }
            if (e.KeyCode == Keys.Space)
            {
                seconds = 9;
            }
            pictureBox1.Invalidate();
        }
        private void DrawGrid(Graphics g)
        {
            //draw a grid of random coloured squares
            int x = 0;
            int y = 0;
            int w = pictureBox1.Width;
            int h = pictureBox1.Height;
            int size = 15;
            int count = 0;
            while (y < h)
            {
                x = 0;
                while (x < w)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256))), x, y, size, size);
                    x += size;
                    count++;
                }
                y += size;
            }
            Thread.Sleep(2000);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            seconds++;
            pictureBox1.Invalidate();
        }

        private double[] rgb2lms(double red, double green, double blue)
        {
            int N = 360;
            double[] L = new double[N];
            double[] M = new double[N];
            double[] S = new double[N];
            double[] R = new double[N];
            double[] G = new double[N];
            double[] B = new double[N];

            double[] InputSpectrum = new double[N];

            for (int i = 0; i < N; i++)
            {
                L[i] = 1.0 * Math.Exp(-((Math.Pow(i - 30, 2) / (2 * 150))));
                M[i] = 1.0 * Math.Exp(-((Math.Pow(i - 180, 2) / (2 * 350))));
                M[i] += 0.3 * Math.Exp(-((Math.Pow(i - 230, 2) / (2 * 350))));
                S[i] = 1.0 * Math.Exp(-((Math.Pow(i - 290, 2) / (2 * 650))));


                R[i] = red * Math.Exp(-((Math.Pow(i - 30, 2) / (2 * 150))));
                G[i] = green * Math.Exp(-((Math.Pow(i - 180, 2) / (2 * 350))));
                B[i] = blue * Math.Exp(-((Math.Pow(i - 290, 2) / (2 * 650))));

                InputSpectrum[i] = (R[i] + G[i] + B[i]);
            }

            // convolution

            double tempSumL = 0.0;
            double tempSumM = 0.0;
            double tempSumS = 0.0;

            double totL = 0.0;
            double totM = 0.0;
            double totS = 0.0;
            double totInput = 0.0;
            for (int i = 0; i < N; i++)
            {
                totL += L[i];
                totM += M[i];
                totS += S[i];
                totInput += InputSpectrum[i];

                tempSumL += InputSpectrum[i] * L[i];
                tempSumM += InputSpectrum[i] * M[i];
                tempSumS += InputSpectrum[i] * S[i];
            }

            double LFinal = tempSumL / totL;
            double MFinal = tempSumM / totM;
            double SFinal = tempSumS / totS;

            //LFinal = (int)(LFinal * 255);
            //MFinal = (int)(MFinal * 255);
            //SFinal = (int)(SFinal * 255);

            Console.WriteLine(LFinal + "     " + MFinal + "     " + SFinal);
            return new double[] { LFinal, MFinal, SFinal };
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            saturation /= 100;
            value /= 100;
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
        public void saveImage()
        {
            huesExperimental[selected - 1] = hA;

            double[] tempLms = rgb2lms(ColorFromHSV(hA, sA, vA).R, ColorFromHSV(hA, sA, vA).G, ColorFromHSV(hA, sA, vA).B);

            expLMS[0, selected - 1] = tempLms[0];
            expLMS[1, selected - 1] = tempLms[1];
            expLMS[2, selected - 1] = tempLms[2];
        }
        public void randomColor()
        {
            if (selected < n + 1)
            {
                seconds = 0;
                h = hues[selected];
                hA = huesPredicted[selected];
                selected++;
            }
            else { saveCSV(); }
        }
        public void saveCSV()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV|*.csv";
            saveFileDialog1.Title = "Save the experiment values!";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                var filePath = saveFileDialog1.FileName;

                var csv = new StringBuilder();
                csv.AppendLine("hue,predicted hue,experiment hue,long,medium,short,predL,predM,predS,expL,expM,expS");

                for (int i = 0; i < n; i++)
                {
                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", (int)hues[i], (int)huesPredicted[i], (int)huesExperimental[i], (int)lms[0, i], (int)lms[1, i], (int)lms[2, i], (int)suppLMS[0, i], (int)suppLMS[1, i], (int)suppLMS[2, i], (int)expLMS[0, i], (int)expLMS[1, i], (int)expLMS[2, i]);
                    csv.AppendLine(newLine);
                }

                File.WriteAllText(filePath, csv.ToString());
                //File.AppendAllText(filePath, csv.ToString());
            }
        }
        private void saveCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveCSV();
        }
    }
}