﻿using ContourAnalysisNS;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TCP_LISTENER_Delta
{
    public partial class ShowContoursForm : Form
    {
        Templates templates;
        Templates samples;
        public Template selectedTemplate;
        Bitmap bmp;

        public ShowContoursForm(Templates templates, Templates samples, Image<Bgr, Byte> image)
        {
            if (image == null)
                return;
            InitializeComponent();
            this.templates = templates;
            this.samples = samples;

            this.samples = new Templates();
            foreach (var sample in samples)
                this.samples.Add(sample);

            dgvContours.RowCount = samples.Count;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            //some magic
            string fileName = Path.GetTempPath() + "\\temp.bmp";
            image.Save(fileName);
            bmp = (Bitmap)Image.FromFile(fileName);
        }

        private void dgvContours_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
            e.Handled = true;

            if (e.RowIndex < 0) return;

            Template template = samples[e.RowIndex];

            if (e.ColumnIndex == -1)
            {
                e.Graphics.DrawString(e.RowIndex.ToString(), Font, Brushes.Black, e.CellBounds.Location);
                return;
            }

            if (e.ColumnIndex == 0)
            {
                var rect = new Rectangle(e.CellBounds.X, e.CellBounds.Y, (e.CellBounds.Width - 24) / 2, e.CellBounds.Height);
                rect.Inflate(-20, -20);
                Rectangle boundRect = template.contour.SourceBoundingRect;
                float k1 = 1f * rect.Width / boundRect.Width;
                float k2 = 1f * rect.Height / boundRect.Height;
                float k = Math.Min(k1, k2);

                e.Graphics.DrawImage(bmp,
                    new Rectangle(rect.X, rect.Y, (int)(boundRect.Width * k), (int)(boundRect.Height * k)),
                    boundRect, GraphicsUnit.Pixel);
            }

            if (e.ColumnIndex == 0)
            {
                template.Draw(e.Graphics, e.CellBounds);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tbTemplateName.Text == "<template name>")
                MessageBox.Show("Enter template name");
            else
                try
                {
                    int i = dgvContours.SelectedCells[0].RowIndex;
                    samples[i].name = tbTemplateName.Text;
                    templates.Add(samples[i]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        private void tbTemplateName_Enter(object sender, EventArgs e)
        {
            tbTemplateName.ForeColor = Color.Black;
            if (tbTemplateName.Text == "<template name>")
                tbTemplateName.Text = "";
        }
    }
}
