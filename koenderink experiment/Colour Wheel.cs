using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace koenderink_experiment
{
    public partial class Colour_Wheel : Form
    {
        int n;
        double[] hues, huesPredicted, huesExperimental;

        public Colour_Wheel(double[] hues, double[] huesPredicted, double[] huesExperimental, int n)
        {
            InitializeComponent();
            this.hues = hues;
            this.huesPredicted = huesPredicted;
            this.huesExperimental = huesExperimental;
            this.n = n;

            richTextBox1.Clear();
            richTextBox1.AppendText("| Inducer | predict | experiment |");
            for (int i = 0; i < n; i++)
            {
                richTextBox1.AppendText("\r\n| " + ((int)hues[i]).ToString() + " | " + ((int)huesPredicted[i]).ToString() + " | " + ((int)huesExperimental[i]).ToString() + " | ");
            }
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int cx = pictureBox1.Width / 2, cy = pictureBox1.Height / 2;
            
            int thickness = 50;
            var arcLength = n != 0 ? 360 / n : 1;

            int innerR = (pictureBox1.Width + pictureBox1.Height)/10;
            int outerR = innerR + thickness;
            var outerRect = new Rectangle(cx - outerR, cy - outerR, 2 * outerR, 2 * outerR);
            var innerRect = new Rectangle(cx - innerR, cy - innerR, 2 * innerR, 2 * innerR);

            int innerR2 = innerR - thickness;
            int outerR2 = innerR;
            var outerRect2 = new Rectangle(cx - outerR2, cy - outerR2, 2 * outerR2, 2 * outerR2);
            var innerRect2 = new Rectangle(cx - innerR2, cy - innerR2, 2 * innerR2, 2 * innerR2);

            int innerR3 = innerR2 - thickness;
            int outerR3 = innerR2;
            var outerRect3 = new Rectangle(cx - outerR3, cy - outerR3, 2 * outerR3, 2 * outerR3);
            var innerRect3 = new Rectangle(cx - innerR3, cy - innerR3, 2 * innerR3, 2 * innerR3);
            for (int i = 0; i < n; i++)
            {
                var colorHues = ColorFromHSV(hues[i], 100, 100);
                var colorHuesPredicted = ColorFromHSV(huesPredicted[i], 100, 100);
                var colorHuesExperimental = ColorFromHSV(huesExperimental[i], 100, 100);
                
                float startAngle = (float)hues[i];

                using (var p = new GraphicsPath())
                {
                    p.AddArc(outerRect, startAngle, arcLength);
                    p.AddArc(innerRect, startAngle + arcLength, -arcLength);
                    p.CloseFigure();
                    e.Graphics.FillPath(new SolidBrush(colorHues), p);
                    e.Graphics.DrawPath(new Pen(colorHues), p);
                }
                using (var p = new GraphicsPath())
                {
                    p.AddArc(outerRect2, startAngle, arcLength);
                    p.AddArc(innerRect2, startAngle + arcLength, -arcLength);
                    p.CloseFigure();
                    e.Graphics.FillPath(new SolidBrush(colorHuesPredicted), p);
                    e.Graphics.DrawPath(new Pen(colorHuesPredicted), p);
                }
                using (var p = new GraphicsPath())
                {
                    p.AddArc(outerRect3, startAngle, arcLength);
                    p.AddArc(innerRect3, startAngle + arcLength, -arcLength);
                    p.CloseFigure();
                    e.Graphics.FillPath(new SolidBrush(colorHuesExperimental), p);
                    e.Graphics.DrawPath(new Pen(colorHuesExperimental), p);
                }
            }
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
    }
}
