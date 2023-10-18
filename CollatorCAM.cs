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
using EasyModbus;
using System.ComponentModel.Composition.Primitives;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using ZedGraph;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;

namespace CollatorCAM
{

    public partial class Form_Listener : Form
    {
        private List<double> ptList = new List<double>();
        //private BezierCurve bc = new BezierCurve();

        private Emgu.CV.Capture _capture;
        Bitmap image;
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
        
        int i;
        int month;
        int monthph;
        int i_mdb;
        bool photoBlock;
        bool PhotoMonth;
        bool check1 = false;
        bool[] MDB_WRITE = new bool[15];
        bool[] CONTROL_READ = new bool[1];
        private string[] files;
        double gX;
        double gY;
        double g2X = 1;
        double g2Y = 1;
        int nRect = 0;

        /// 
        /// Init Form
        /// 
        public Form_Listener()
        {
            this.InitializeComponent();
            System.Windows.Forms.Application.Idle += new EventHandler(Application_Idle);
            thread2 = new Thread(() => WriteMDBS("WRITE"));
            
            //create image processor
            processor = new ContourAnalysisNS.ImageProcessor();
            //load default templates
            //templateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";
            //LoadTemplates(templateFile);

            //start capture from cam
            //StartCapture();                     (RUN CAMERA)
            //apply settings
            //ApplySettings();
            //
            RunForm();


            this.Closing += new CancelEventHandler(this.Form_Listener_Close);
            thread2.Start();
        }

        private void RunForm()
        {
            month = 1;
            button7.BackColor = System.Drawing.Color.DarkGray;
            Front();
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
                        ApplySettings();
                        if (modbus.Connected == true)
                        {
                            lblStat.Text = "Status: Connected";
                            btnStart.Refresh();
                            lblStat.Refresh();
                                if (monthph == 0)
                                    CONTROL_READ = modbus.ReadCoils(1025, 1);
                                if (monthph == 0 && CONTROL_READ[0] == true)
                                {
                                    ScanCycle();
                                }
                        }

                        else if (modbus.Connected == false)
                        {
                            lblStat.Text = "Status: Disconnected";
                            btnStart.Refresh();
                            lblStat.Refresh();
                        }

                    }));
                    }
                    else
                    {
                        label2.Text = "Current template file: " + templateFile;
                        ApplySettings();
                        if (modbus.Connected == true)
                        {
                            if (monthph ==0)
                                CONTROL_READ = modbus.ReadCoils(1025, 1);
                            if (monthph == 0 && CONTROL_READ[0] == true)
                            {
                                ScanCycle();
                            }
                        }
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
            textBox2.Text = Convert.ToString(ibMain.ZoomScale);
            //label4.Text = Convert.ToString();
            oldFrameCount = frameCount;
            if (processor.contours != null)
                lbContoursCount.Text = "Contours: " + processor.contours.Count;
            if (processor.foundTemplates != null)
                lbRecognized.Text = "Recognized contours: " + processor.foundTemplates.Count;
        }

        private void ibMain_Paint(object sender, PaintEventArgs e)
        {
                string TBtime = "";
                string TBmonth = "  NG Month";
                string TByear = "    NG Year";
                string TBmaori = "      NG Maori";
                DateTime thisDay = DateTime.Now;
                TBtime = thisDay.ToString("d") + " " + thisDay.ToString("T");

                if (frame == null) return; //not when prog run only

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
                    if (found.template.name == "January" || found.template.name == "February" || found.template.name == "March" ||
                    found.template.name == "April" || found.template.name == "May" || found.template.name == "June" ||
                    found.template.name == "July" || found.template.name == "August" || found.template.name == "September" ||
                    found.template.name == "October" || found.template.name == "November" || found.template.name == "December" ||
                    found.template.name == "Front" || found.template.name == "Rear")
                        TBmonth = "  OK " + found.template.name;
                    if (found.template.name == "2024")
                    {
                        TByear = "    OK " + found.template.name;
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
                            TBmaori = "      OK Maori";
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
                if ((PhotoMonth == true && ibMain.Image == frame) || (PhotoMonth == true && ibMain.Image == processor.binarizedFrame))
                {
                    tbResult.Items.Add(TBtime);
                    tbResult.Items.Add(TBmonth);
                    tbResult.Items.Add(TByear);
                    tbResult.Items.Add(TBmaori);
                if (TBmonth != "  NG Month" && TByear != "    NG Year" && TBmaori != "      NG Maori")
                    MDB_WRITE[i_mdb] = true;
                if (TBmonth == "  NG Month" || TByear == "    NG Year" || TBmaori == "      NG Maori")
                    MDB_WRITE[i_mdb] = false;
                    PhotoMonth = false;
                    monthph++;
                    i_mdb++;
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
                        //ApplyCamSettings();
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
            modbus.WriteSingleCoil(1025, false);
            modbus.WriteSingleCoil(1026, true);
            if (monthph == 0)
            {
                i_mdb = 1;
                photoBlock = true;
                Intro();
            }
            if (monthph == 1)
            {
                Front();
                PhotoMonth = true;
            }
            if (monthph == 2)
            {
                January();
                PhotoMonth = true;
            }
            if (monthph == 3)
            {
                February();
                PhotoMonth = true;
            }
            if (monthph == 4)
            {
                March();
                PhotoMonth = true;
            }
            if (monthph == 5)
            { 
                April();
                PhotoMonth = true;
            }
            if (monthph == 6)
            {  
                May();
                PhotoMonth = true;
            }
            if (monthph == 7)
            {
                June();
                PhotoMonth = true;
            }
            if (monthph == 8)
            {
                July();
                PhotoMonth = true;
            }
            if (monthph == 9)
            {
                August();
                PhotoMonth = true;
            }
            if (monthph == 10)
            {
                September();
                PhotoMonth = true;
            }
            if (monthph == 11)
            {
                October();
                PhotoMonth = true;
            }
            if (monthph == 12)
            {
                November();
                PhotoMonth = true;
            }
            if (monthph == 13)
            {
                December();
                PhotoMonth = true;
            }
            if (monthph == 14)
            {
                Rear();
                PhotoMonth = true;
            }
            if (monthph == 15)
            {
                MDB_WRITE[0] = true;
                modbus.WriteMultipleCoils(2019, MDB_WRITE);
                photoBlock = false;
                monthph = 0;
            }
        }

        private void Intro()
        {
            tbResult.Items.Add(" ");
            tbResult.Items.Add("                      SCAN           ");
            tbResult.Items.Add(" ");
            GetImagePath = Properties.Settings.Default.JanuaryGetImagePath;
            GetImage();
            monthph++;
            ScanCycle();
        }
        #region Month 
        private void Front()
        {
            if (Properties.Settings.Default.FrontAdaptiveThBlockSize == 0)
                Properties.Settings.Default.FrontAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.FrontMaxACF == 0)
                Properties.Settings.Default.FrontMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FrontMinContourArea == 0)
                Properties.Settings.Default.FrontMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FrontMinContourLength == 0)
                Properties.Settings.Default.FrontMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FrontMinDefinition == 0)
                Properties.Settings.Default.FrontMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.FrontMinACF == 0)
                Properties.Settings.Default.FrontMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.FrontMinICF == 0)
                Properties.Settings.Default.FrontMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.FrontGetImagePath == "")
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
                Properties.Settings.Default.FrontGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.FrontTemplateFile == "")
                Properties.Settings.Default.FrontTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.FrontAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.FrontBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.FrontNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.FrontAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.FrontCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.FrontCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.FrontNoizeFilt;
            templateFile = Properties.Settings.Default.FrontTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.FrontShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.FrontShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.FrontShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.FrontCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.FrontAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.FrontMaxACF;
            nudMinACF.Value = Properties.Settings.Default.FrontMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.FrontMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.FrontMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.FrontMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.FrontMinICF;
            cbRotation.SelectedIndex = Properties.Settings.Default.FrontRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.FrontGetImagePath;
                GetImage();
            }
        }
            private void January()
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
            cbRotation.SelectedIndex = Properties.Settings.Default.JanuaryRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.JanuaryGetImagePath;
                GetImage();
            }
        }
        private void February()
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
            cbRotation.SelectedIndex = Properties.Settings.Default.FebruaryRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.FebruaryGetImagePath;
                GetImage();
            }
        }
        private void March()
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
            cbRotation.SelectedIndex = Properties.Settings.Default.MarchRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.MarchGetImagePath;
                GetImage();
            }
        }
        private void April()
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
            cbRotation.SelectedIndex = Properties.Settings.Default.AprilRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.AprilGetImagePath;
                GetImage();
            }
        }
        private void May()
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
            cbRotation.SelectedIndex = Properties.Settings.Default.MayRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.MayGetImagePath;
                GetImage();
            }
        }
        private void June()
        {
            if (Properties.Settings.Default.JuneAdaptiveThBlockSize == 0)
                Properties.Settings.Default.JuneAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.JuneMaxACF == 0)
                Properties.Settings.Default.JuneMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JuneMinContourArea == 0)
                Properties.Settings.Default.JuneMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JuneMinContourLength == 0)
                Properties.Settings.Default.JuneMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JuneMinDefinition == 0)
                Properties.Settings.Default.JuneMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JuneMinACF == 0)
                Properties.Settings.Default.JuneMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.JuneMinICF == 0)
                Properties.Settings.Default.JuneMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.JuneGetImagePath == "")
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
                Properties.Settings.Default.JuneGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.JuneTemplateFile == "")
                Properties.Settings.Default.JuneTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.JuneAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.JuneBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.JuneNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.JuneAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.JuneCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.JuneCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.JuneNoizeFilt;
            templateFile = Properties.Settings.Default.JuneTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.JuneShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.JuneShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.JuneShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.JuneCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.JuneAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.JuneMaxACF;
            nudMinACF.Value = Properties.Settings.Default.JuneMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.JuneMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.JuneMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.JuneMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.JuneMinICF;
            cbRotation.SelectedIndex = Properties.Settings.Default.JuneRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.JuneGetImagePath;
                GetImage();
            }
        }
        private void July()
        {
            if (Properties.Settings.Default.JulyAdaptiveThBlockSize == 0)
                Properties.Settings.Default.JulyAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.JulyMaxACF == 0)
                Properties.Settings.Default.JulyMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JulyMinContourArea == 0)
                Properties.Settings.Default.JulyMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JulyMinContourLength == 0)
                Properties.Settings.Default.JulyMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JulyMinDefinition == 0)
                Properties.Settings.Default.JulyMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.JulyMinACF == 0)
                Properties.Settings.Default.JulyMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.JulyMinICF == 0)
                Properties.Settings.Default.JulyMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.JulyGetImagePath == "")
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
                Properties.Settings.Default.JulyGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.JulyTemplateFile == "")
                Properties.Settings.Default.JulyTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.JulyAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.JulyBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.JulyNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.JulyAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.JulyCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.JulyCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.JulyNoizeFilt;
            templateFile = Properties.Settings.Default.JulyTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.JulyShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.JulyShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.JulyShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.JulyCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.JulyAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.JulyMaxACF;
            nudMinACF.Value = Properties.Settings.Default.JulyMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.JulyMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.JulyMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.JulyMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.JulyMinICF;
            cbRotation.SelectedIndex = Properties.Settings.Default.JulyRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.JulyGetImagePath;
                GetImage();
            }
        }
        private void August()
        {
            if (Properties.Settings.Default.AugustAdaptiveThBlockSize == 0)
                Properties.Settings.Default.AugustAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.AugustMaxACF == 0)
                Properties.Settings.Default.AugustMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AugustMinContourArea == 0)
                Properties.Settings.Default.AugustMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AugustMinContourLength == 0)
                Properties.Settings.Default.AugustMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AugustMinDefinition == 0)
                Properties.Settings.Default.AugustMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.AugustMinACF == 0)
                Properties.Settings.Default.AugustMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.AugustMinICF == 0)
                Properties.Settings.Default.AugustMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.AugustGetImagePath == "")
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
                Properties.Settings.Default.AugustGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.AugustTemplateFile == "")
                Properties.Settings.Default.AugustTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.AugustAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.AugustBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.AugustNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.AugustAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.AugustCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.AugustCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.AugustNoizeFilt;
            templateFile = Properties.Settings.Default.AugustTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.AugustShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.AugustShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.AugustShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.AugustCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.AugustAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.AugustMaxACF;
            nudMinACF.Value = Properties.Settings.Default.AugustMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.AugustMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.AugustMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.AugustMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.AugustMinICF;
            cbRotation.SelectedIndex = Properties.Settings.Default.AugustRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.AugustGetImagePath;
                GetImage();
            }
        }
        private void September()
        {
            if (Properties.Settings.Default.SeptemberAdaptiveThBlockSize == 0)
                Properties.Settings.Default.SeptemberAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.SeptemberMaxACF == 0)
                Properties.Settings.Default.SeptemberMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.SeptemberMinContourArea == 0)
                Properties.Settings.Default.SeptemberMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.SeptemberMinContourLength == 0)
                Properties.Settings.Default.SeptemberMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.SeptemberMinDefinition == 0)
                Properties.Settings.Default.SeptemberMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.SeptemberMinACF == 0)
                Properties.Settings.Default.SeptemberMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.SeptemberMinICF == 0)
                Properties.Settings.Default.SeptemberMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.SeptemberGetImagePath == "")
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
                Properties.Settings.Default.SeptemberGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.SeptemberTemplateFile == "")
                Properties.Settings.Default.SeptemberTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.SeptemberAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.SeptemberBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.SeptemberNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.SeptemberAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.SeptemberCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.SeptemberCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.SeptemberNoizeFilt;
            templateFile = Properties.Settings.Default.SeptemberTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.SeptemberShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.SeptemberShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.SeptemberShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.SeptemberCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.SeptemberAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.SeptemberMaxACF;
            nudMinACF.Value = Properties.Settings.Default.SeptemberMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.SeptemberMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.SeptemberMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.SeptemberMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.SeptemberMinICF;
            cbRotation.SelectedIndex = Properties.Settings.Default.SeptemberRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.SeptemberGetImagePath;
                GetImage();
            }
        }
        private void October()
        {
            if (Properties.Settings.Default.OctoberAdaptiveThBlockSize == 0)
                Properties.Settings.Default.OctoberAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.OctoberMaxACF == 0)
                Properties.Settings.Default.OctoberMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.OctoberMinContourArea == 0)
                Properties.Settings.Default.OctoberMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.OctoberMinContourLength == 0)
                Properties.Settings.Default.OctoberMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.OctoberMinDefinition == 0)
                Properties.Settings.Default.OctoberMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.OctoberMinACF == 0)
                Properties.Settings.Default.OctoberMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.OctoberMinICF == 0)
                Properties.Settings.Default.OctoberMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.OctoberGetImagePath == "")
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
                Properties.Settings.Default.OctoberGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.OctoberTemplateFile == "")
                Properties.Settings.Default.OctoberTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.OctoberAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.OctoberBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.OctoberNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.OctoberAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.OctoberCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.OctoberCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.OctoberNoizeFilt;
            templateFile = Properties.Settings.Default.OctoberTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.OctoberShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.OctoberShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.OctoberShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.OctoberCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.OctoberAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.OctoberMaxACF;
            nudMinACF.Value = Properties.Settings.Default.OctoberMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.OctoberMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.OctoberMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.OctoberMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.OctoberMinICF;
            cbRotation.SelectedIndex = Properties.Settings.Default.OctoberRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.OctoberGetImagePath;
                GetImage();
            }
        }
        private void November()
        {
            if (Properties.Settings.Default.NovemberAdaptiveThBlockSize == 0)
                Properties.Settings.Default.NovemberAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.NovemberMaxACF == 0)
                Properties.Settings.Default.NovemberMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.NovemberMinContourArea == 0)
                Properties.Settings.Default.NovemberMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.NovemberMinContourLength == 0)
                Properties.Settings.Default.NovemberMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.NovemberMinDefinition == 0)
                Properties.Settings.Default.NovemberMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.NovemberMinACF == 0)
                Properties.Settings.Default.NovemberMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.NovemberMinICF == 0)
                Properties.Settings.Default.NovemberMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.NovemberGetImagePath == "")
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
                Properties.Settings.Default.NovemberGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.NovemberTemplateFile == "")
                Properties.Settings.Default.NovemberTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.NovemberAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.NovemberBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.NovemberNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.NovemberAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.NovemberCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.NovemberCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.NovemberNoizeFilt;
            templateFile = Properties.Settings.Default.NovemberTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.NovemberShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.NovemberShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.NovemberShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.NovemberCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.NovemberAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.NovemberMaxACF;
            nudMinACF.Value = Properties.Settings.Default.NovemberMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.NovemberMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.NovemberMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.NovemberMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.NovemberMinICF;
            cbRotation.SelectedIndex = Properties.Settings.Default.NovemberRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.NovemberGetImagePath;
                GetImage();
            }
        }
        private void December()
        {
            if (Properties.Settings.Default.DecemberAdaptiveThBlockSize == 0)
                Properties.Settings.Default.DecemberAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.DecemberMaxACF == 0)
                Properties.Settings.Default.DecemberMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.DecemberMinContourArea == 0)
                Properties.Settings.Default.DecemberMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.DecemberMinContourLength == 0)
                Properties.Settings.Default.DecemberMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.DecemberMinDefinition == 0)
                Properties.Settings.Default.DecemberMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.DecemberMinACF == 0)
                Properties.Settings.Default.DecemberMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.DecemberMinICF == 0)
                Properties.Settings.Default.DecemberMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.DecemberGetImagePath == "")
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
                Properties.Settings.Default.DecemberGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.DecemberTemplateFile == "")
                Properties.Settings.Default.DecemberTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.DecemberAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.DecemberBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.DecemberNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.DecemberAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.DecemberCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.DecemberCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.DecemberNoizeFilt;
            templateFile = Properties.Settings.Default.DecemberTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.DecemberShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.DecemberShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.DecemberShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.DecemberCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.DecemberAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.DecemberMaxACF;
            nudMinACF.Value = Properties.Settings.Default.DecemberMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.DecemberMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.DecemberMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.DecemberMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.DecemberMinICF;
            cbRotation.SelectedIndex = Properties.Settings.Default.DecemberRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.DecemberGetImagePath;
                GetImage();
            }
        }
        private void Rear()
        {
            if (Properties.Settings.Default.RearAdaptiveThBlockSize == 0)
                Properties.Settings.Default.RearAdaptiveThBlockSize = new decimal(new int[] { 1, 0, 0, 0 }); ;
            if (Properties.Settings.Default.RearMaxACF == 0)
                Properties.Settings.Default.RearMaxACF = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.RearMinContourArea == 0)
                Properties.Settings.Default.RearMinContourArea = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.RearMinContourLength == 0)
                Properties.Settings.Default.RearMinContourLength = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.RearMinDefinition == 0)
                Properties.Settings.Default.RearMinDefinition = new decimal(new int[] { 0, 0, 0, 65536 });
            if (Properties.Settings.Default.RearMinACF == 0)
                Properties.Settings.Default.RearMinACF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.RearMinICF == 0)
                Properties.Settings.Default.RearMinICF = new decimal(new int[] { 2, 0, 0, 65536 });
            if (Properties.Settings.Default.RearGetImagePath == "")
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
                Properties.Settings.Default.RearGetImagePath = GetImagePath;
            }
            if (Properties.Settings.Default.RearTemplateFile == "")
                Properties.Settings.Default.RearTemplateFile = AppDomain.CurrentDomain.BaseDirectory + "\\Tahoma.bin";

            cbAutoContrast.Checked = Properties.Settings.Default.RearAutoContrast;
            cbBlur.Checked = Properties.Settings.Default.RearBlur;
            cbAdaptiveNoiseFilter.Checked = Properties.Settings.Default.RearNoizeFilter;
            cbAllowAngleMore45.Checked = Properties.Settings.Default.RearAllowAngles;
            cbCamResolution.SelectedIndex = Properties.Settings.Default.RearCameraResolution;
            cbCaptureFromCam.Checked = Properties.Settings.Default.RearCaptureFromCamera;
            cbNoiseFilter.Checked = Properties.Settings.Default.RearNoizeFilt;
            templateFile = Properties.Settings.Default.RearTemplateFile;
            cbShowAngle.Checked = Properties.Settings.Default.RearShowAngle;
            cbShowBinarized.Checked = Properties.Settings.Default.RearShowBinarized;
            cbShowContours.Checked = Properties.Settings.Default.RearShowContours;
            cbCycleCapture.Checked = Properties.Settings.Default.RearCycleCapture;
            nudAdaptiveThBlockSize.Value = Properties.Settings.Default.RearAdaptiveThBlockSize;
            nudMaxACFdesc.Value = Properties.Settings.Default.RearMaxACF;
            nudMinACF.Value = Properties.Settings.Default.RearMinACF;
            nudMinContourArea.Value = Properties.Settings.Default.RearMinContourArea;
            nudMinContourLength.Value = Properties.Settings.Default.RearMinContourLength;
            nudMinDefinition.Value = Properties.Settings.Default.RearMinDefinition;
            nudMinICF.Value = Properties.Settings.Default.RearMinICF;
            cbRotation.SelectedIndex = Properties.Settings.Default.RearRotation;
            LoadTemplates(templateFile);
            if (cbCaptureFromCam.Checked == false)
            {
                GetImagePath = Properties.Settings.Default.RearGetImagePath;
                GetImage();
            }
        }
#endregion
        private void GetImage()
        {
            if (GetImagePath != null)
            {
                
                files = Directory.GetFiles(GetImagePath);
                i = files.Length;
                Image<Bgr, byte> img = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i - 1]));
                //e.Graphics.DrawRectangle(px, new Rectangle(Convert.ToInt16(gX), Convert.ToInt16(gY), 100, 100));
                double x = 0;
                if (cbRotation.SelectedIndex == 0)
                x = 0;
                if (cbRotation.SelectedIndex == 1)
                    x = -90;
                if (cbRotation.SelectedIndex == 2)
                    x = 90;
                if (cbRotation.SelectedIndex == 3)
                    x = 180;
                frame = img.Rotate(x, new Bgr(255,255,255), false);
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
        #region Save Month
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (month == 1)
            {
                    Properties.Settings.Default.FrontAutoContrast = cbAutoContrast.Checked;
                    Properties.Settings.Default.FrontBlur = cbBlur.Checked;
                    Properties.Settings.Default.FrontNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                    Properties.Settings.Default.FrontAllowAngles = cbAllowAngleMore45.Checked;
                    Properties.Settings.Default.FrontCameraResolution = cbCamResolution.SelectedIndex;
                    Properties.Settings.Default.FrontCaptureFromCamera = cbCaptureFromCam.Checked;
                    Properties.Settings.Default.FrontNoizeFilt = cbNoiseFilter.Checked;
                    Properties.Settings.Default.FrontShowContours = cbShowContours.Checked;
                    Properties.Settings.Default.FrontTemplateFile = templateFile;
                    Properties.Settings.Default.FrontShowAngle = cbShowAngle.Checked;
                    Properties.Settings.Default.FrontShowBinarized = cbShowBinarized.Checked;
                    Properties.Settings.Default.FrontCycleCapture = cbCycleCapture.Checked;
                    Properties.Settings.Default.FrontAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                    Properties.Settings.Default.FrontMaxACF = nudMaxACFdesc.Value;
                    Properties.Settings.Default.FrontMinACF = nudMinACF.Value;
                    Properties.Settings.Default.FrontMinContourArea = nudMinContourArea.Value;
                    Properties.Settings.Default.FrontMinContourLength = nudMinContourLength.Value;
                    Properties.Settings.Default.FrontMinDefinition = nudMinDefinition.Value;
                    Properties.Settings.Default.FrontMinICF = nudMinICF.Value;
                    Properties.Settings.Default.FrontGetImagePath = GetImagePath;
                    Properties.Settings.Default.FrontRotation = cbRotation.SelectedIndex;
            }
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
                Properties.Settings.Default.JanuaryRotation = cbRotation.SelectedIndex;
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
                Properties.Settings.Default.FebruaryRotation = cbRotation.SelectedIndex;
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
                Properties.Settings.Default.MarchRotation = cbRotation.SelectedIndex;
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
                Properties.Settings.Default.AprilRotation = cbRotation.SelectedIndex;
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
                Properties.Settings.Default.MayRotation = cbRotation.SelectedIndex;
            }
            if (month == 7)
            {
                Properties.Settings.Default.JuneAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.JuneBlur = cbBlur.Checked;
                Properties.Settings.Default.JuneNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.JuneAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.JuneCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.JuneCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.JuneNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.JuneShowContours = cbShowContours.Checked;
                Properties.Settings.Default.JuneTemplateFile = templateFile;
                Properties.Settings.Default.JuneShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.JuneShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.JuneCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.JuneAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.JuneMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.JuneMinACF = nudMinACF.Value;
                Properties.Settings.Default.JuneMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.JuneMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.JuneMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.JuneMinICF = nudMinICF.Value;
                Properties.Settings.Default.JuneGetImagePath = GetImagePath;
                Properties.Settings.Default.JuneRotation = cbRotation.SelectedIndex;
            }
            if (month == 8)
            {
                Properties.Settings.Default.JulyAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.JulyBlur = cbBlur.Checked;
                Properties.Settings.Default.JulyNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.JulyAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.JulyCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.JulyCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.JulyNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.JulyShowContours = cbShowContours.Checked;
                Properties.Settings.Default.JulyTemplateFile = templateFile;
                Properties.Settings.Default.JulyShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.JulyShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.JulyCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.JulyAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.JulyMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.JulyMinACF = nudMinACF.Value;
                Properties.Settings.Default.JulyMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.JulyMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.JulyMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.JulyMinICF = nudMinICF.Value;
                Properties.Settings.Default.JulyGetImagePath = GetImagePath;
                Properties.Settings.Default.JulyRotation = cbRotation.SelectedIndex;
            }
            if (month == 9)
            {
                Properties.Settings.Default.AugustAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.AugustBlur = cbBlur.Checked;
                Properties.Settings.Default.AugustNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.AugustAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.AugustCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.AugustCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.AugustNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.AugustShowContours = cbShowContours.Checked;
                Properties.Settings.Default.AugustTemplateFile = templateFile;
                Properties.Settings.Default.AugustShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.AugustShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.AugustCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.AugustAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.AugustMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.AugustMinACF = nudMinACF.Value;
                Properties.Settings.Default.AugustMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.AugustMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.AugustMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.AugustMinICF = nudMinICF.Value;
                Properties.Settings.Default.AugustGetImagePath = GetImagePath;
                Properties.Settings.Default.AugustRotation = cbRotation.SelectedIndex;
            }
            if (month == 10)
            {
                Properties.Settings.Default.SeptemberAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.SeptemberBlur = cbBlur.Checked;
                Properties.Settings.Default.SeptemberNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.SeptemberAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.SeptemberCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.SeptemberCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.SeptemberNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.SeptemberShowContours = cbShowContours.Checked;
                Properties.Settings.Default.SeptemberTemplateFile = templateFile;
                Properties.Settings.Default.SeptemberShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.SeptemberShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.SeptemberCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.SeptemberAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.SeptemberMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.SeptemberMinACF = nudMinACF.Value;
                Properties.Settings.Default.SeptemberMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.SeptemberMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.SeptemberMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.SeptemberMinICF = nudMinICF.Value;
                Properties.Settings.Default.SeptemberGetImagePath = GetImagePath;
                Properties.Settings.Default.SeptemberRotation = cbRotation.SelectedIndex;
            }
            if (month == 11)
            {
                Properties.Settings.Default.OctoberAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.OctoberBlur = cbBlur.Checked;
                Properties.Settings.Default.OctoberNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.OctoberAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.OctoberCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.OctoberCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.OctoberNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.OctoberShowContours = cbShowContours.Checked;
                Properties.Settings.Default.OctoberTemplateFile = templateFile;
                Properties.Settings.Default.OctoberShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.OctoberShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.OctoberCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.OctoberAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.OctoberMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.OctoberMinACF = nudMinACF.Value;
                Properties.Settings.Default.OctoberMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.OctoberMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.OctoberMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.OctoberMinICF = nudMinICF.Value;
                Properties.Settings.Default.OctoberGetImagePath = GetImagePath;
                Properties.Settings.Default.OctoberRotation = cbRotation.SelectedIndex;
            }
            if (month == 12)
            {
                Properties.Settings.Default.NovemberAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.NovemberBlur = cbBlur.Checked;
                Properties.Settings.Default.NovemberNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.NovemberAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.NovemberCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.NovemberCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.NovemberNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.NovemberShowContours = cbShowContours.Checked;
                Properties.Settings.Default.NovemberTemplateFile = templateFile;
                Properties.Settings.Default.NovemberShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.NovemberShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.NovemberCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.NovemberAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.NovemberMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.NovemberMinACF = nudMinACF.Value;
                Properties.Settings.Default.NovemberMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.NovemberMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.NovemberMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.NovemberMinICF = nudMinICF.Value;
                Properties.Settings.Default.NovemberGetImagePath = GetImagePath;
                Properties.Settings.Default.NovemberRotation = cbRotation.SelectedIndex;
            }
            if (month == 13)
            {
                Properties.Settings.Default.DecemberAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.DecemberBlur = cbBlur.Checked;
                Properties.Settings.Default.DecemberNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.DecemberAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.DecemberCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.DecemberCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.DecemberNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.DecemberShowContours = cbShowContours.Checked;
                Properties.Settings.Default.DecemberTemplateFile = templateFile;
                Properties.Settings.Default.DecemberShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.DecemberShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.DecemberCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.DecemberAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.DecemberMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.DecemberMinACF = nudMinACF.Value;
                Properties.Settings.Default.DecemberMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.DecemberMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.DecemberMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.DecemberMinICF = nudMinICF.Value;
                Properties.Settings.Default.DecemberGetImagePath = GetImagePath;
                Properties.Settings.Default.DecemberRotation = cbRotation.SelectedIndex;
            }
            if (month == 14)
            {
                Properties.Settings.Default.RearAutoContrast = cbAutoContrast.Checked;
                Properties.Settings.Default.RearBlur = cbBlur.Checked;
                Properties.Settings.Default.RearNoizeFilter = cbAdaptiveNoiseFilter.Checked;
                Properties.Settings.Default.RearAllowAngles = cbAllowAngleMore45.Checked;
                Properties.Settings.Default.RearCameraResolution = cbCamResolution.SelectedIndex;
                Properties.Settings.Default.RearCaptureFromCamera = cbCaptureFromCam.Checked;
                Properties.Settings.Default.RearNoizeFilt = cbNoiseFilter.Checked;
                Properties.Settings.Default.RearShowContours = cbShowContours.Checked;
                Properties.Settings.Default.RearTemplateFile = templateFile;
                Properties.Settings.Default.RearShowAngle = cbShowAngle.Checked;
                Properties.Settings.Default.RearShowBinarized = cbShowBinarized.Checked;
                Properties.Settings.Default.RearCycleCapture = cbCycleCapture.Checked;
                Properties.Settings.Default.RearAdaptiveThBlockSize = nudAdaptiveThBlockSize.Value;
                Properties.Settings.Default.RearMaxACF = nudMaxACFdesc.Value;
                Properties.Settings.Default.RearMinACF = nudMinACF.Value;
                Properties.Settings.Default.RearMinContourArea = nudMinContourArea.Value;
                Properties.Settings.Default.RearMinContourLength = nudMinContourLength.Value;
                Properties.Settings.Default.RearMinDefinition = nudMinDefinition.Value;
                Properties.Settings.Default.RearMinICF = nudMinICF.Value;
                Properties.Settings.Default.RearGetImagePath = GetImagePath;
                Properties.Settings.Default.RearRotation = cbRotation.SelectedIndex;
            }
            Properties.Settings.Default.Save();
        }
#endregion
        private void button7_Click(object sender, EventArgs e)
        {
            month = 1;
            button7.BackColor = System.Drawing.Color.DarkGray;
            button1.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;

            Front();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            month = 2;
            button1.BackColor = System.Drawing.Color.DarkGray;
            button8.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;

            January();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            month = 3;
            button1.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.DarkGray;

            February();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            month = 4;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.DarkGray;

            March();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            month = 5;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.DarkGray;

            April();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            month = 6;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.DarkGray;

            May();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            month = 7;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.DarkGray;

            June();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            month = 8;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.DarkGray;

            July();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            month = 9;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.DarkGray;

            August();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            month = 10;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.DarkGray;

            September();
        }
        private void button15_Click(object sender, EventArgs e)
        {
            month = 11;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.DarkGray;

            October();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            month = 12;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.DarkGray;

            November();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            month = 13;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.DarkGray;

            December();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            month = 14;
            button1.BackColor = System.Drawing.Color.Transparent;
            button2.BackColor = System.Drawing.Color.Transparent;
            button8.BackColor = System.Drawing.Color.Transparent;
            button9.BackColor = System.Drawing.Color.Transparent;
            button10.BackColor = System.Drawing.Color.Transparent;
            button11.BackColor = System.Drawing.Color.Transparent;
            button12.BackColor = System.Drawing.Color.Transparent;
            button13.BackColor = System.Drawing.Color.Transparent;
            button14.BackColor = System.Drawing.Color.Transparent;
            button15.BackColor = System.Drawing.Color.Transparent;
            button16.BackColor = System.Drawing.Color.Transparent;
            button17.BackColor = System.Drawing.Color.Transparent;
            button7.BackColor = System.Drawing.Color.Transparent;
            button18.BackColor = System.Drawing.Color.DarkGray;

            Rear();
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            modbus.IPAddress = Convert.ToString(txtIPAdress.Text);
            modbus.Port = Convert.ToInt32(txtPort.Text);
            if (modbus.Connected == false)
            {
                try
                {

                    modbus.Connect();
                    check1 = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connect Modbus" + ex.Message);
                }
                //if (modbus.Connected == true)
                //lblStat.Text = "Status: Connected";

                //this.lblStat.ForeColor = System.Drawing.Color.Green;
            }
            else if (modbus.Connected == true)
            {
                try
                {
                    check1 = false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Disconnect Modbus: " + ex.Message);

                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetImage();
        }
        Pen px = new Pen(Brushes.Red);
        Pen newpx = new Pen(Brushes.Magenta);
        Graphics g;

        private void ibMain_Click(object sender, MouseEventArgs g)
        {
            if (ibMain.ZoomScale <= 1)
            {
                if (nRect == 0)
                {
                    gX = (g.X / ibMain.ZoomScale);
                    gY = (g.Y / ibMain.ZoomScale);
                    g2X = 1;
                    g2Y = 1;
                }
                if (nRect == 1)
                {
                    g2X = ((g.X / ibMain.ZoomScale) - gX);
                    g2Y = ((g.Y / ibMain.ZoomScale) - gY);
                    nRect = -1;
                    Bitmap pic = frame.ToBitmap();
                    image = pic.Clone(new Rectangle(Convert.ToInt16(gX), Convert.ToInt16(gY), Convert.ToInt16(g2X), Convert.ToInt16(g2Y)), PixelFormat.Format16bppRgb555);
                    imageBox1.Image = new Image<Bgr, byte>(image);
                }
                nRect++;
            }
        }

    }
}
