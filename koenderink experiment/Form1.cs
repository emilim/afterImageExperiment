using Microsoft.VisualBasic;

namespace koenderink_experiment
{
    public partial class Form1 : Form
    {
        int x, y, radius;
        double h, s, v;
        double hA, sA, vA;
        int n, selected;
        double[] hues, huesPredicted, huesExperimental;

        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        int seconds;
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            t.Interval = 1000;
            t.Enabled = true;
            t.Tick += new EventHandler(timer1_Tick);

            s = 100;
            v = 100;
            sA = s;
            vA = v;

            n = Int32.Parse(Interaction.InputBox("How many afterimages you want to see?", "Experiment number sample", "10"));
            hues = new double[n+1];
            huesPredicted = new double[n+1];
            huesExperimental = new double[n+1];
            int count = 0;
            for (double i = 0; i <= 360; i += (360.0 / n))
            {
                hues[count] = i;
                huesPredicted[count] = (i+180)%360;
                count++;
            }
            int r = rnd.Next();
            hues = hues.OrderBy(x => r).ToArray();
            huesPredicted = huesPredicted.OrderBy(x => r).ToArray();

            randomColor();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            radius = (Width + Height) / 7;
            x = Width / 3 - (radius/2);
            y = Height / 2 - (radius/2);

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
                e.Graphics.DrawString("Prova N: "+selected.ToString(), myFont, Brushes.Black, new Point(2, 30));

                e.Graphics.DrawString("Inducer, predict, experiment", myFont, Brushes.Black, new Point(2, 70));
                for (int i = 0; i < n; i++)
                {
                    e.Graphics.DrawString(((int)hues[i]).ToString(), myFont, Brushes.Black, new Point(2, 100 + i * 20));
                    e.Graphics.DrawString(((int)huesPredicted[i]).ToString(), myFont, Brushes.Black, new Point(90, 100 + i * 20));
                    e.Graphics.DrawString(((int)huesExperimental[i]).ToString(), myFont, Brushes.Black, new Point(180, 100 + i * 20));
                }
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
            sA = sA % 100;
            
            if (e.KeyCode == Keys.Return && seconds > 10)
            {
                saveImage();
                randomColor();
            }
            if (e.KeyCode == Keys.Space)
            {
                seconds = 9;
            }
            pictureBox1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            seconds++;
            pictureBox1.Invalidate();
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
        }
        public void randomColor()
        {
            if (selected < n+1)
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
            MessageBox.Show("saved");
        }
    }
}