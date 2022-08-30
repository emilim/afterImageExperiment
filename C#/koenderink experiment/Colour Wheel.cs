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

        private DataTable MakeNamesTable()
        {
            // Create a new DataTable titled 'Names.'
            DataTable namesTable = new DataTable("Names");

            // Add three column objects to the table.
            DataColumn idColumn = new DataColumn();
            idColumn.DataType = System.Type.GetType("System.Int32");
            idColumn.ColumnName = "id";
            idColumn.AutoIncrement = true;
            namesTable.Columns.Add(idColumn);

            DataColumn huesColumn = new DataColumn();
            huesColumn.DataType = System.Type.GetType("System.Int32");
            huesColumn.ColumnName = "hues";
            namesTable.Columns.Add(huesColumn);

            DataColumn huesPredictedColumn = new DataColumn();
            huesPredictedColumn.DataType = System.Type.GetType("System.Int32");
            huesPredictedColumn.ColumnName = "huesPredicted";
            namesTable.Columns.Add(huesPredictedColumn);

            DataColumn huesExperimental = new DataColumn();
            huesExperimental.DataType = System.Type.GetType("System.Int32");
            huesExperimental.ColumnName = "huesExperimental";
            namesTable.Columns.Add(huesExperimental);

            DataColumn done = new DataColumn();
            done.DataType = System.Type.GetType("System.Boolean");
            done.ColumnName = "done";
            namesTable.Columns.Add(done);

            // Create an array for DataColumn objects.
            DataColumn[] keys = new DataColumn[1];
            keys[0] = idColumn;
            namesTable.PrimaryKey = keys;

            // Return the new DataTable.
            return namesTable;
        }

        public Colour_Wheel(double[] hues, double[] huesPredicted, double[] huesExperimental, bool[] done, int n)
        {
            InitializeComponent();
            this.hues = hues;
            this.huesPredicted = huesPredicted;
            this.huesExperimental = huesExperimental;
            this.n = n-1;
            DataTable dt = MakeNamesTable();

            for (int i = 0; i < this.n; i++)
            {
                DataRow row;
                row = dt.NewRow();
                row["hues"] = (int)hues[i];
                row["huesPredicted"] = (int)huesPredicted[i];
                row["huesExperimental"] = (int)huesExperimental[i];
                row["done"] = done[i];
                dt.Rows.Add(row);
            }
            ResultGrid.DataSource = dt;
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int cx = pictureBox1.Width / 2, cy = pictureBox1.Height / 2;
            
            int thickness = 20;
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
