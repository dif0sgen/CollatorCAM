using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CollatorCAM
{
    partial class Form_Listener
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ///<param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Listener));
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox45 = new System.Windows.Forms.TextBox();
            this.textBox44 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.tmUpdateState = new System.Windows.Forms.Timer(this.components);
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.lbFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbContoursCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbRecognized = new System.Windows.Forms.ToolStripStatusLabel();
            this.button3 = new System.Windows.Forms.Button();
            this.tbResult = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ibMain = new Emgu.CV.UI.ImageBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btNewTemplates = new System.Windows.Forms.ToolStripButton();
            this.btOpenTemplates = new System.Windows.Forms.ToolStripButton();
            this.btSaveTemplates = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.btCreateTemplate = new System.Windows.Forms.ToolStripButton();
            this.btAutoGenerate = new System.Windows.Forms.ToolStripButton();
            this.btTemplateEditor = new System.Windows.Forms.ToolStripButton();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.button7 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.textBox41 = new System.Windows.Forms.TextBox();
            this.textBox42 = new System.Windows.Forms.TextBox();
            this.textBox46 = new System.Windows.Forms.TextBox();
            this.textBox48 = new System.Windows.Forms.TextBox();
            this.textBox49 = new System.Windows.Forms.TextBox();
            this.textBox50 = new System.Windows.Forms.TextBox();
            this.textBox51 = new System.Windows.Forms.TextBox();
            this.textBox53 = new System.Windows.Forms.TextBox();
            this.textBox54 = new System.Windows.Forms.TextBox();
            this.textBox79 = new System.Windows.Forms.TextBox();
            this.textBox80 = new System.Windows.Forms.TextBox();
            this.textBox81 = new System.Windows.Forms.TextBox();
            this.textBox82 = new System.Windows.Forms.TextBox();
            this.textBox83 = new System.Windows.Forms.TextBox();
            this.pnSettings = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbShowBinarized = new System.Windows.Forms.CheckBox();
            this.cbShowContours = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.nudMaxACFdesc = new System.Windows.Forms.NumericUpDown();
            this.nudMinACF = new System.Windows.Forms.NumericUpDown();
            this.label34 = new System.Windows.Forms.Label();
            this.cbAllowAngleMore45 = new System.Windows.Forms.CheckBox();
            this.label35 = new System.Windows.Forms.Label();
            this.nudMinICF = new System.Windows.Forms.NumericUpDown();
            this.label38 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.nudMinDefinition = new System.Windows.Forms.NumericUpDown();
            this.cbNoiseFilter = new System.Windows.Forms.CheckBox();
            this.label39 = new System.Windows.Forms.Label();
            this.nudMinContourLength = new System.Windows.Forms.NumericUpDown();
            this.nudMinContourArea = new System.Windows.Forms.NumericUpDown();
            this.label46 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btLoadFolder = new System.Windows.Forms.Button();
            this.btnPhoto = new System.Windows.Forms.Button();
            this.cbCycleCapture = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.cbAdaptiveNoiseFilter = new System.Windows.Forms.CheckBox();
            this.nudAdaptiveThBlockSize = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.cbBlur = new System.Windows.Forms.CheckBox();
            this.label30 = new System.Windows.Forms.Label();
            this.cbCaptureFromCam = new System.Windows.Forms.CheckBox();
            this.cbCamResolution = new System.Windows.Forms.ComboBox();
            this.btLoadImage = new System.Windows.Forms.Button();
            this.cbAutoContrast = new System.Windows.Forms.CheckBox();
            this.cbShowAngle = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.tabPage5.SuspendLayout();
            this.ssMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibMain)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.pnSettings.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxACFdesc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinACF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinICF)).BeginInit();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinDefinition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinContourLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinContourArea)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdaptiveThBlockSize)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(115)))), ((int)(((byte)(115)))));
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox3, "textBox3");
            this.textBox3.ForeColor = System.Drawing.Color.White;
            this.textBox3.Name = "textBox3";
            // 
            // textBox45
            // 
            resources.ApplyResources(this.textBox45, "textBox45");
            this.textBox45.Name = "textBox45";
            // 
            // textBox44
            // 
            resources.ApplyResources(this.textBox44, "textBox44");
            this.textBox44.Name = "textBox44";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // textBox9
            // 
            this.textBox9.BackColor = System.Drawing.SystemColors.Control;
            this.textBox9.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox9, "textBox9");
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // trackBar2
            // 
            this.trackBar2.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.trackBar2, "trackBar2");
            this.trackBar2.LargeChange = 100;
            this.trackBar2.Maximum = 20000;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.SmallChange = 100;
            this.trackBar2.TickFrequency = 2000;
            // 
            // tmUpdateState
            // 
            this.tmUpdateState.Enabled = true;
            this.tmUpdateState.Interval = 1000;
            this.tmUpdateState.Tick += new System.EventHandler(this.tmUpdateState_Tick);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.ssMain);
            this.tabPage5.Controls.Add(this.button3);
            this.tabPage5.Controls.Add(this.tbResult);
            this.tabPage5.Controls.Add(this.label3);
            this.tabPage5.Controls.Add(this.label2);
            this.tabPage5.Controls.Add(this.ibMain);
            this.tabPage5.Controls.Add(this.toolStrip1);
            this.tabPage5.Controls.Add(this.flowLayoutPanel2);
            this.tabPage5.Controls.Add(this.pnSettings);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // ssMain
            // 
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbFPS,
            this.lbContoursCount,
            this.lbRecognized});
            resources.ApplyResources(this.ssMain, "ssMain");
            this.ssMain.Name = "ssMain";
            // 
            // lbFPS
            // 
            resources.ApplyResources(this.lbFPS, "lbFPS");
            this.lbFPS.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbFPS.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lbFPS.Name = "lbFPS";
            // 
            // lbContoursCount
            // 
            resources.ApplyResources(this.lbContoursCount, "lbContoursCount");
            this.lbContoursCount.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbContoursCount.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lbContoursCount.Name = "lbContoursCount";
            // 
            // lbRecognized
            // 
            resources.ApplyResources(this.lbRecognized, "lbRecognized");
            this.lbRecognized.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbRecognized.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lbRecognized.Name = "lbRecognized";
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tbResult
            // 
            this.tbResult.FormattingEnabled = true;
            resources.ApplyResources(this.tbResult, "tbResult");
            this.tbResult.Name = "tbResult";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Name = "label2";
            // 
            // ibMain
            // 
            resources.ApplyResources(this.ibMain, "ibMain");
            this.ibMain.Name = "ibMain";
            this.ibMain.TabStop = false;
            this.ibMain.Paint += new System.Windows.Forms.PaintEventHandler(this.ibMain_Paint);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btNewTemplates,
            this.btOpenTemplates,
            this.btSaveTemplates,
            this.toolStripSeparator,
            this.btCreateTemplate,
            this.btAutoGenerate,
            this.btTemplateEditor});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // btNewTemplates
            // 
            this.btNewTemplates.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btNewTemplates, "btNewTemplates");
            this.btNewTemplates.Name = "btNewTemplates";
            this.btNewTemplates.Click += new System.EventHandler(this.btNewTemplates_Click);
            // 
            // btOpenTemplates
            // 
            this.btOpenTemplates.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btOpenTemplates, "btOpenTemplates");
            this.btOpenTemplates.Name = "btOpenTemplates";
            this.btOpenTemplates.Click += new System.EventHandler(this.btOpenTemplates_Click);
            // 
            // btSaveTemplates
            // 
            this.btSaveTemplates.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btSaveTemplates, "btSaveTemplates");
            this.btSaveTemplates.Name = "btSaveTemplates";
            this.btSaveTemplates.Click += new System.EventHandler(this.btSaveTemplates_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            resources.ApplyResources(this.toolStripSeparator, "toolStripSeparator");
            // 
            // btCreateTemplate
            // 
            this.btCreateTemplate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btCreateTemplate, "btCreateTemplate");
            this.btCreateTemplate.Name = "btCreateTemplate";
            this.btCreateTemplate.Click += new System.EventHandler(this.btCreateTemplate_Click);
            // 
            // btAutoGenerate
            // 
            this.btAutoGenerate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btAutoGenerate, "btAutoGenerate");
            this.btAutoGenerate.Name = "btAutoGenerate";
            this.btAutoGenerate.Click += new System.EventHandler(this.btAutoGenerate_Click);
            // 
            // btTemplateEditor
            // 
            this.btTemplateEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btTemplateEditor, "btTemplateEditor");
            this.btTemplateEditor.Name = "btTemplateEditor";
            this.btTemplateEditor.Click += new System.EventHandler(this.btTemplateEditor_Click);
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.button7);
            this.flowLayoutPanel2.Controls.Add(this.button1);
            this.flowLayoutPanel2.Controls.Add(this.button2);
            this.flowLayoutPanel2.Controls.Add(this.button8);
            this.flowLayoutPanel2.Controls.Add(this.button9);
            this.flowLayoutPanel2.Controls.Add(this.button10);
            this.flowLayoutPanel2.Controls.Add(this.button11);
            this.flowLayoutPanel2.Controls.Add(this.button12);
            this.flowLayoutPanel2.Controls.Add(this.button13);
            this.flowLayoutPanel2.Controls.Add(this.button14);
            this.flowLayoutPanel2.Controls.Add(this.button15);
            this.flowLayoutPanel2.Controls.Add(this.button16);
            this.flowLayoutPanel2.Controls.Add(this.button17);
            this.flowLayoutPanel2.Controls.Add(this.button18);
            this.flowLayoutPanel2.Controls.Add(this.textBox41);
            this.flowLayoutPanel2.Controls.Add(this.textBox42);
            this.flowLayoutPanel2.Controls.Add(this.textBox46);
            this.flowLayoutPanel2.Controls.Add(this.textBox48);
            this.flowLayoutPanel2.Controls.Add(this.textBox49);
            this.flowLayoutPanel2.Controls.Add(this.textBox50);
            this.flowLayoutPanel2.Controls.Add(this.textBox51);
            this.flowLayoutPanel2.Controls.Add(this.textBox53);
            this.flowLayoutPanel2.Controls.Add(this.textBox54);
            this.flowLayoutPanel2.Controls.Add(this.textBox79);
            this.flowLayoutPanel2.Controls.Add(this.textBox80);
            this.flowLayoutPanel2.Controls.Add(this.textBox81);
            this.flowLayoutPanel2.Controls.Add(this.textBox82);
            this.flowLayoutPanel2.Controls.Add(this.textBox83);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // button7
            // 
            resources.ApplyResources(this.button7, "button7");
            this.button7.Name = "button7";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button8
            // 
            resources.ApplyResources(this.button8, "button8");
            this.button8.Name = "button8";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            resources.ApplyResources(this.button9, "button9");
            this.button9.Name = "button9";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            resources.ApplyResources(this.button10, "button10");
            this.button10.Name = "button10";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            resources.ApplyResources(this.button11, "button11");
            this.button11.Name = "button11";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            resources.ApplyResources(this.button12, "button12");
            this.button12.Name = "button12";
            this.button12.UseVisualStyleBackColor = true;
            // 
            // button13
            // 
            resources.ApplyResources(this.button13, "button13");
            this.button13.Name = "button13";
            this.button13.UseVisualStyleBackColor = true;
            // 
            // button14
            // 
            resources.ApplyResources(this.button14, "button14");
            this.button14.Name = "button14";
            this.button14.UseVisualStyleBackColor = true;
            // 
            // button15
            // 
            resources.ApplyResources(this.button15, "button15");
            this.button15.Name = "button15";
            this.button15.UseVisualStyleBackColor = true;
            // 
            // button16
            // 
            resources.ApplyResources(this.button16, "button16");
            this.button16.Name = "button16";
            this.button16.UseVisualStyleBackColor = true;
            // 
            // button17
            // 
            resources.ApplyResources(this.button17, "button17");
            this.button17.Name = "button17";
            this.button17.UseVisualStyleBackColor = true;
            // 
            // button18
            // 
            resources.ApplyResources(this.button18, "button18");
            this.button18.Name = "button18";
            this.button18.UseVisualStyleBackColor = true;
            // 
            // textBox41
            // 
            this.textBox41.BackColor = System.Drawing.Color.White;
            this.textBox41.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox41, "textBox41");
            this.textBox41.ForeColor = System.Drawing.Color.Black;
            this.textBox41.Name = "textBox41";
            this.textBox41.ReadOnly = true;
            // 
            // textBox42
            // 
            this.textBox42.BackColor = System.Drawing.Color.White;
            this.textBox42.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox42, "textBox42");
            this.textBox42.ForeColor = System.Drawing.Color.Black;
            this.textBox42.Name = "textBox42";
            this.textBox42.ReadOnly = true;
            // 
            // textBox46
            // 
            this.textBox46.BackColor = System.Drawing.Color.White;
            this.textBox46.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox46, "textBox46");
            this.textBox46.ForeColor = System.Drawing.Color.Black;
            this.textBox46.Name = "textBox46";
            this.textBox46.ReadOnly = true;
            // 
            // textBox48
            // 
            this.textBox48.BackColor = System.Drawing.Color.White;
            this.textBox48.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox48, "textBox48");
            this.textBox48.ForeColor = System.Drawing.Color.Black;
            this.textBox48.Name = "textBox48";
            this.textBox48.ReadOnly = true;
            // 
            // textBox49
            // 
            this.textBox49.BackColor = System.Drawing.Color.White;
            this.textBox49.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox49, "textBox49");
            this.textBox49.ForeColor = System.Drawing.Color.Black;
            this.textBox49.Name = "textBox49";
            this.textBox49.ReadOnly = true;
            // 
            // textBox50
            // 
            this.textBox50.BackColor = System.Drawing.Color.White;
            this.textBox50.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox50, "textBox50");
            this.textBox50.ForeColor = System.Drawing.Color.Black;
            this.textBox50.Name = "textBox50";
            this.textBox50.ReadOnly = true;
            // 
            // textBox51
            // 
            this.textBox51.BackColor = System.Drawing.Color.White;
            this.textBox51.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox51, "textBox51");
            this.textBox51.ForeColor = System.Drawing.Color.Black;
            this.textBox51.Name = "textBox51";
            this.textBox51.ReadOnly = true;
            // 
            // textBox53
            // 
            this.textBox53.BackColor = System.Drawing.Color.White;
            this.textBox53.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox53, "textBox53");
            this.textBox53.ForeColor = System.Drawing.Color.Black;
            this.textBox53.Name = "textBox53";
            this.textBox53.ReadOnly = true;
            // 
            // textBox54
            // 
            this.textBox54.BackColor = System.Drawing.Color.White;
            this.textBox54.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox54, "textBox54");
            this.textBox54.ForeColor = System.Drawing.Color.Black;
            this.textBox54.Name = "textBox54";
            this.textBox54.ReadOnly = true;
            // 
            // textBox79
            // 
            this.textBox79.BackColor = System.Drawing.Color.White;
            this.textBox79.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox79, "textBox79");
            this.textBox79.ForeColor = System.Drawing.Color.Black;
            this.textBox79.Name = "textBox79";
            this.textBox79.ReadOnly = true;
            // 
            // textBox80
            // 
            this.textBox80.BackColor = System.Drawing.Color.White;
            this.textBox80.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox80, "textBox80");
            this.textBox80.ForeColor = System.Drawing.Color.Black;
            this.textBox80.Name = "textBox80";
            this.textBox80.ReadOnly = true;
            // 
            // textBox81
            // 
            this.textBox81.BackColor = System.Drawing.Color.White;
            this.textBox81.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox81, "textBox81");
            this.textBox81.ForeColor = System.Drawing.Color.Black;
            this.textBox81.Name = "textBox81";
            this.textBox81.ReadOnly = true;
            // 
            // textBox82
            // 
            this.textBox82.BackColor = System.Drawing.Color.White;
            this.textBox82.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox82, "textBox82");
            this.textBox82.ForeColor = System.Drawing.Color.Black;
            this.textBox82.Name = "textBox82";
            this.textBox82.ReadOnly = true;
            // 
            // textBox83
            // 
            this.textBox83.BackColor = System.Drawing.Color.White;
            this.textBox83.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox83, "textBox83");
            this.textBox83.ForeColor = System.Drawing.Color.Black;
            this.textBox83.Name = "textBox83";
            this.textBox83.ReadOnly = true;
            // 
            // pnSettings
            // 
            resources.ApplyResources(this.pnSettings, "pnSettings");
            this.pnSettings.Controls.Add(this.btnSave);
            this.pnSettings.Controls.Add(this.cbShowBinarized);
            this.pnSettings.Controls.Add(this.cbShowContours);
            this.pnSettings.Controls.Add(this.groupBox7);
            this.pnSettings.Controls.Add(this.groupBox8);
            this.pnSettings.Controls.Add(this.groupBox6);
            this.pnSettings.Controls.Add(this.cbShowAngle);
            this.pnSettings.Name = "pnSettings";
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbShowBinarized
            // 
            resources.ApplyResources(this.cbShowBinarized, "cbShowBinarized");
            this.cbShowBinarized.Name = "cbShowBinarized";
            this.cbShowBinarized.UseVisualStyleBackColor = true;
            // 
            // cbShowContours
            // 
            resources.ApplyResources(this.cbShowContours, "cbShowContours");
            this.cbShowContours.Name = "cbShowContours";
            this.cbShowContours.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.nudMaxACFdesc);
            this.groupBox7.Controls.Add(this.nudMinACF);
            this.groupBox7.Controls.Add(this.label34);
            this.groupBox7.Controls.Add(this.cbAllowAngleMore45);
            this.groupBox7.Controls.Add(this.label35);
            this.groupBox7.Controls.Add(this.nudMinICF);
            this.groupBox7.Controls.Add(this.label38);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // nudMaxACFdesc
            // 
            resources.ApplyResources(this.nudMaxACFdesc, "nudMaxACFdesc");
            this.nudMaxACFdesc.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudMaxACFdesc.Name = "nudMaxACFdesc";
            this.nudMaxACFdesc.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // nudMinACF
            // 
            this.nudMinACF.DecimalPlaces = 2;
            this.nudMinACF.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            resources.ApplyResources(this.nudMinACF, "nudMinACF");
            this.nudMinACF.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMinACF.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.nudMinACF.Name = "nudMinACF";
            this.nudMinACF.Value = new decimal(new int[] {
            96,
            0,
            0,
            131072});
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // cbAllowAngleMore45
            // 
            resources.ApplyResources(this.cbAllowAngleMore45, "cbAllowAngleMore45");
            this.cbAllowAngleMore45.Name = "cbAllowAngleMore45";
            this.cbAllowAngleMore45.UseVisualStyleBackColor = true;
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // nudMinICF
            // 
            this.nudMinICF.DecimalPlaces = 2;
            this.nudMinICF.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            resources.ApplyResources(this.nudMinICF, "nudMinICF");
            this.nudMinICF.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMinICF.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.nudMinICF.Name = "nudMinICF";
            this.nudMinICF.Value = new decimal(new int[] {
            85,
            0,
            0,
            131072});
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.Name = "label38";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.nudMinDefinition);
            this.groupBox8.Controls.Add(this.cbNoiseFilter);
            this.groupBox8.Controls.Add(this.label39);
            this.groupBox8.Controls.Add(this.nudMinContourLength);
            this.groupBox8.Controls.Add(this.nudMinContourArea);
            this.groupBox8.Controls.Add(this.label46);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // nudMinDefinition
            // 
            resources.ApplyResources(this.nudMinDefinition, "nudMinDefinition");
            this.nudMinDefinition.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudMinDefinition.Name = "nudMinDefinition";
            this.nudMinDefinition.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // cbNoiseFilter
            // 
            resources.ApplyResources(this.cbNoiseFilter, "cbNoiseFilter");
            this.cbNoiseFilter.Name = "cbNoiseFilter";
            this.cbNoiseFilter.UseVisualStyleBackColor = true;
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            // 
            // nudMinContourLength
            // 
            resources.ApplyResources(this.nudMinContourLength, "nudMinContourLength");
            this.nudMinContourLength.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.nudMinContourLength.Name = "nudMinContourLength";
            this.nudMinContourLength.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // nudMinContourArea
            // 
            resources.ApplyResources(this.nudMinContourArea, "nudMinContourArea");
            this.nudMinContourArea.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.nudMinContourArea.Name = "nudMinContourArea";
            this.nudMinContourArea.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label46
            // 
            resources.ApplyResources(this.label46, "label46");
            this.label46.Name = "label46";
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.btLoadFolder);
            this.groupBox6.Controls.Add(this.btnPhoto);
            this.groupBox6.Controls.Add(this.cbCycleCapture);
            this.groupBox6.Controls.Add(this.comboBox1);
            this.groupBox6.Controls.Add(this.cbAdaptiveNoiseFilter);
            this.groupBox6.Controls.Add(this.nudAdaptiveThBlockSize);
            this.groupBox6.Controls.Add(this.label26);
            this.groupBox6.Controls.Add(this.cbBlur);
            this.groupBox6.Controls.Add(this.label30);
            this.groupBox6.Controls.Add(this.cbCaptureFromCam);
            this.groupBox6.Controls.Add(this.cbCamResolution);
            this.groupBox6.Controls.Add(this.btLoadImage);
            this.groupBox6.Controls.Add(this.cbAutoContrast);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // btLoadFolder
            // 
            resources.ApplyResources(this.btLoadFolder, "btLoadFolder");
            this.btLoadFolder.Name = "btLoadFolder";
            this.btLoadFolder.UseVisualStyleBackColor = true;
            this.btLoadFolder.Click += new System.EventHandler(this.btLoadFolder_Click);
            // 
            // btnPhoto
            // 
            resources.ApplyResources(this.btnPhoto, "btnPhoto");
            this.btnPhoto.Name = "btnPhoto";
            this.btnPhoto.UseVisualStyleBackColor = true;
            this.btnPhoto.Click += new System.EventHandler(this.btnPhoto_Click);
            // 
            // cbCycleCapture
            // 
            resources.ApplyResources(this.cbCycleCapture, "cbCycleCapture");
            this.cbCycleCapture.Checked = true;
            this.cbCycleCapture.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCycleCapture.Name = "cbCycleCapture";
            this.cbCycleCapture.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Name = "comboBox1";
            // 
            // cbAdaptiveNoiseFilter
            // 
            resources.ApplyResources(this.cbAdaptiveNoiseFilter, "cbAdaptiveNoiseFilter");
            this.cbAdaptiveNoiseFilter.Checked = true;
            this.cbAdaptiveNoiseFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAdaptiveNoiseFilter.Name = "cbAdaptiveNoiseFilter";
            this.cbAdaptiveNoiseFilter.UseVisualStyleBackColor = true;
            // 
            // nudAdaptiveThBlockSize
            // 
            resources.ApplyResources(this.nudAdaptiveThBlockSize, "nudAdaptiveThBlockSize");
            this.nudAdaptiveThBlockSize.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.nudAdaptiveThBlockSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAdaptiveThBlockSize.Name = "nudAdaptiveThBlockSize";
            this.nudAdaptiveThBlockSize.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // cbBlur
            // 
            resources.ApplyResources(this.cbBlur, "cbBlur");
            this.cbBlur.Checked = true;
            this.cbBlur.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBlur.Name = "cbBlur";
            this.cbBlur.UseVisualStyleBackColor = true;
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // cbCaptureFromCam
            // 
            resources.ApplyResources(this.cbCaptureFromCam, "cbCaptureFromCam");
            this.cbCaptureFromCam.Checked = true;
            this.cbCaptureFromCam.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCaptureFromCam.Name = "cbCaptureFromCam";
            this.cbCaptureFromCam.UseVisualStyleBackColor = true;
            // 
            // cbCamResolution
            // 
            this.cbCamResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCamResolution.FormattingEnabled = true;
            this.cbCamResolution.Items.AddRange(new object[] {
            resources.GetString("cbCamResolution.Items"),
            resources.GetString("cbCamResolution.Items1"),
            resources.GetString("cbCamResolution.Items2")});
            resources.ApplyResources(this.cbCamResolution, "cbCamResolution");
            this.cbCamResolution.Name = "cbCamResolution";
            // 
            // btLoadImage
            // 
            resources.ApplyResources(this.btLoadImage, "btLoadImage");
            this.btLoadImage.Name = "btLoadImage";
            this.btLoadImage.UseVisualStyleBackColor = true;
            this.btLoadImage.Click += new System.EventHandler(this.btLoadImage_Click);
            // 
            // cbAutoContrast
            // 
            resources.ApplyResources(this.cbAutoContrast, "cbAutoContrast");
            this.cbAutoContrast.Name = "cbAutoContrast";
            this.cbAutoContrast.UseVisualStyleBackColor = true;
            // 
            // cbShowAngle
            // 
            resources.ApplyResources(this.cbShowAngle, "cbShowAngle");
            this.cbShowAngle.Name = "cbShowAngle";
            this.cbShowAngle.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage5);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // Form_Listener
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label5);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.Name = "Form_Listener";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibMain)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.pnSettings.ResumeLayout(false);
            this.pnSettings.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxACFdesc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinACF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinICF)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinDefinition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinContourLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinContourArea)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdaptiveThBlockSize)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmUpdateState;
        private System.Windows.Forms.Timer timer2;
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox45;
        private System.Windows.Forms.TextBox textBox44;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TrackBar trackBar2;
        private TabPage tabPage5;
        private StatusStrip ssMain;
        private ToolStripStatusLabel lbFPS;
        private ToolStripStatusLabel lbContoursCount;
        private ToolStripStatusLabel lbRecognized;
        private Button button3;
        private ListBox tbResult;
        private Label label3;
        private Label label2;
        private Emgu.CV.UI.ImageBox ibMain;
        private ToolStrip toolStrip1;
        private ToolStripButton btNewTemplates;
        private ToolStripButton btOpenTemplates;
        private ToolStripButton btSaveTemplates;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripButton btCreateTemplate;
        private ToolStripButton btAutoGenerate;
        private ToolStripButton btTemplateEditor;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button button7;
        private Button button1;
        private Button button2;
        private Button button8;
        private Button button9;
        private Button button10;
        private Button button11;
        private Button button12;
        private Button button13;
        private Button button14;
        private Button button15;
        private Button button16;
        private Button button17;
        private Button button18;
        private TextBox textBox41;
        private TextBox textBox42;
        private TextBox textBox46;
        private TextBox textBox48;
        private TextBox textBox49;
        private TextBox textBox50;
        private TextBox textBox51;
        private TextBox textBox53;
        private TextBox textBox54;
        private TextBox textBox79;
        private TextBox textBox80;
        private TextBox textBox81;
        private TextBox textBox82;
        private TextBox textBox83;
        private Panel pnSettings;
        private Button btnSave;
        private CheckBox cbShowBinarized;
        private CheckBox cbShowContours;
        private GroupBox groupBox7;
        private NumericUpDown nudMaxACFdesc;
        private NumericUpDown nudMinACF;
        private Label label34;
        private CheckBox cbAllowAngleMore45;
        private Label label35;
        private NumericUpDown nudMinICF;
        private Label label38;
        private GroupBox groupBox8;
        private NumericUpDown nudMinDefinition;
        private CheckBox cbNoiseFilter;
        private Label label39;
        private NumericUpDown nudMinContourLength;
        private NumericUpDown nudMinContourArea;
        private Label label46;
        private GroupBox groupBox6;
        private Button btLoadFolder;
        private Button btnPhoto;
        private CheckBox cbCycleCapture;
        private ComboBox comboBox1;
        private CheckBox cbAdaptiveNoiseFilter;
        private NumericUpDown nudAdaptiveThBlockSize;
        private Label label26;
        private CheckBox cbBlur;
        private Label label30;
        private CheckBox cbCaptureFromCam;
        private ComboBox cbCamResolution;
        private Button btLoadImage;
        private CheckBox cbAutoContrast;
        private CheckBox cbShowAngle;
        private TabControl tabControl1;
    }
}

