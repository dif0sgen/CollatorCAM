using EasyModbus;
//using Emgu.CV.OCR;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using TCP_LISTENER_Delta.Properties;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using ContourAnalysisNS;
using System.IO;
using System.Text.RegularExpressions;
using ContourAnalysisDemo;
using CollatorCAM.Properties;
using TCP_LISTENER_Delta;
using System.Security.Cryptography;

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
        private Thread thread1;
        private Thread thread2;
        private Thread thread3;
        private Thread n_server;
        private Thread n_shot;
        private TcpListener listener;
        string out1 = "0";
        string out2 = "0";
        string position = "";


        string JobID = "JobID";
        string SchoolName = "SchoolName";
        string CalendarType = "CZ S-S";
        string ImagePath = "Please select image";
        string GetImagePath;
        string FoundTemplateName;
        public int ACC_X_MAN;
        int motorSpeed;
        int motorSpeedX;
        int motorSpeedY;
        int ManSpd;
        int txt1;
        int txt2;
        int txt3;
        int txt4;
        int txt5;
        int txt6;
        int txt7;
        int txt8;
        int txt9;
        int txt10;
        int txt11;
        int Scan_TMR;
        int Grab_TMR;
        int Release_TMR;
        int i;
        int milliseconds = 300;
        int month;
        bool PhotoMonth;
        //alarms
        bool Rilecart_emergency_stop;
        bool Rilecart_missing_fin;
        bool Rilecart_stop;
        bool Rilecart_bypass;
        bool Collator_Missing_left;
        bool Collator_Missing_right;
        bool Collator_Paper_jam;
        bool Collator_bypass;
        bool Rotation_paper_jam;

        bool capture_from_camera;
        int camera_resolution;
        bool blur;
        bool autocontrast;
        int adaptive_trshold;
        bool noize_filter;
        bool show_angle;
        bool show_contours;
        int min_contour_length;
        int mіт_contour_area;
        int noize_int;
        bool noize_filt;
        int max_acf;
        int min_acf;
        int allow_angles;


        bool bit1;
        bool bit2;
        bool bit3;
        bool bit4;
        bool bit5;
        bool bit6;
        bool bit7;
        bool bit8;
        bool bit9;
        bool bit10;
        bool bit11;
        bool bit12;
        bool bit13;
        bool bit14;
        bool angle0;
        bool angle90;
        bool angle180;
        bool run = false;
        bool BoolUp;
        bool BoolDwn;
        bool BoolCollator;
        bool BoolRailcart;
        Int32 posXtab1;
        Int32 posXtab2;
        Int32 posYtab1;
        Int32 posYtab2;
        Int32 Hi;
        private int imageIndex;
        private string[] imageList;
        private Int32[] WORD_READ = new Int32[56];
        private Int32[] WORD_WRITE = new Int32[56];
        private bool[] CONTROL_WRITE = new bool[16];
        private bool[] CONTROL_READ = new bool[16];
        private string[] files;
      
        private bool check1 = false;
    
        string sub = "";
        string tesser;


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


            thread1 = new Thread(() => ReadMDBS("MDBS"));
            thread2 = new Thread(() => WriteMDBS("WRITE"));
            //thread3 = new Thread(() => btnOpen_Click("IMAGE"));
            thread1.Start();
            thread2.Start();


            imageIndex = 0;
            //aTimer.Elapsed += ReadMDBS; //CYCLE VOID
            //aTimer.Interval = 50;
            //aTimer.AutoReset = true;
            //aTimer.Enabled = true;
            //bTimer.Elapsed += WriteMDBS;//CYCLE VOID
            //bTimer.Interval = 50;
            //bTimer.AutoReset = true;
            //bTimer.Enabled = true;
            this.Closing += new CancelEventHandler(this.Form_Listener_Close);
            btnUP.MouseUp += btnUP_Up;
            btnUP.MouseDown += btnUP_Down;
            btnUP.MouseEnter += btnUp_ENTER;
            btnDWN.MouseDown += btnDWN_Down;
            btnDWN.MouseUp += btnDWN_Up;
            btnDWN.MouseEnter += btnDWN_ENTER;
            btnStart.MouseEnter += OnMouseEnterButton1;
            btnStart.MouseLeave += OnMouseLeaveButton1;
           

            ///
            /// GET SAVED VALUES
            ///
            textBox45.Text = Properties.Settings.Default.ACC_Y_MAN;
            textBox44.Text = Properties.Settings.Default.DEC_Y_MAN;
            txtPort.Text = Properties.Settings.Default.Port;
            txtIPAdress.Text = Properties.Settings.Default.IP;
            textBox16.Text = Properties.Settings.Default.SpeedY1;
            textBox17.Text = Properties.Settings.Default.SpeedY2;
            textBox24.Text = Properties.Settings.Default.POS1;
            textBox23.Text = Properties.Settings.Default.POS2;
            textBox22.Text = Properties.Settings.Default.POS3;
            textBox27.Text = Properties.Settings.Default.GrabTMR;
            textBox26.Text = Properties.Settings.Default.ScanTMR;
            textBox28.Text = Properties.Settings.Default.ReleaseTMR;
            textBox5.Text = Properties.Settings.Default.tesPath;
            textBox2.Text = Properties.Settings.Default.Height;
            textBox4.Text = Properties.Settings.Default.ManSpdSave.ToString();
            trackBar1.Value = Properties.Settings.Default.ManSpdSave;
            checkBox12.Checked = Properties.Settings.Default.CollatorSave;
            checkBox11.Checked = Properties.Settings.Default.RailcartSave;
        }
        /// 
        /// STOP
        /// 
        private void button2_Click(object sender, EventArgs e)
        {
            if (modbus.Connected == true)
            {
                //CONTROL[5] = false;
                //CONTROL[6] = true;
            }
            else if (modbus.Connected == false)
            {
                MessageBox.Show("PLC disabled");
            }
        }
        /// 
        /// START
        /// 
        private void button1_Click(object sender, EventArgs e)
        {
            if (modbus.Connected == true)
            {

            }
            else if (modbus.Connected == false)
            {
                MessageBox.Show("PLC disabled");
            }
        }
        /// 
        /// HOME
        ///
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (modbus.Connected == true)
            {

            }
            else if (modbus.Connected == false)
            {
                MessageBox.Show("PLC disabled");
            }
        }
        /// 
        /// Set color on mouse BTN_Connect
        /// 
        private void OnMouseEnterButton1(object sender, EventArgs e)
        {
            btnStart.BackColor = System.Drawing.Color.FromArgb(218, 67, 60); // or Color.Red or whatever you want
        }
        private void OnMouseLeaveButton1(object sender, EventArgs e)
        {
            btnStart.BackColor = System.Drawing.Color.FromArgb(115, 115, 115);
        }
        /// 
        /// Connect to PLC
        /// 
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
                    MessageBox.Show("Connect Modbus"+ex.Message);
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
                    MessageBox.Show("Disconnect Modbus: "+ex.Message);

                }
            }
        }


         

        /// 
        /// Read/Write Modbus cycle
        /// 
        void ReadMDBS(string name)
        {
            while (true)
            {
                if (modbus.Connected == true)
                {
                    try
                    {

                        //WORD_WRITE[2] = Convert.ToInt32(motorSpeedX); //Write Manual Speed X
                        //WORD_WRITE[10] = Convert.ToInt32(ManSpd);//Write ManSpd
                        //WORD_WRITE[30] = Convert.ToInt32(motorSpeed); //Write Main Speed Percent
                        //WORD_WRITE[4] = txt1; //ACC X
                        //WORD_WRITE[12] = txt3; //ACC Y
                        //WORD_WRITE[28] = txt2; //ACC X
                        //WORD_WRITE[26] = txt4; //ACC Y
                        //WORD_WRITE[32] = txt5; //SPD X1
                        //WORD_WRITE[34] = txt6; //SPD X2
                        //WORD_WRITE[36] = txt7; //SPD Y1
                        //WORD_WRITE[38] = txt8; //SPD Y2
                        //WORD_WRITE[48] = txt9; //POSITION 1
                        //WORD_WRITE[50] = txt10; //POSITION 2
                        //WORD_WRITE[52] = txt11; //POSITION 3
                        //WORD_WRITE[16] = Grab_TMR; //Grab_TMR
                        //WORD_WRITE[18] = Scan_TMR; //Scan_TMR
                        //WORD_WRITE[20] = Release_TMR; //Release_TMR
                        //WORD_WRITE[54] = Hi;

                        CONTROL_WRITE[0] = checkBox1.Checked; //M1000
                        CONTROL_WRITE[1] = checkBox2.Checked; //M1001
                        CONTROL_WRITE[2] = angle0; //M1002
                        CONTROL_WRITE[3] = angle90; //M1003
                        CONTROL_WRITE[4] = angle180; //M1004
                        CONTROL_WRITE[5] = checkBox6.Checked; //M1005
                        CONTROL_WRITE[6] = checkBox7.Checked; //M1006
                        CONTROL_WRITE[7] = checkBox8.Checked; //M1007
                        CONTROL_WRITE[8] = checkBox9.Checked; //M1008
                        CONTROL_WRITE[9] = checkBox10.Checked; //M1009
                        CONTROL_WRITE[10] = BoolUp; //M1010
                        CONTROL_WRITE[11] = BoolDwn; //M1011
                        CONTROL_WRITE[12] = BoolCollator; //M1012
                        CONTROL_WRITE[13] = BoolRailcart; //M1013
                        CONTROL_WRITE[14] = checkBox3.Checked; //M1014
                        CONTROL_WRITE[15] = run; //M1015

                        try
                        {
                            modbus.WriteMultipleCoils(1000, CONTROL_WRITE); // WRITE ALL BITS
                        }

                        catch (Exception ex)
                       {
                           MessageBox.Show("Write mulriple coils: " + ex.Message);
                       }
                        try
                        {
                            //modbus.WriteMultipleRegisters(0, WORD_WRITE); ;  // WRITE ALL WORDS
                        }

                       catch (Exception ex)
                        {
                           MessageBox.Show("Write mulriple registers: " + ex.Message);
                        }
                        if (check1 == true)
                        {
                            try
                            {
                                //WORD_READ = modbus.ReadHoldingRegisters(0, 56); //READ ALL WORDS
                                //posXtab1 = EasyModbus.ModbusClient.ConvertRegistersToInt(modbus.ReadHoldingRegisters(24, 2));
                                //posYtab1 = EasyModbus.ModbusClient.ConvertRegistersToInt(modbus.ReadHoldingRegisters(22, 2));
                            }

                            catch (Exception ex)
                            {
                                MessageBox.Show("Read words MDBS: " + ex.Message);
                            }

                            //Thread.Sleep(milliseconds);
                            try
                            {
                                CONTROL_READ = modbus.ReadCoils(2000, 16);
                            }
                            
                            catch (Exception ex)
                            {
                                MessageBox.Show("Read bits MDBS: " + ex.Message);
                            }
                            bit1 = CONTROL_READ[0]; //M2000
                            Rilecart_stop = CONTROL_READ[1]; //M2001
                            bit3 = CONTROL_READ[2]; //M2002
                            bit4 = CONTROL_READ[3]; //M2003
                            bit5 = CONTROL_READ[4]; //M2004
                            Collator_Missing_left = CONTROL_READ[5]; //M2005
                            Collator_Missing_right = CONTROL_READ[6]; //M2006
                            Collator_Paper_jam = CONTROL_READ[7]; //M2007
                            Rilecart_emergency_stop = CONTROL_READ[8]; //M2008
                            Rilecart_missing_fin = CONTROL_READ[9]; //M2009
                            bit11 = CONTROL_READ[10]; //M2010
                            bit12 = CONTROL_READ[11]; //M2011
                            Collator_bypass = CONTROL_READ[12]; //M2012
                            Rilecart_bypass = CONTROL_READ[13]; //M2013
                            Rotation_paper_jam = CONTROL_READ[14]; //M2014
                            bit2 = CONTROL_READ[15]; //M2014


                            if (CONTROL_READ[0] == true) // GREEN UP
                            {
                                this.label9.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[0] == false) // STANDART UP
                            {
                                this.label9.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[1] == true) // GREEN DOWN
                            {
                                this.label10.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[1] == false) // STANDART DOWN
                            {
                                this.label10.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[2] == true) // GREEN DOWN
                            {
                                this.label17.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[2] == false) // STANDART DOWN
                            {
                                this.label17.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[3] == true) // GREEN DOWN
                            {
                                this.label18.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[3] == false) // STANDART DOWN
                            {
                                this.label18.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[4] == true) // GREEN DOWN
                            {
                                this.label24.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[4] == false) // STANDART DOWN
                            {
                                this.label24.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[5] == true) // GREEN DOWN
                            {
                                this.label23.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[5] == false) // STANDART DOWN
                            {
                                this.label23.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[6] == true) // GREEN DOWN
                            {
                                this.label20.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[6] == false) // STANDART DOWN
                            {
                                this.label20.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[7] == true) // GREEN DOWN
                            {
                                this.label19.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[7] == false) // STANDART DOWN
                            {
                                this.label19.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[8] == true) // GREEN DOWN
                            {
                                this.label32.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[8] == false) // STANDART DOWN
                            {
                                this.label32.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[9] == true) // GREEN DOWN
                            {
                                this.label31.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[9] == false) // STANDART DOWN
                            {
                                this.label31.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[10] == true) // GREEN DOWN
                            {
                                this.label12.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[10] == false) // STANDART DOWN
                            {
                                this.label12.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[11] == true) // GREEN DOWN
                            {
                                this.label11.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[11] == false) // STANDART DOWN
                            {
                                this.label11.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[12] == true) // GREEN DOWN
                            {
                                this.label14.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[12] == false) // STANDART DOWN
                            {
                                this.label14.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[13] == true) // GREEN DOWN
                            {
                                this.label13.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[13] == false) // STANDART DOWN
                            {
                                this.label13.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[14] == true) // GREEN DOWN
                            {
                                this.label47.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[14] == false) // STANDART DOWN
                            {
                                this.label47.BackColor = System.Drawing.Color.White;
                            }
                            if (CONTROL_READ[15] == true) // GREEN DOWN
                            {
                                this.label48.BackColor = System.Drawing.Color.Green;
                            }
                            else if (CONTROL_READ[15] == false) // STANDART DOWN
                            {
                                this.label48.BackColor = System.Drawing.Color.White;
                            }
                            //if (START == true & HOME == false) // GREEN START, STANDART STOP, STANDART HOME
                            //{
                            //    this.button1.Image = global::TCP_LISTENER_Delta.Properties.Resources.Group_23;
                            //    this.button2.Image = global::TCP_LISTENER_Delta.Properties.Resources.Group_22;
                            //    this.button3.Image = global::TCP_LISTENER_Delta.Properties.Resources.Group_26;
                            //}
                            //if (START == false) // RED STOP, STANDART START,STANDART HOME
                            //{
                            //    this.button1.Image = global::TCP_LISTENER_Delta.Properties.Resources.Group_21;
                            //    this.button2.Image = global::TCP_LISTENER_Delta.Properties.Resources.Group_24;
                            //    this.button3.Image = global::TCP_LISTENER_Delta.Properties.Resources.Group_26;
                            //}
                            //if (HOME == true & START == false) // GREEN HOME, STANDART STOP, STANDART START
                            //{
                            //    this.button2.Image = global::TCP_LISTENER_Delta.Properties.Resources.Group_22;
                            //    this.button3.Image = global::TCP_LISTENER_Delta.Properties.Resources.Group_27;
                            //    this.button1.Image = global::TCP_LISTENER_Delta.Properties.Resources.Group_21;
                            //}
                            //if (InvokeRequired)
                            //{
                            //    Invoke(new Action(() =>
                            //    {
                            //        if (tabControl1.Controls[0] == tabControl1.SelectedTab) // AUTO MODE ON
                            //            CONTROL[4] = true;
                            //        if (tabControl1.Controls[1] == tabControl1.SelectedTab) // AUTO MODE OFF
                            //            CONTROL[4] = false;
                            //        if (tabControl1.Controls[2] == tabControl1.SelectedTab) // AUTO MODE OFF
                            //            CONTROL[4] = false;
                            //    }));
                            //}
                            //else
                            //{
                            //    if (tabControl1.Controls[0] == tabControl1.SelectedTab) // AUTO MODE ON
                            //        CONTROL[4] = true;
                            //    if (tabControl1.Controls[1] == tabControl1.SelectedTab) // AUTO MODE OFF
                            //        CONTROL[4] = false;
                            //    if (tabControl1.Controls[2] == tabControl1.SelectedTab) // AUTO MODE OFF
                            //        CONTROL[4] = false;
                            //}
                            //if (Solenoid == true) // GREEN RIGHT
                            //{
                            //    this.label8.BackColor = System.Drawing.Color.Green;
                            //}
                            //else if (Solenoid == false) // STANDART RIGHT
                            //{
                            //    this.label8.BackColor = System.Drawing.Color.White;
                            //}
                            //if (UpLim == true) // GREEN RIGHT
                            //{
                            //    this.label11.BackColor = System.Drawing.Color.Green;
                            //}
                            //else if (UpLim == false) // STANDART RIGHT
                            //{
                            //    this.label11.BackColor = System.Drawing.Color.White;
                            //}
                            //if (DownLim == true) // GREEN RIGHT
                            //{
                            //    this.label12.BackColor = System.Drawing.Color.Green;
                            //}
                            //else if (DownLim == false) // STANDART RIGHT
                            //{
                            //    this.label12.BackColor = System.Drawing.Color.White;
                            //}
                            //if (PapLim == true) // GREEN RIGHT
                            //{
                            //    this.label14.BackColor = System.Drawing.Color.Green;
                            //    this.label15.BackColor = System.Drawing.Color.Green;
                            //}
                            //else if (PapLim == false) // STANDART RIGHT
                            //{
                            //    this.label14.BackColor = System.Drawing.Color.White;
                            //    this.label15.BackColor = System.Drawing.Color.White;
                            //}
                            //if (PresLim == true) // GREEN RIGHT
                            //{
                            //    this.label13.BackColor = System.Drawing.Color.Green;
                            //    this.label16.BackColor = System.Drawing.Color.Green;
                            //}
                            //else if (PresLim == false) // STANDART RIGHT
                            //{
                            //    this.label13.BackColor = System.Drawing.Color.White;
                            //    this.label16.BackColor = System.Drawing.Color.White;
                            //}
                        }
                        else if (check1 == false)
                        {
                             modbus.Disconnect();
                        }
                    }
                    catch (Exception ex) when (ex.Source == "mscorlib")
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        string msg = Convert.ToString(ex.InnerException);
                        MessageBox.Show("Read MDBS: " + ex.Message);
                    }
                }
           
            }
        }
        /// 
        /// Write data to box, from modbus
        /// 
        void WriteMDBS(string name)
        {
            string Collator1 = null;
            string Collator2 = null;
            string Collator3 = null;
            string Collator4 = null;
            string Rilecart1 = null;
            string Rilecart2 = null;
            string Rilecart3 = null;
            string Rilecart4 = null;
            string Rotation = null;
            string RunStat1 = null;
            string RunStat2 = null;
            string RunStat3 = null;
            while (true)
            {
                try
                {

                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                    {
                        label2.Text = "Current template file: " + templateFile;
                        pictureBox2.Refresh();
                        if (modbus.Connected == true)
                        {
                            lblStat.Text = "Status: Connected";
                            this.btnStart.Image = global::CollatorCAM.Properties.Resources.Group_9;
                            btnStart.Refresh();
                            lblStat.Refresh();
                        }
                            
                        else if (modbus.Connected == false)
                        {
                            lblStat.Text = "Status: Disconnected";
                            this.btnStart.Image = global::CollatorCAM.Properties.Resources.Group_10;
                            btnStart.Refresh();
                            lblStat.Refresh();
                        }
                            
                        //MotorSpeedSliderControl.Value = Properties.Settings.Default.SpeedPERC;// PERCENT OF MAIN SPEED = SAVED VALUE
                        //motorSpeed = MotorSpeedSliderControl.Value;                           //
                        //trackBar2.Value = Properties.Settings.Default.RailcartSave;            // MANUAL SPEED Y = SAVED VALUE
                        //motorSpeedY = trackBar2.Value;

                        DateTime thisDay = DateTime.Now;
                        textBox3.Text = thisDay.ToString("d") + @"
" + thisDay.ToString("T");
                        textBox6.Text = "["+ JobID + "] - [" + SchoolName + "]"+ @"
" + "Calendar type: " + CalendarType;

                        if (run == true)
                        {
                            RunStat1 = "RUN";
                            RunStat2 = "RUN";
                            RunStat3 = "RUN";
                        }
                        else if (run == false)
                        {
                            RunStat1 = "STOP";
                            RunStat2 = "STOP";
                            RunStat3 = "STOP";
                        }
                        if (Collator_Missing_left == true)
                        {
                            Collator1 = "Missing left" + @"
";
                        }
                        if (Collator_Missing_left == false)
                        {
                            Collator1 = null;
                        }
                        if (Collator_Missing_right == true)
                        {
                            Collator2 = "Missing right" + @"
";
                        }
                        if (Collator_Missing_right == false)
                        {
                            Collator2 = null;
                        }
                        if (Collator_Paper_jam == true)
                        {
                            Collator3 = "Paper jam" + @"
";
                        }
                        if (Collator_Paper_jam == false)
                        {
                            Collator3 = null;
                        }
                        if (Collator_bypass == true)
                        {
                            Collator4 = "Collator bypass";
                        }
                        if (Collator_bypass == false)
                        {
                            Collator4 = null;
                        }


                        if (run == true && Collator_Paper_jam == true || Collator_Missing_right == true || Collator_Missing_left == true)
                        {
                            textBox14.BackColor = System.Drawing.Color.FromArgb(218, 67, 60);
                            textBox14.Text = Collator1 + Collator2 + Collator3 + Collator4;
                            textBox14.ForeColor = System.Drawing.Color.White;
                        }
                        if (run == true && Collator_Paper_jam == false && Collator_Missing_right == false && Collator_Missing_left == false)
                        {
                            textBox14.BackColor = System.Drawing.Color.FromArgb(45, 232, 14);
                            textBox14.Text = Collator4 + @"
" + RunStat3;
                            textBox14.ForeColor = System.Drawing.Color.Black;
                        }
                        if (run == false && Collator_Paper_jam == false && Collator_Missing_right == false && Collator_Missing_left == false)
                        {
                            textBox14.BackColor = System.Drawing.Color.FromArgb(218, 218, 218);
                            textBox14.Text = Collator4 + @"
" + RunStat3;
                            textBox14.ForeColor = System.Drawing.Color.Black;
                        }
                        ////////////////////////////////////////////////////
                        if (Rilecart_emergency_stop == true)
                        {
                            Rilecart1 = "Emergency stop" + @"
";
                        }
                        if (Rilecart_emergency_stop == false)
                        {
                            Rilecart1 = null;
                        }
                        if (Rilecart_missing_fin == true)
                        {
                            Rilecart2 = "Missing fin" + @"
";
                        }
                        if (Rilecart_missing_fin == false)
                        {
                            Rilecart2 = null;
                        }

                        if (Rilecart_stop == true)
                        {
                            Rilecart3 = "Rilecart STOP" + @"
";
                        }
                        if (Rilecart_stop == false)
                        {
                            Rilecart3 = null;
                        }
                        if (Rilecart_bypass == true)
                        {
                            Rilecart4 = "Rilecart bypass";
                        }
                        if (Rilecart_bypass == false)
                        {
                            Rilecart4 = null;
                        }

                        if (run == true && Rilecart_emergency_stop == true || Rilecart_missing_fin == true || Rilecart_stop == true)
                        {
                            textBox11.BackColor = System.Drawing.Color.FromArgb(218, 67, 60);
                            textBox11.Text = Rilecart1 + Rilecart2 + Rilecart3 + Rilecart4;
                            textBox11.ForeColor = System.Drawing.Color.White;
                        }
                        if (run == true && Rilecart_emergency_stop == false && Rilecart_missing_fin == false && Rilecart_stop == false)
                        {
                            textBox11.BackColor = System.Drawing.Color.FromArgb(45, 232, 14);
                            textBox11.Text = Rilecart4 + @"
" + RunStat1;
                            textBox11.ForeColor = System.Drawing.Color.Black;
                        }
                        if (run == false && Rilecart_emergency_stop == false && Rilecart_missing_fin == false && Rilecart_stop == false)
                        {
                            textBox11.BackColor = System.Drawing.Color.FromArgb(218, 218, 218);
                            textBox11.Text = Rilecart4 + @"
" + RunStat1;
                            textBox11.ForeColor = System.Drawing.Color.Black;
                        }
                        /////////////////////////////////////////////////////////////////////////////////////////////////
                        if (run == true && Rotation_paper_jam == true)
                        {
                            Rotation = "Rotation paper jam";
                            textBox12.BackColor = System.Drawing.Color.FromArgb(218, 67, 60);
                            textBox12.ForeColor = System.Drawing.Color.White;
                            textBox12.Text = Rotation;

                        }
                        if (run == true && Rotation_paper_jam == false)
                        {
                            Rotation = null;
                            textBox12.BackColor = System.Drawing.Color.FromArgb(45, 232, 14);
                            textBox12.ForeColor = System.Drawing.Color.Black;
                            textBox12.Text = Rotation + @"
" + RunStat2;
                        }
                        if (run == false && Rotation_paper_jam == false)
                        {
                            Rotation = null;
                            textBox12.BackColor = System.Drawing.Color.FromArgb(218, 218, 218);
                            textBox12.ForeColor = System.Drawing.Color.Black;
                            textBox12.Text = Rotation + @"
" + RunStat2;
                        }
                        /////////////////////////////////////////////////////////////////////////////////////////////////
                        if (Collator_Paper_jam == true || Collator_Missing_right == true || Collator_Missing_left == true || Rilecart_emergency_stop == true || Rilecart_missing_fin == true || Rilecart_stop == true || Rotation_paper_jam == true)
                        {
                            pictureBox3.Image = Resources.Rectangle_146__1_;
                        }
                        else if (run == true && Collator_Paper_jam == false && Collator_Missing_right == false && Collator_Missing_left == false && Rilecart_emergency_stop == false && Rilecart_missing_fin == false && Rilecart_stop == false && Rotation_paper_jam == false)
                        {
                            pictureBox3.Image = Resources.Rectangle_147__2_;

                        }
                        else if (run == false && Collator_Paper_jam == false && Collator_Missing_right == false && Collator_Missing_left == false && Rilecart_emergency_stop == false && Rilecart_missing_fin == false && Rilecart_stop == false && Rotation_paper_jam == false)
                        {
                            pictureBox3.Image = null;
                        }

                        //                        textBox30.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox31.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox32.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox33.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox34.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox35.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox43.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox46.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox47.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox48.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox49.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox50.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox51.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";

                        if (modbus.Connected == false)
                        {
                            textBox9.Text = "";
                        }
                        ApplySettings();
                        
                    }));
                    }
                    else
                    {
                        label2.Text = "Current template file: " + templateFile;
                        
                        pictureBox2.Refresh();
                        if (modbus.Connected == true)
                        {
                            lblStat.Text = "Status: Connected";
                            this.btnStart.Image = global::CollatorCAM.Properties.Resources.Group_9;
                            btnStart.Refresh();
                            lblStat.Refresh();
                        }

                        else if (modbus.Connected == false)
                        {
                            lblStat.Text = "Status: Disconnected";
                            this.btnStart.Image = global::CollatorCAM.Properties.Resources.Group_10;
                            btnStart.Refresh();
                            lblStat.Refresh();
                        }
                        //MotorSpeedSliderControl.Value = Properties.Settings.Default.SpeedPERC;
                        //motorSpeed = MotorSpeedSliderControl.Value;
                        //trackBar2.Value = Properties.Settings.Default.RailcartSave;
                        //motorSpeedY = trackBar2.Value;

                        DateTime thisDay = DateTime.Now;
                        textBox3.Text = thisDay.ToString("d") + @"
" + thisDay.ToString("T");
                        textBox6.Text = "[" + JobID + "] - [" + SchoolName + "]" + @"
" + "Calendar type: " + CalendarType;

                        if (run == true)
                        {
                            RunStat1 = "RUN";
                            RunStat2 = "RUN";
                            RunStat3 = "RUN";

                        }
                        else if (run == false)
                        {
                            RunStat1 = "STOP";
                            RunStat2 = "STOP";
                            RunStat3 = "STOP";
                        }

                        if (Collator_Missing_left == true)
                        {
                            Collator1 = "Missing left" + @"
";
                        }
                        if (Collator_Missing_left == false)
                        {
                            Collator1 = null;
                        }
                        if (Collator_Missing_right == true)
                        {
                            Collator2 = "Missing right" + @"
";
                        }
                        if (Collator_Missing_right == false)
                        {
                            Collator2 = null;
                        }
                        if (Collator_Paper_jam == true)
                        {
                            Collator3 = "Paper jam" + @"
";
                        }
                        if (Collator_Paper_jam == false)
                        {
                            Collator3 = null;
                        }
                        if (Collator_bypass == true)
                        {
                            Collator4 = "Collator bypass";
                        }
                        if (Collator_bypass == false)
                        {
                            Collator4 = null;
                        }


                        if (run == true && Collator_Paper_jam == true || Collator_Missing_right == true || Collator_Missing_left == true)
                        {
                            textBox14.BackColor = System.Drawing.Color.FromArgb(218, 67, 60);
                            textBox14.Text = Collator1 + Collator2 + Collator3 + Collator4;
                            textBox14.ForeColor = System.Drawing.Color.White;
                        }
                        if (run == true && Collator_Paper_jam == false && Collator_Missing_right == false && Collator_Missing_left == false)
                        {
                            textBox14.BackColor = System.Drawing.Color.FromArgb(45, 232, 14);
                            textBox14.Text = Collator4 + @"
" + RunStat3;
                            textBox14.ForeColor = System.Drawing.Color.Black;
                        }
                        if (run == false && Collator_Paper_jam == false && Collator_Missing_right == false && Collator_Missing_left == false)
                        {
                            textBox14.BackColor = System.Drawing.Color.FromArgb(218, 218, 218);
                            textBox14.Text = Collator4 + @"
" + RunStat3;
                            textBox14.ForeColor = System.Drawing.Color.Black;
                        }
                        ////////////////////////////////////////////////////
                        if (Rilecart_emergency_stop == true)
                        {
                            Rilecart1 = "Emergency stop" + @"
";
                        }
                        if (Rilecart_emergency_stop == false)
                        {
                            Rilecart1 = null;
                        }
                        if (Rilecart_missing_fin == true)
                        {
                            Rilecart2 = "Missing fin" + @"
";
                        }
                        if (Rilecart_missing_fin == false)
                        {
                            Rilecart2 = null;
                        }

                        if (Rilecart_stop == true)
                        {
                            Rilecart3 = "Rilecart STOP" + @"
";
                        }
                        if (Rilecart_stop == false)
                        {
                            Rilecart3 = null;
                        }
                        if (Rilecart_bypass == true)
                        {
                            Rilecart4 = "Rilecart bypass";
                        }
                        if (Rilecart_bypass == false)
                        {
                            Rilecart4 = null;
                        }

                        if (run == true && Rilecart_emergency_stop == true || Rilecart_missing_fin == true || Rilecart_stop == true)
                        {
                            textBox11.BackColor = System.Drawing.Color.FromArgb(218, 67, 60);
                            textBox11.Text = Rilecart1 + Rilecart2 + Rilecart3 + Rilecart4;
                            textBox11.ForeColor = System.Drawing.Color.White;
                        }
                        if (run == true && Rilecart_emergency_stop == false && Rilecart_missing_fin == false && Rilecart_stop == false)
                        {
                            textBox11.BackColor = System.Drawing.Color.FromArgb(45, 232, 14);
                            textBox11.Text = Rilecart4 + @"
" + RunStat1;
                            textBox11.ForeColor = System.Drawing.Color.Black;
                        }
                        if (run == false && Rilecart_emergency_stop == false && Rilecart_missing_fin == false && Rilecart_stop == false)
                        {
                            textBox11.BackColor = System.Drawing.Color.FromArgb(218, 218, 218);
                            textBox11.Text = Rilecart4 + @"
" + RunStat1;
                            textBox11.ForeColor = System.Drawing.Color.Black;
                        }

                        /////////////////////////////////////////////////////////////////////////////////////////////////

                        if (run == true && Rotation_paper_jam == true)
                        {
                            Rotation = "Rotation paper jam";
                            textBox12.BackColor = System.Drawing.Color.FromArgb(218, 67, 60);
                            textBox12.ForeColor = System.Drawing.Color.White;
                            textBox12.Text = Rotation;

                        }
                        if (run == true && Rotation_paper_jam == false)
                        {
                            Rotation = null;
                            textBox12.BackColor = System.Drawing.Color.FromArgb(45, 232, 14);
                            textBox12.ForeColor = System.Drawing.Color.Black;
                            textBox12.Text = Rotation + @"
" + RunStat2;
                        }
                        if (run == false && Rotation_paper_jam == false)
                        {
                            Rotation = null;
                            textBox12.BackColor = System.Drawing.Color.FromArgb(218, 218, 218);
                            textBox12.ForeColor = System.Drawing.Color.Black;
                            textBox12.Text = Rotation + @"
" + RunStat2;
                        }

                        /////////////////////////////////////////////////////////////////////////////////////////////////

                        if (Collator_Paper_jam == true || Collator_Missing_right == true || Collator_Missing_left == true || Rilecart_emergency_stop == true || Rilecart_missing_fin == true || Rilecart_stop == true || Rotation_paper_jam == true)
                        {
                            pictureBox3.Image = Resources.Rectangle_146__1_;
                        }
                        else if (run == true && Collator_Paper_jam == false && Collator_Missing_right == false && Collator_Missing_left == false && Rilecart_emergency_stop == false && Rilecart_missing_fin == false && Rilecart_stop == false && Rotation_paper_jam==false)
                        {
                            pictureBox3.Image = Resources.Rectangle_147__2_;

                        }
                        else if (run == false && Collator_Paper_jam == false && Collator_Missing_right == false && Collator_Missing_left == false && Rilecart_emergency_stop == false && Rilecart_missing_fin == false && Rilecart_stop == false && Rotation_paper_jam == false)
                        {
                            pictureBox3.Image = null;
                        }

                        //                        textBox30.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox31.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox32.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox33.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox34.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox35.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox43.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox46.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox47.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox48.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox49.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox50.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";
                        //                        textBox51.Text = "Pos: " + "1" + @"
                        //" + "[" + "Month" + "]";


                        if (modbus.Connected == false)
                        {
                            textBox9.Text = "";
                        }
                        ApplySettings();
                    }
                }
                catch (Exception ex) when (ex.Source == "mscorlib")
                {
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Write trackbar from memory: "+ex.Message);
                }
                if (modbus.Connected == true)
                {
                    try
                    {

                        //if (D[2] < 0)
                        //    resultX = (unchecked((uint)D[2]) - 4294901762);
                        //if (D[2] >= 0)
                        //    resultX = (unchecked((uint)D[2]));
                        //if (D[30] < 0)
                        //   result = (unchecked((uint)D[30]) - 4294901762);
                        //if (D[30] >= 0)
                        //    result = (unchecked((uint)D[30]));
                        //if (D[10] < 0)
                        //    resultY = (unchecked((uint)D[10]) - 4294901762);
                        //if (D[10] >= 0)
                        //    resultY = (unchecked((uint)D[10]));

                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                textBox9.Text = WORD_READ[10].ToString(); //M1 SPEED (Manual Speed Y)
                                

                            }));
                        }

                        else
                        {
                            textBox9.Text = WORD_READ[10].ToString(); //M1 SPEED (Manual Speed Y)

                        }

                    }
                    catch (Exception ex) when (ex.Source == "mscorlib")
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Positions from modbus: "+ex.Message);
                    }
                }
            }
        }
        /// 
        /// Write X ACC
        /// 
        private void textBox45_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox45.Text == "")
            {
                return;
            }
            else if (textBox45.Text != "")
            {
                Int32 txt = Convert.ToInt32(textBox45.Text);
                if (txt > 1000)
                {
                    MessageBox.Show("Set value from 0 to 1000");
                    txt1 = 1000;
                    textBox45.Text = "1000";
                }
                else if (txt <= 1000)
                    txt1 = txt;
            }
            Properties.Settings.Default.ACC_Y_MAN = textBox45.Text;
            Properties.Settings.Default.Save();
        }
        private void CheckEnter(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0') && (e.KeyChar <= '9') || (e.KeyChar == '.') || (e.KeyChar == (char)Keys.Back)) //|| (e.KeyChar == '-')
            {
                return;
            }
            e.Handled = true;
        }
        /// 
        /// Write X DEC
        /// 
        private void textBox44_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox44.Text == "")
            {
                return;
            }
            else if (textBox44.Text != "")
            {
                Int32 txt = Convert.ToInt32(textBox44.Text);
                if (txt > 1000)
                {
                    MessageBox.Show("Set value from 0 to 1000");
                    txt2 = 1000;
                    textBox44.Text = "1000";
                }
                else if (txt <= 1000)
                    txt2 = txt;
            }
            Properties.Settings.Default.DEC_Y_MAN = textBox44.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// BTN UP
        /// 
        private void btnUP_Down(object sender, EventArgs e)
        {
            if (modbus.Connected == true)
            {
                if (ManSpd > 0)
                {
                   BoolUp = true;
                }
                else if (ManSpd == 0)
                {
                   BoolUp = false;
                   MessageBox.Show("set speed greater than 0");
                }
            }
            else if (modbus.Connected == false)
            {
                MessageBox.Show("plc disabled");
            }

        }
        private void btnUP_Up(object sender, EventArgs e)
        {
            BoolUp = false;
        }

        ///// 
        ///// BTN DOWN
        ///// 
        private void btnDWN_Down(object sender, EventArgs e)
        {
            if (modbus.Connected == true)
            {
                if (ManSpd > 0)
                {
                    BoolDwn = true;
                }
               else if (ManSpd == 0)
                {
                    BoolDwn = false;
                    MessageBox.Show("Set speed greater than 0");
                }
            }
            else if (modbus.Connected == false)
            {
                MessageBox.Show("PLC disabled");
            }

        }
        private void btnDWN_Up(object sender, EventArgs e)
        {
            BoolDwn = false;
        }
        /// 
        /// WHITE BACKGROUND IF MOUSE ENTER
        /// 
        private void btnDWN_ENTER(object sender, EventArgs e)
        {
            btnDWN.BackColor = System.Drawing.Color.White;
        }
        /// 
        /// WHITE BACKGROUND IF MOUSE ENTER
        /// 
        private void btnUp_ENTER(object sender, EventArgs e)
        {
            btnUP.BackColor = System.Drawing.Color.White;
        }
        /// 
        /// READ TXT FROM PICTURE
        /// 
        private void btnTXT_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    Tesseract tesseract = new Tesseract(@tesser, "eng", OcrEngineMode.TesseractLstmCombined);
            //    tesseract.SetImage(new Image<Bgr, byte>(pictureBox1.ImageLocation));
            //    tesseract.Recognize();
            //    richTextBox1.Text = tesseract.GetUTF8Text();
            //    tesseract.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("tesseract"+ex.Message);
            //}
        }
        /// 
        /// MAIN SPEED Y1
        /// 
        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            {

                if (textBox16.Text == "")
                {
                    return;
                }
                else if (textBox16.Text != "")
                {
                    Int32 txt = Convert.ToInt32(textBox16.Text);
                    if (txt > 20000)
                    {
                        MessageBox.Show("Set value from 0 to 20000");
                        txt7 = 20000;
                        textBox16.Text = "20000";
                    }
                    else if (txt <= 20000)
                        txt7 = txt;
                }
            }
            Properties.Settings.Default.SpeedY1 = textBox16.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// RELEASE TIMER INPUT
        /// 
        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            {

                if (textBox17.Text == "")
                {
                    return;
                }
                else if (textBox17.Text != "")
                {
                    Int32 txt = Convert.ToInt32(textBox17.Text);
                    if (txt > 20000)
                    {
                        MessageBox.Show("Set value from 0 to 20000");
                        txt8 = 20000;
                        textBox17.Text = "20000";
                    }
                    else if (txt <= 20000)
                        txt8 = txt;
                }
            }
            Properties.Settings.Default.SpeedY2 = textBox17.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// RELEASE POSITION Y INPUT
        /// 
        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            if (textBox24.Text == "")
            {
                return;
            }
            else if (textBox24.Text != "")
            {
                Int32 txt = Convert.ToInt32(textBox24.Text);
                if (txt > 20000)
                {
                    MessageBox.Show("Set value from 0 to 20000");
                    txt9 = 20000;
                    textBox24.Text = "20000";
                }
                else if (txt <= 20000)
                    txt9 = txt;
            }
            Properties.Settings.Default.POS1 = textBox24.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// SCAN POSITION Y INPUT
        /// 
        private void textBox23_TextChanged(object sender, EventArgs e)
        {
            if (textBox23.Text == "")
            {
                return;
            }
            else if (textBox23.Text != "")
            {
                Int32 txt = Convert.ToInt32(textBox23.Text);
                if (txt > 20000)
                {
                    MessageBox.Show("Set value from 0 to 20000");
                    txt10 = 20000;
                    textBox23.Text = "20000";
                }
                else if (txt <= 20000)
                    txt10 = txt;
            }
            Properties.Settings.Default.POS2 = textBox23.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// GRAB POSITION Y INPUT
        /// 
        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            if (textBox22.Text == "")
            {
                return;
            }
            else if (textBox22.Text != "")
            {
                Int32 txt = Convert.ToInt32(textBox22.Text);
                if (txt > 20000)
                {
                    MessageBox.Show("Set value from 0 to 20000");
                    txt11 = 20000;
                    textBox22.Text = "20000";
                }
                else if (txt <= 20000)
                    txt11 = txt;
            }
            Properties.Settings.Default.POS3 = textBox22.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// PORT INPUT
        /// 
        private void txtPort_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Port = txtPort.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// IP INPUT
        /// 
        private void txtIPAdress_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IP = txtIPAdress.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// GRAB TIMER INPUT
        /// 
        private void textBox27_TextChanged(object sender, EventArgs e)
        {
            if (textBox27.Text == "")
            {
                return;
            }
            else if (textBox27.Text != "")
            {
                Int32 txt = Convert.ToInt32(textBox27.Text);
                if (txt > 20000)
                {
                    MessageBox.Show("Set value from 0 to 20000");
                    Grab_TMR = 20000;
                    textBox27.Text = "20000";
                }
                else if (txt <= 20000)
                    Grab_TMR = txt;
            }
            Properties.Settings.Default.GrabTMR = textBox27.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// SCAN TIMER INPUT
        /// 
        private void textBox26_TextChanged(object sender, EventArgs e)
        {
            if (textBox26.Text == "")
            {
                return;
            }
            else if (textBox26.Text != "")
            {
                Int32 txt = Convert.ToInt32(textBox26.Text);
                if (txt > 20000)
                {
                    MessageBox.Show("Set value from 0 to 20000");
                    Scan_TMR = 20000;
                    textBox26.Text = "20000";
                }
                else if (txt <= 20000)
                    Scan_TMR = txt;
            }
            Properties.Settings.Default.ScanTMR = textBox26.Text;
            Properties.Settings.Default.Save();
        }
        /// 
        /// RELEASE TIMER INPUT
        /// 
        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            if (textBox28.Text == "")
            {
                return;
            }
            else if (textBox22.Text != "")
            {
                Int32 txt = Convert.ToInt32(textBox28.Text);
                if (txt > 20000)
                {
                    MessageBox.Show("Set value from 0 to 20000");
                    Release_TMR = 20000;
                    textBox28.Text = "20000";
                }
                else if (txt <= 20000)
                    Release_TMR = txt;
            }
            Properties.Settings.Default.ReleaseTMR = textBox28.Text;
            Properties.Settings.Default.Save();
        }


        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            tesser = textBox5.Text;
            Properties.Settings.Default.tesPath = textBox5.Text;
            Properties.Settings.Default.Save();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (modbus.Connected == true)
            {
                //if (Solenoid == false)
                //{
                //    CONTROL[9] = !CONTROL[9];
                //}
                //if (Solenoid == true)
                //{
                //    CONTROL[9] = false;
               // }
                  
            }
            else if (modbus.Connected == false)
            {
                MessageBox.Show("PLC disabled");
            }
        }
        
        private void SOL_Down(object sender, EventArgs e)
        {
          //  if (modbus.Connected == true)
          //  {
//
          //          CONTROL[9] = true;
          //  }
          //  else if (modbus.Connected == false)
          //  {
          //      MessageBox.Show("PLC disabled");
          //  }
        }
        private void SOL_Up(object sender, EventArgs e)
        {
          //  CONTROL[9] = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                return;
            }
            else if (textBox2.Text != "")
            {
                Int32 txt = Convert.ToInt32(textBox2.Text);
                if (txt > 20000 | txt < -20000)
                {
                    MessageBox.Show("Set value from -20000 to 20000");
                    Hi = 0;
                    textBox2.Text = "0";
                }
                else if (txt > -20000 | txt < 20000)
                    Hi = txt;
            }
            Properties.Settings.Default.Height = textBox2.Text;
            Properties.Settings.Default.Save();
        }

        private void Form_Listener_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void textBox7_TextChanged_1(object sender, EventArgs e)
        {
            JobID = textBox7.Text.ToString();
        }

        private void textBox8_TextChanged_1(object sender, EventArgs e)
        {
            SchoolName = textBox8.Text.ToString();
        }

        private void textBox10_TextChanged_1(object sender, EventArgs e)
        {
            CalendarType = textBox10.Text.ToString();
        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ManSpd = trackBar1.Value;
            textBox4.Text = ManSpd.ToString();
            Properties.Settings.Default.ManSpdSave = ManSpd;
            Properties.Settings.Default.Save();
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            BoolCollator = checkBox12.Checked;
            Properties.Settings.Default.CollatorSave = BoolCollator;
            Properties.Settings.Default.Save();
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            BoolRailcart = checkBox11.Checked;
            Properties.Settings.Default.RailcartSave = BoolRailcart;
            Properties.Settings.Default.Save();
        }

        private void Form_Listener_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                check1 = false;
                Thread.Sleep(100);
                modbus.Disconnect();
                thread1.Abort(); ;
                thread2.Abort();
            }
            catch
            {
                return;
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            run = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            run = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.Text == "0")
            {
                angle0 = true;
                angle90 = false;
                angle180 = false;
            }
            if (listBox1.Text == "90")
            {
                angle0 = false;
                angle90 = true;
                angle180 = false;
            }
            if (listBox1.Text == "180")
            {
                angle0 = false;
                angle90 = false;
                angle180 = true;
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
            if (cbCycleCapture.Checked || PhotoMonth == true)
            {
                string TBtime = null;
                string TBmonth = null;
                string TByear = null;
                string TBmaori = null;
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
                        if (found.template.name == "January" || found.template.name == "February")
                            TBmonth = found.template.name;
                        if (found.template.name == "2024")
                        {
                            TByear = found.template.name;
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
                                TBmaori = "Maori";
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
                if (TBmonth == null)
                    TBmonth = "Month NG";
                if (TByear == null)
                    TBmonth = "Year NG";
                if (TBmaori == null)
                    TBmonth = "Maori NG";
                tbResult.Items.Add(TBtime + @"
" + TBmonth + @"
" + TByear + @"
" + TBmaori);
                PhotoMonth = false;
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
                    frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i-1]));
                    ImagePath = "Current image file: " + GetImagePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }
        private void btnPhoto_Click(object sender, EventArgs e)
        {
            PhotoMonth = true;
            //btnPhoto.Click += new System.EventHandler(this.button1_Click_1);
            //if (FoundTemplateName == "January")
            //    textBox42.BackColor = Color.Green;
            ////GetImagePath = Properties.Settings.Default.JanuaryGetImagePath;
            ////GetImage();

            //btnPhoto.Click += new System.EventHandler(this.button2_Click_1);
            //if (FoundTemplateName == "February")
            //    textBox46.BackColor = Color.Green;
            ////GetImagePath = Properties.Settings.Default.FebruaryGetImagePath;
            ////GetImage();

        }
        private void ScanCycle()
        {             

        }
        private void GetImage()
        {
            if (GetImagePath != null)
            {
                files = Directory.GetFiles(GetImagePath);
                i = files.Length;
                frame = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(files[i - 1]));
                ImagePath = "Current image file: " + GetImagePath;
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
            Properties.Settings.Default.Save();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            month = 2;
            button1.BackColor = System.Drawing.Color.DarkGray;
            button2.BackColor = System.Drawing.Color.Transparent;

            if (Properties.Settings.Default.JanuaryAdaptiveThBlockSize == 0)
                Properties.Settings.Default.JanuaryAdaptiveThBlockSize = new decimal(new int[] {1,0,0,0}); ;
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
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            month = 3;
            button1.BackColor = System.Drawing.Color.Transparent;
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
        }


    }
}
     