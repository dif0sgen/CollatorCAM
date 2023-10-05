using ContourAnalysisDemo;
using ContourAnalysisNS;
using EasyModbus;
using Emgu.CV;
using Emgu.CV.Structure;
//using Emgu.CV.OCR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using TCP_LISTENER_Delta;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace CollatorCAM
{

    public partial class Form_Listener : Form
    {

        private Emgu.CV.Capture _capture;
        Image<Bgr, Byte> frame;
        ImageProcessor processor;
        Dictionary<string, System.Drawing.Image> AugmentedRealityImages = new Dictionary<string, System.Drawing.Image>();

        bool captureFromCam = true;
        int frameCount = 0;
        int oldFrameCount = 0;
        bool showAngle;
        int camWidth = 640;
        int camHeight = 480;
        string templateFile;

        ModbusClient modbus = new ModbusClient();
        System.Timers.Timer aTimer = new System.Timers.Timer();
        System.Timers.Timer bTimer = new System.Timers.Timer();

        // Set up the controls and events to be used and update the device list.

        public delegate void InvokeDelegate();
        private Thread thread2;

        string ImagePath = "Please select image";
        string GetImagePath;
        public int ACC_X_MAN;
        
        int i;
        int month;
        int monthph;
        bool PhotoMonth;
        private int imageIndex;
        private string[] files;


        /// 
        /// Init Form
        /// 
        public Form_Listener()
        {
            this.InitializeComponent();
            //create image preocessor
            processor = new ContourAnalysisNS.ImageProcessor();
            //load default templates
            templateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";
            LoadTemplates(templateFile);
            //start capture from cam
            StartCapture();
            //apply settings
            //ApplySettings();
            //
            System.Windows.Forms.Application.Idle += new EventHandler(Application_Idle);
            thread2 = new Thread(() => WriteMDBS("WRITE"));
            thread2.Start();
            imageIndex = 0;
            this.Closing += new CancelEventHandler(this.Form_Listener_Close);
        }

        /// 
        /// Write data to box, from modbus
        /// 
        void WriteMDBS(string name)
        {
            while (true)
            {
                try
                {

                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                    {
                        label2.Text = "Current template file: " + templateFile;
                      //  ApplySettings();

                    }));
                    }
                    else
                    {
                        label2.Text = "Current template file: " + templateFile;

                        pictureBox2.Refresh();
                       // ApplySettings();
                    }
                }
                catch (Exception ex) when (ex.Source == "mscorlib")
                {
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Write trackbar from memory: " + ex.Message);
                }
            }
        }












        private void LoadTemplates(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    processor.templates = (Templates)new BinaryFormatter().Deserialize(fs);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveTemplates(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    new BinaryFormatter().Serialize(fs, processor.templates);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StartCapture()
        {
            try
            {
                _capture = new Emgu.CV.Capture();
                ApplyCamSettings();
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ApplyCamSettings()
        {
            try
            {
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, camWidth);
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, camHeight);
                cbCamResolution.Text = camWidth + "x" + camHeight;
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void Application_Idle(object sender, EventArgs e)
        {
            ProcessFrame();
        }

        private void ProcessFrame()
        {
            try
            {
                if (captureFromCam)
                    frame = _capture.QueryFrame();
                //frame = _capture.QueryFrame();
                frameCount++;
                //
                processor.ProcessImage(frame);
                //
                if (cbShowBinarized.Checked)
                    ibMain.Image = processor.binarizedFrame;
                else
                    ibMain.Image = frame;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void tmUpdateState_Tick(object sender, EventArgs e)
        {
            lbFPS.Text = (frameCount - oldFrameCount) + " fps";
            oldFrameCount = frameCount;
            if (processor.contours != null)
                lbContoursCount.Text = "Contours: " + processor.contours.Count;
            if (processor.foundTemplates != null)
                lbRecognized.Text = "Recognized contours: " + processor.foundTemplates.Count;
        }

        private void ibMain_Paint(object sender, PaintEventArgs e)
        {
                string TBtime = "";
                string TBmonth = "NG Month";
                string TByear = "NG Year";
                string TBmaori = "NG Maori";
                DateTime thisDay = DateTime.Now;
                TBtime = thisDay.ToString("d") + " " + thisDay.ToString("T");

                if (frame == null) return;

                Font font = new Font(Font.FontFamily, 24);//16

                e.Graphics.DrawString(lbFPS.Text, new Font(Font.FontFamily, 16), Brushes.Yellow, new PointF(1, 1));

                Brush bgBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 0));
                Brush foreBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
                Pen borderPen = new Pen(Color.FromArgb(150, 0, 255, 0));
                //
                if (cbShowContours.Checked)
                    foreach (var contour in processor.contours)
                        if (contour.Total > 1)
                            e.Graphics.DrawLines(Pens.Red, contour.ToArray());
                //
                lock (processor.foundTemplates)
                    foreach (FoundTemplateDesc found in processor.foundTemplates)
                    {
                        if (found.template.name.EndsWith(".png") || found.template.name.EndsWith(".jpg"))
                        {
                            DrawAugmentedReality(found, e.Graphics);
                            continue;
                        }

                        Rectangle foundRect = found.sample.contour.SourceBoundingRect;
                        Point p1 = new Point((foundRect.Left + foundRect.Right) / 2, foundRect.Top);
                        string text = "";
                        //FoundTemplateName = found.template.name;

                        if (found.template.name != "2024")
                            text = found.template.name;
                        if (found.template.name == "January" || found.template.name == "February" || found.template.name == "March" || found.template.name == "April")
                            TBmonth = "OK " + found.template.name;
                        if (found.template.name == "2024")
                        {
                            TByear = "OK " + found.template.name;
                            text = found.template.name;
                            Point point = new Point(foundRect.Right - 150, foundRect.Top + 100);

                            Rectangle maori = new Rectangle(foundRect.Right + 10, foundRect.Top, foundRect.Width * 2, foundRect.Height);
                            Point p2 = new Point((maori.Left + maori.Right) / 2, maori.Top);
                            //ProcessImage.GrayFrame
                            Bitmap bitmap = new Bitmap(foundRect.Width * 2, foundRect.Height);
                            Bitmap gray = (processor.binarizedFrame).Bitmap;
                            bitmap = gray.Clone(maori, gray.PixelFormat);
                            var sourceContours = new Image<Gray, Byte>(bitmap).FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
                            //cont = sourceContours;
                            string num = point.ToString();
                            e.Graphics.DrawString(num, font, new SolidBrush(Color.FromArgb(255, 0, 255, 0)), point);
                            e.Graphics.DrawRectangle(borderPen, maori);
                            if (sourceContours != null)
                            {
                                e.Graphics.DrawString("Maori", font, bgBrush, new PointF(p2.X + 1 - font.Height / 3, p2.Y + 1 - font.Height));
                                e.Graphics.DrawString("Maori", font, foreBrush, new PointF(p2.X - font.Height / 3, p2.Y - font.Height));
                                TBmaori = "OK Maori";
                            }
                            if (sourceContours == null)
                            {
                                e.Graphics.DrawString("Null", font, bgBrush, new PointF(p2.X + 1 - font.Height / 3, p2.Y + 1 - font.Height));
                                e.Graphics.DrawString("Null", font, foreBrush, new PointF(p2.X - font.Height / 3, p2.Y - font.Height));
                            }
                        }
                        if (showAngle)
                            text += string.Format("\r\nangle={0:000}°\r\nscale={1:0.0}", 180 * found.angle / Math.PI, found.scale);
                        e.Graphics.DrawRectangle(borderPen, foundRect);
                        e.Graphics.DrawString(text, font, bgBrush, new PointF(p1.X + 1 - font.Height / 3, p1.Y + 1 - font.Height));
                        e.Graphics.DrawString(text, font, foreBrush, new PointF(p1.X - font.Height / 3, p1.Y - font.Height));
                    }
                if (PhotoMonth == true && ibMain.Image == frame || ibMain.Image == processor.binarizedFrame)
                {
                    tbResult.Items.Add(TBtime);
                    tbResult.Items.Add(TBmonth);
                    tbResult.Items.Add(TByear);
                    tbResult.Items.Add(TBmaori);
                    PhotoMonth = false;
                    monthph++;
                    ScanCycle();
                }

        }

        private void DrawAugmentedReality(FoundTemplateDesc found, Graphics gr)
        {
            string fileName = Path.GetDirectoryName(templateFile) + "\\" + found.template.name;
            if (!AugmentedRealityImages.ContainsKey(fileName))
            {
                if (!File.Exists(fileName)) return;
                AugmentedRealityImages[fileName] = System.Drawing.Image.FromFile(fileName);
            }
            System.Drawing.Image img = AugmentedRealityImages[fileName];
            Point p = found.sample.contour.SourceBoundingRect.Center();
            var state = gr.Save();
            gr.TranslateTransform(p.X, p.Y);
            gr.RotateTransform((float)(180f * found.angle / Math.PI));
            gr.ScaleTransform((float)(found.scale), (float)(found.scale));
            gr.DrawImage(img, new Point(-img.Width / 2, -img.Height / 2));
            gr.Restore(state);
        }

        private void cbAutoContrast_CheckedChanged(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void ApplySettings()
        {
            try
            {
                processor.equalizeHist = cbAutoContrast.Checked;
                showAngle = cbShowAngle.Checked;
                captureFromCam = cbCaptureFromCam.Checked;
                btLoadImage.Enabled = !captureFromCam;
                btLoadFolder.Enabled = !captureFromCam;
                cbCamResolution.Enabled = captureFromCam;
                processor.finder.maxRotateAngle = cbAllowAngleMore45.Checked ? Math.PI : Math.PI / 4;
                processor.minContourArea = (int)nudMinContourArea.Value;
                processor.minContourLength = (int)nudMinContourLength.Value;
                processor.finder.maxACFDescriptorDeviation = (int)nudMaxACFdesc.Value;
                processor.finder.minACF = (double)nudMinACF.Value;
                processor.finder.minICF = (double)nudMinICF.Value;
                processor.blur = cbBlur.Checked;
                processor.noiseFilter = cbNoiseFilter.Checked;
                processor.cannyThreshold = (int)nudMinDefinition.Value;
                nudMinDefinition.Enabled = processor.noiseFilter;
                processor.adaptiveThresholdBlockSize = (int)nudAdaptiveThBlockSize.Value;
                processor.adaptiveThresholdParameter = cbAdaptiveNoiseFilter.Checked ? 1.5 : 0.5;
                if (cbCaptureFromCam.Checked)
                {
                    label3.Text = "Image from camera";
                    ImagePath = "Please select image";
                }
                else if (cbCaptureFromCam.Checked == false)
                    label3.Text = ImagePath;
                //cam resolution
                string[] parts = cbCamResolution.Text.ToLower().Split('x');
                if (parts.Length == 2)
                {
                    int camWidth = int.Parse(parts[0]);
                    int camHeight = int.Parse(parts[1]);
                    if (this.camHeight != camHeight || this.camWidth != camWidth)
                    {
                        this.camWidth = camWidth;
                        this.camHeight = camHeight;
                        ApplyCamSettings();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image|*.bmp;*.png;*.jpg;*.jpeg";
            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                try
                {
                    frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(ofd.FileName));
                    ImagePath = "Current image file: " + ofd.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }
        private void btLoadFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                try
                {
                    GetImagePath = ofd.SelectedPath;
                    files = Directory.GetFiles(GetImagePath);
                    i = files.Length;
                    frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i - 1]));
                    ImagePath = "Current image file: " + GetImagePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }
        private void btnPhoto_Click(object sender, EventArgs e)
        {
        monthph = 0;
        ScanCycle();
        }
        private void ScanCycle()
        {
            if (monthph == 0)
                ScanCycle0();
            if (monthph == 1)
                ScanCycle2();
            if (monthph == 2)
                ScanCycle3();
            if (monthph == 3)
                ScanCycle4();
            if (monthph == 4)
                ScanCycle5();
            if (monthph == 5)
                ScanCycle6();
            if (monthph == 6)
                monthph = 0;
        }
        private void ScanCycle0()
        {
            tbResult.Items.Add(" ");
            tbResult.Items.Add("                      SCAN           ");
            tbResult.Items.Add(" ");
            GetImagePath = Properties.Settings.Default.JanuaryGetImagePath;
            GetImage();
            monthph++;
            ScanCycle();
        }
            private void ScanCycle2()
        {
            if (Properties.Settings.Default.JanuaryAdaptiveThBlockSize == 0)
                Properties.Settings.Default.JanuaryAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.JanuaryMaxACF == 0)
                Properties.Settings.Default.JanuaryMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinContourArea == 0)
                Properties.Settings.Default.JanuaryMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinContourLength == 0)
                Properties.Settings.Default.JanuaryMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinDefinition == 0)
                Properties.Settings.Default.JanuaryMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinACF == 0)
                Properties.Settings.Default.JanuaryMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinICF == 0)
                Properties.Settings.Default.JanuaryMinICF = new decimal(new int[] { 2, 0, 0, 65536 });

            cbAutoContrast.Checked = Properties.Settings.Default.JanuaryAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.JanuaryBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.JanuaryNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.JanuaryAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.JanuaryCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.JanuaryCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.JanuaryNoizeFilt;
            templateFile = Properties.Settings.Default.JanuaryTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.JanuaryShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.JanuaryShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.JanuaryShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.JanuaryCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.JanuaryAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.JanuaryMaxACF;
            nudMinACF.Value = Properties.Settings.Default.JanuaryMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.JanuaryMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.JanuaryMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.JanuaryMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.JanuaryMinICF;
            LoadTemplates(templateFile);
            GetImagePath = Properties.Settings.Default.JanuaryGetImagePath;
            GetImage();
            PhotoMonth = true;
        }
        private void ScanCycle3()
        {
            if (Properties.Settings.Default.FebruaryAdaptiveThBlockSize == 0)
                Properties.Settings.Default.FebruaryAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.FebruaryMaxACF == 0)
                Properties.Settings.Default.FebruaryMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinContourArea == 0)
                Properties.Settings.Default.FebruaryMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinContourLength == 0)
                Properties.Settings.Default.FebruaryMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinDefinition == 0)
                Properties.Settings.Default.FebruaryMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinACF == 0)
                Properties.Settings.Default.FebruaryMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinICF == 0)
                Properties.Settings.Default.FebruaryMinICF = new decimal(new int[] { 2, 0, 0, 65536 });

            cbAutoContrast.Checked = Properties.Settings.Default.FebruaryAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.FebruaryBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.FebruaryNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.FebruaryAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.FebruaryCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.FebruaryCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.FebruaryNoizeFilt;
            templateFile = Properties.Settings.Default.FebruaryTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.FebruaryShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.FebruaryShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.FebruaryShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.FebruaryCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.FebruaryAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.FebruaryMaxACF;
            nudMinACF.Value = Properties.Settings.Default.FebruaryMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.FebruaryMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.FebruaryMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.FebruaryMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.FebruaryMinICF;
            LoadTemplates(templateFile);
            GetImagePath = Properties.Settings.Default.FebruaryGetImagePath;
            GetImage();
            PhotoMonth = true;
        }
        private void ScanCycle4()
        {
            if (Properties.Settings.Default.MarchAdaptiveThBlockSize == 0)
                Properties.Settings.Default.MarchAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.MarchMaxACF == 0)
                Properties.Settings.Default.MarchMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinContourArea == 0)
                Properties.Settings.Default.MarchMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinContourLength == 0)
                Properties.Settings.Default.MarchMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinDefinition == 0)
                Properties.Settings.Default.MarchMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinACF == 0)
                Properties.Settings.Default.MarchMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinICF == 0)
                Properties.Settings.Default.MarchMinICF = new decimal(new int[] { 2, 0, 0, 65536 });

            cbAutoContrast.Checked = Properties.Settings.Default.MarchAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.MarchBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.MarchNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.MarchAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.MarchCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.MarchCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.MarchNoizeFilt;
            templateFile = Properties.Settings.Default.MarchTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.MarchShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.MarchShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.MarchShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.MarchCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.MarchAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.MarchMaxACF;
            nudMinACF.Value = Properties.Settings.Default.MarchMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.MarchMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.MarchMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.MarchMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.MarchMinICF;
            LoadTemplates(templateFile);
            GetImagePath = Properties.Settings.Default.MarchGetImagePath;;
            GetImage();

            PhotoMonth = true;

        }
        private void ScanCycle5()
        {
            if (Properties.Settings.Default.AprilAdaptiveThBlockSize == 0)
                Properties.Settings.Default.AprilAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.AprilMaxACF == 0)
                Properties.Settings.Default.AprilMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinContourArea == 0)
                Properties.Settings.Default.AprilMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinContourLength == 0)
                Properties.Settings.Default.AprilMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinDefinition == 0)
                Properties.Settings.Default.AprilMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinACF == 0)
                Properties.Settings.Default.AprilMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinICF == 0)
                Properties.Settings.Default.AprilMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            
            cbAutoContrast.Checked = Properties.Settings.Default.AprilAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.AprilBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.AprilNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.AprilAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.AprilCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.AprilCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.AprilNoizeFilt;
            templateFile = Properties.Settings.Default.AprilTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.AprilShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.AprilShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.AprilShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.AprilCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.AprilAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.AprilMaxACF;
            nudMinACF.Value = Properties.Settings.Default.AprilMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.AprilMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.AprilMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.AprilMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.AprilMinICF;
            LoadTemplates(templateFile);
            GetImagePath = Properties.Settings.Default.AprilGetImagePath;
            GetImage();
            PhotoMonth = true;
        }
        private void ScanCycle6()
        {
            if (Properties.Settings.Default.MayAdaptiveThBlockSize == 0)
                Properties.Settings.Default.MayAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.MayMaxACF == 0)
                Properties.Settings.Default.MayMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinContourArea == 0)
                Properties.Settings.Default.MayMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinContourLength == 0)
                Properties.Settings.Default.MayMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinDefinition == 0)
                Properties.Settings.Default.MayMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinACF == 0)
                Properties.Settings.Default.MayMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinICF == 0)
                Properties.Settings.Default.MayMinICF = new decimal(new int[] { 2, 0, 0, 65536 });

            cbAutoContrast.Checked = Properties.Settings.Default.MayAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.MayBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.MayNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.MayAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.MayCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.MayCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.MayNoizeFilt;
            templateFile = Properties.Settings.Default.MayTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.MayShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.MayShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.MayShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.MayCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.MayAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.MayMaxACF;
            nudMinACF.Value = Properties.Settings.Default.MayMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.MayMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.MayMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.MayMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.MayMinICF;
            LoadTemplates(templateFile);
            GetImagePath = Properties.Settings.Default.MayGetImagePath;
            GetImage();
            PhotoMonth = true;
        }
        private void GetImage()
        {
            if (GetImagePath != null)
            {
                files = Directory.GetFiles(GetImagePath);
                i = files.Length;
                frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i - 1]));
                ImagePath = "Current image file: " + GetImagePath;
                ApplySettings();
            }
        }
        private void btCreateTemplate_Click(object sender, EventArgs e)
        {
            if (frame != null)
                new ShowContoursForm(processor.templates, processor.samples, frame).ShowDialog();
        }

        private void btNewTemplates_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to create new template database?", "Create new template database", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                processor.templates.Clear();
        }

        private void btOpenTemplates_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Templates(*.bin)|*.bin";
            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                templateFile = ofd.FileName;
                LoadTemplates(templateFile);
            }
        }

        private void btSaveTemplates_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Templates(*.bin)|*.bin";
            if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                templateFile = sfd.FileName;
                SaveTemplates(templateFile);
            }
        }

        private void btTemplateEditor_Click(object sender, EventArgs e)
        {
            new TemplateEditor(processor.templates).Show();
        }

        private void btAutoGenerate_Click(object sender, EventArgs e)
        {
            new AutoGenerateForm(processor).ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (month == 2)
            {
                Properties.Settings.Default.JanuaryAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.JanuaryBlur = cbBlur.Checked;
                Properties.Settings.Default.JanuaryNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.JanuaryAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.JanuaryCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.JanuaryCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.JanuaryNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.JanuaryShowContours = cbShowContours.Checked;
                Properties.Settings.Default.JanuaryTemplateFile = templateFile;
                Properties.Settings.Default.JanuaryShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.JanuaryShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.JanuaryCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.JanuaryAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.JanuaryMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.JanuaryMinACF = nudMinACF.Value;
                Properties.Settings.Default.JanuaryMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.JanuaryMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.JanuaryMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.JanuaryMinICF = nudMinICF.Value;
                Properties.Settings.Default.JanuaryGetImagePath = GetImagePath;


            }
            if (month == 3)
            {
                Properties.Settings.Default.FebruaryAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.FebruaryBlur = cbBlur.Checked;
                Properties.Settings.Default.FebruaryNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.FebruaryAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.FebruaryCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.FebruaryCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.FebruaryNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.FebruaryShowContours = cbShowContours.Checked;
                Properties.Settings.Default.FebruaryTemplateFile = templateFile;
                Properties.Settings.Default.FebruaryShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.FebruaryShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.FebruaryCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.FebruaryAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.FebruaryMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.FebruaryMinACF = nudMinACF.Value;
                Properties.Settings.Default.FebruaryMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.FebruaryMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.FebruaryMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.FebruaryMinICF = nudMinICF.Value;
                Properties.Settings.Default.FebruaryGetImagePath = GetImagePath;
            }
            if (month == 4)
            {
                Properties.Settings.Default.MarchAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.MarchBlur = cbBlur.Checked;
                Properties.Settings.Default.MarchNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.MarchAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.MarchCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.MarchCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.MarchNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.MarchShowContours = cbShowContours.Checked;
                Properties.Settings.Default.MarchTemplateFile = templateFile;
                Properties.Settings.Default.MarchShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.MarchShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.MarchCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.MarchAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.MarchMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.MarchMinACF = nudMinACF.Value;
                Properties.Settings.Default.MarchMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.MarchMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.MarchMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.MarchMinICF = nudMinICF.Value;
                Properties.Settings.Default.MarchGetImagePath = GetImagePath;
            }
            if (month == 5)
            {
                Properties.Settings.Default.AprilAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.AprilBlur = cbBlur.Checked;
                Properties.Settings.Default.AprilNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.AprilAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.AprilCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.AprilCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.AprilNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.AprilShowContours = cbShowContours.Checked;
                Properties.Settings.Default.AprilTemplateFile = templateFile;
                Properties.Settings.Default.AprilShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.AprilShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.AprilCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.AprilAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.AprilMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.AprilMinACF = nudMinACF.Value;
                Properties.Settings.Default.AprilMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.AprilMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.AprilMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.AprilMinICF = nudMinICF.Value;
                Properties.Settings.Default.AprilGetImagePath = GetImagePath;
            }
            if (month == 6)
            {
                Properties.Settings.Default.MayAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.MayBlur = cbBlur.Checked;
                Properties.Settings.Default.MayNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.MayAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.MayCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.MayCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.MayNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.MayShowContours = cbShowContours.Checked;
                Properties.Settings.Default.MayTemplateFile = templateFile;
                Properties.Settings.Default.MayShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.MayShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.MayCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.MayAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.MayMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.MayMinACF = nudMinACF.Value;
                Properties.Settings.Default.MayMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.MayMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.MayMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.MayMinICF = nudMinICF.Value;
                Properties.Settings.Default.MayGetImagePath = GetImagePath;
            }
            Properties.Settings.Default.Save();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            month = 2;
            button1.BackColor = System.Drawing.Color.DarkGray;
            button8.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;

            if (Properties.Settings.Default.JanuaryAdaptiveThBlockSize == 0)
                Properties.Settings.Default.JanuaryAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.JanuaryMaxACF == 0)
                Properties.Settings.Default.JanuaryMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinContourArea == 0)
                Properties.Settings.Default.JanuaryMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinContourLength == 0)
                Properties.Settings.Default.JanuaryMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinDefinition == 0)
                Properties.Settings.Default.JanuaryMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinACF == 0)
                Properties.Settings.Default.JanuaryMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryMinICF == 0)
                Properties.Settings.Default.JanuaryMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.JanuaryGetImagePath == "")
            {
                FolderBrowserDialog ofd = new FolderBrowserDialog();
                if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    GetImagePath = ofd.SelectedPath;
                    files = Directory.GetFiles(GetImagePath);
                    i = files.Length;
                    frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i - 1]));
                    ImagePath = "Current image file: " + GetImagePath;
                }
                Properties.Settings.Default.JanuaryGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.JanuaryTemplateFile == "")
                Properties.Settings.Default.JanuaryTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.JanuaryAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.JanuaryBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.JanuaryNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.JanuaryAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.JanuaryCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.JanuaryCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.JanuaryNoizeFilt;
            templateFile = Properties.Settings.Default.JanuaryTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.JanuaryShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.JanuaryShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.JanuaryShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.JanuaryCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.JanuaryAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.JanuaryMaxACF;
            nudMinACF.Value = Properties.Settings.Default.JanuaryMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.JanuaryMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.JanuaryMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.JanuaryMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.JanuaryMinICF;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.JanuaryGetImagePath;
                GetImage();
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            month = 3;
            button1.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.DarkGray;

            if (Properties.Settings.Default.FebruaryAdaptiveThBlockSize == 0)
                Properties.Settings.Default.FebruaryAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.FebruaryMaxACF == 0)
                Properties.Settings.Default.FebruaryMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinContourArea == 0)
                Properties.Settings.Default.FebruaryMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinContourLength == 0)
                Properties.Settings.Default.FebruaryMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinDefinition == 0)
                Properties.Settings.Default.FebruaryMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinACF == 0)
                Properties.Settings.Default.FebruaryMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryMinICF == 0)
                Properties.Settings.Default.FebruaryMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.FebruaryGetImagePath == "")
            {
                FolderBrowserDialog ofd = new FolderBrowserDialog();
                if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    GetImagePath = ofd.SelectedPath;
                    files = Directory.GetFiles(GetImagePath);
                    i = files.Length;
                    frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i - 1]));
                    ImagePath = "Current image file: " + GetImagePath;
                }
                Properties.Settings.Default.FebruaryGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.FebruaryTemplateFile == "")
                Properties.Settings.Default.FebruaryTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.FebruaryAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.FebruaryBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.FebruaryNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.FebruaryAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.FebruaryCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.FebruaryCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.FebruaryNoizeFilt;
            templateFile = Properties.Settings.Default.FebruaryTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.FebruaryShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.FebruaryShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.FebruaryShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.FebruaryCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.FebruaryAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.FebruaryMaxACF;
            nudMinACF.Value = Properties.Settings.Default.FebruaryMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.FebruaryMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.FebruaryMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.FebruaryMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.FebruaryMinICF;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.FebruaryGetImagePath;
                GetImage();
            }

        }



        private void button8_Click(object sender, EventArgs e)
        {
            month = 4;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.DarkGray;

            if (Properties.Settings.Default.MarchAdaptiveThBlockSize == 0)
                Properties.Settings.Default.MarchAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.MarchMaxACF == 0)
                Properties.Settings.Default.MarchMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinContourArea == 0)
                Properties.Settings.Default.MarchMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinContourLength == 0)
                Properties.Settings.Default.MarchMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinDefinition == 0)
                Properties.Settings.Default.MarchMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinACF == 0)
                Properties.Settings.Default.MarchMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchMinICF == 0)
                Properties.Settings.Default.MarchMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.MarchGetImagePath == "")
            {
                FolderBrowserDialog ofd = new FolderBrowserDialog();
                if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    GetImagePath = ofd.SelectedPath;
                    files = Directory.GetFiles(GetImagePath);
                    i = files.Length;
                    frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i - 1]));
                    ImagePath = "Current image file: " + GetImagePath;
                }
                Properties.Settings.Default.MarchGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.MarchTemplateFile == "")
                Properties.Settings.Default.MarchTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.MarchAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.MarchBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.MarchNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.MarchAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.MarchCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.MarchCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.MarchNoizeFilt;
            templateFile = Properties.Settings.Default.MarchTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.MarchShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.MarchShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.MarchShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.MarchCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.MarchAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.MarchMaxACF;
            nudMinACF.Value = Properties.Settings.Default.MarchMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.MarchMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.MarchMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.MarchMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.MarchMinICF;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.MarchGetImagePath;
                GetImage();
            }

        }
        private void button9_Click(object sender, EventArgs e)
        {
            month = 5;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.DarkGray;

            if (Properties.Settings.Default.AprilAdaptiveThBlockSize == 0)
                Properties.Settings.Default.AprilAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.AprilMaxACF == 0)
                Properties.Settings.Default.AprilMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinContourArea == 0)
                Properties.Settings.Default.AprilMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinContourLength == 0)
                Properties.Settings.Default.AprilMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinDefinition == 0)
                Properties.Settings.Default.AprilMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinACF == 0)
                Properties.Settings.Default.AprilMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilMinICF == 0)
                Properties.Settings.Default.AprilMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.AprilGetImagePath == "")
            {
                FolderBrowserDialog ofd = new FolderBrowserDialog();
                if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    GetImagePath = ofd.SelectedPath;
                    files = Directory.GetFiles(GetImagePath);
                    i = files.Length;
                    frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i - 1]));
                    ImagePath = "Current image file: " + GetImagePath;
                }
                Properties.Settings.Default.AprilGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.AprilTemplateFile == "")
                Properties.Settings.Default.AprilTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.AprilAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.AprilBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.AprilNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.AprilAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.AprilCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.AprilCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.AprilNoizeFilt;
            templateFile = Properties.Settings.Default.AprilTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.AprilShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.AprilShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.AprilShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.AprilCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.AprilAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.AprilMaxACF;
            nudMinACF.Value = Properties.Settings.Default.AprilMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.AprilMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.AprilMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.AprilMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.AprilMinICF;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.AprilGetImagePath;
                GetImage();
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            month = 6;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.DarkGray;

            if (Properties.Settings.Default.MayAdaptiveThBlockSize == 0)
                Properties.Settings.Default.MayAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.MayMaxACF == 0)
                Properties.Settings.Default.MayMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinContourArea == 0)
                Properties.Settings.Default.MayMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinContourLength == 0)
                Properties.Settings.Default.MayMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinDefinition == 0)
                Properties.Settings.Default.MayMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinACF == 0)
                Properties.Settings.Default.MayMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.MayMinICF == 0)
                Properties.Settings.Default.MayMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.MayGetImagePath == "")
            {
                FolderBrowserDialog ofd = new FolderBrowserDialog();
                if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    GetImagePath = ofd.SelectedPath;
                    files = Directory.GetFiles(GetImagePath);
                    i = files.Length;
                    frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i - 1]));
                    ImagePath = "Current image file: " + GetImagePath;
                }
                Properties.Settings.Default.MayGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.MayTemplateFile == "")
                Properties.Settings.Default.MayTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.MayAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.MayBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.MayNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.MayAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.MayCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.MayCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.MayNoizeFilt;
            templateFile = Properties.Settings.Default.MayTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.MayShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.MayShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.MayShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.MayCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.MayAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.MayMaxACF;
            nudMinACF.Value = Properties.Settings.Default.MayMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.MayMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.MayMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.MayMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.MayMinICF;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.MayGetImagePath;
                GetImage();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            tbResult.Items.Clear();
        }
        private void Form_Listener_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                thread2.Abort();
            }
            catch
            {
                return;
            }
        }


    }
}
