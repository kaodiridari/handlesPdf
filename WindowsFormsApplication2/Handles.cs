using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Snipping_OCR;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;

namespace SimulateKeyPress
{
    class HandlesForm : Form
    {
        private TextBox textBox1;
        private Button button1;
        private Button moveToTargetWindowButton;
        private Button button2;
        POINT _targetScreenPos;

        Rectangle _rectScaled;
        Rectangle _rectUnscaled;
        private Button doItButton;
        const string filePath = @"C:\Users\user\dreck\";
        private Label label1;
        private TextBox textBox2;
        private Color pagesTextBoxBackColor;
        private ChooseAreaTool[] _formsForScreens;
        private Button findPageButton;
        private IntPtr myHandle;

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new HandlesForm());
        }

        public HandlesForm()
        {
            //ChooseAreaTool.AreaSelected += OnAreaSelected;
            InitializeComponent();
            pagesTextBoxBackColor = textBox2.BackColor;
            doItButton.Enabled = false;
        }

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags gaFlags);

        internal enum GetAncestorFlags
        {
            GetParent = 1,
            GetRoot = 2,
            GetRootOwner = 3
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Drawing.Point p = new Point(_targetScreenPos.X, _targetScreenPos.Y);
            IntPtr windowAtPoint = WindowFromPoint(p);   //kind-fenster
            IntPtr getAncestorWindowGetRoot = GetAncestor(windowAtPoint, GetAncestorFlags.GetRoot);
            IntPtr getAncestorWindowGetRootOwner = GetAncestor(windowAtPoint, GetAncestorFlags.GetRootOwner);
            string str = "getAncestorWindowGetRoot: " + getAncestorWindowGetRoot + "\n";
            str += "getAncestorWindowGetRootOwner: " + getAncestorWindowGetRootOwner + "\n";
            str += "windowAtPoint: " + windowAtPoint + "\n";
            textBox1.Clear();
            textBox1.AppendText(str);

            myHandle = getAncestorWindowGetRoot;
            
            if (myHandle == IntPtr.Zero)
            {
                MessageBox.Show("Target is not running.");
                return;
            }

            ////shift+ctrl + 1 = drehen guz
            //SetForegroundWindow(myHandle);
            //SendKeys.SendWait("+^1");
            ////Thread.Sleep(1000);

            ////ctrl + L = vollbild
            //SetForegroundWindow(myHandle);
            //SendKeys.SendWait("^l");
            ////Thread.Sleep(1000);

            //for (int i = 0; i < 5; i++)
            //{
            //    SetForegroundWindow(myHandle);
            //    SendKeys.SendWait("{PGDN}");
            //    //Thread.Sleep(1000);
            //}

            ////ctrl + L = vollbild
            //SetForegroundWindow(myHandle);
            //SendKeys.SendWait("^l");
            ////Thread.Sleep(1000);

            ////shift+ctrl + 1 = drehen guz
            //SetForegroundWindow(myHandle);
            //SendKeys.SendWait("+^1");
            ////Thread.Sleep(1000);
            //SetForegroundWindow(myHandle);
            //SendKeys.SendWait("+^1");
            ////Thread.Sleep(1000);
            //SetForegroundWindow(myHandle);
            //SendKeys.SendWait("+^1");
            ////Thread.Sleep(1000);
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.moveToTargetWindowButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.doItButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.findPageButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(10, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "Window choosen";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // moveToTargetWindowButton
            // 
            this.moveToTargetWindowButton.AutoSize = true;
            this.moveToTargetWindowButton.Cursor = System.Windows.Forms.Cursors.Cross;
            this.moveToTargetWindowButton.Location = new System.Drawing.Point(10, 50);
            this.moveToTargetWindowButton.Name = "moveToTargetWindowButton";
            this.moveToTargetWindowButton.Size = new System.Drawing.Size(155, 27);
            this.moveToTargetWindowButton.TabIndex = 1;
            this.moveToTargetWindowButton.Text = "Move mouse to target";
            this.moveToTargetWindowButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MyOnMouseUp);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(1, 260);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(259, 168);
            this.textBox1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(10, 92);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(176, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Choose snipping region.";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.snippButton_Click);
            // 
            // doItButton
            // 
            this.doItButton.Location = new System.Drawing.Point(10, 133);
            this.doItButton.Name = "doItButton";
            this.doItButton.Size = new System.Drawing.Size(124, 22);
            this.doItButton.TabIndex = 3;
            this.doItButton.Text = "Do it!";
            this.doItButton.UseVisualStyleBackColor = true;
            this.doItButton.Click += new System.EventHandler(this.doItButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 213);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Seiten:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(80, 211);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(64, 20);
            this.textBox2.TabIndex = 5;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // findPageButton
            // 
            this.findPageButton.Location = new System.Drawing.Point(10, 171);
            this.findPageButton.Name = "findPageButton";
            this.findPageButton.Size = new System.Drawing.Size(215, 34);
            this.findPageButton.TabIndex = 6;
            this.findPageButton.Text = "Auto-Choose snipping region";
            this.findPageButton.UseVisualStyleBackColor = true;
            this.findPageButton.Click += new System.EventHandler(this.findPageButton_Click);
            // 
            // HandlesForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 496);
            this.Controls.Add(this.findPageButton);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.doItButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.moveToTargetWindowButton);
            this.Controls.Add(this.textBox1);
            this.Name = "HandlesForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HandlesForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void MyOnMouseUp(object sender, MouseEventArgs e)
        {
            // Start the snip on mouse down
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            textBox1.AppendText("MyOnMouseUp: " + e.Location + "\n");
            
            GetCursorPos(out _targetScreenPos);
            textBox1.AppendText("MyOnMouseUp: " + _targetScreenPos.X + ", " + _targetScreenPos.Y + "\n");
            
            button1.Enabled = true;
        }

        private void snippButton_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //var test =new ChooseAreaToolAdjustRectangle();
            //test.Show();
            //findPage(false);
        }

        private void findPageButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("findPageButton_Click");
            //shift+ctrl + 1 = drehen guz
            SetForegroundWindow(myHandle);
            SendKeys.SendWait("+^1");
            Thread.Sleep(1000);

            ////ctrl + L = vollbild
            SetForegroundWindow(myHandle);
            SendKeys.SendWait("^l");
            Thread.Sleep(1000);

            findPage(false);
        }

        void findPage(bool autoSearch)
        {
            //snipping form aufmachen, bei MouseUp Rechteck speichern (nicht den screenshot)
            //ChooseAreaTool.Snip();
            //public static void Snip()
            //{
            Console.WriteLine("findPage(bool autoSearch" + autoSearch);
            List<DeviceInfo> screens = ScreenHelper.GetMonitorsInfo();

            //alle meine Monitore
            _formsForScreens = new ChooseAreaTool[screens.Count];

            for (int i = 0; i < screens.Count; i++)
            {
                int hRes = screens[i].HorizontalResolution;
                int vRes = screens[i].VerticalResolution;
                int top = screens[i].MonitorArea.Top;
                int left = screens[i].MonitorArea.Left;
                var bmp = new Bitmap(hRes, vRes, PixelFormat.Format32bppPArgb);
                //ganzer Bildschirm als Hintergrund
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(left, top, 0, 0, bmp.Size);

                }
                _formsForScreens[i] = ChooseAreaToolFac.get(bmp, left, top, hRes, vRes, i, autoSearch);
                _formsForScreens[i].AreaSelected += OnAreaSelected;
                _formsForScreens[i].Show();
                _formsForScreens[i].start();
            }
        }

        private void OnAreaSelected(object sender, EventArgs e)
        {
            Console.WriteLine("OnAreaSelected");
            RectangleEventArgs rea = (RectangleEventArgs)e;
            this._rectScaled = rea.rectScaled;
            this._rectUnscaled = rea.rectUnscaled;
            int sn = rea.screenNumber;
            textBox1.AppendText("Rectangle scaled: " + _rectScaled.ToString() + " Rectangle unscaled: " + _rectUnscaled.ToString() + " on screen: " + sn + "\n");

            //Thread.Sleep(10000);

            //und die forms loswerden
            for (int i = 0; i < _formsForScreens.Length; i++)
            {
                _formsForScreens[i].Dispose();
                //try
                //{
                //    if (_formsForScreens != null)
                //    {
                //        _formsForScreens[i].Dispose();
                //    } else
                //    {
                //        Console.WriteLine("ups " + i + " is null");
                //    }
                //}  catch (Exception)
                //{
                //    //ignore
                //    Console.WriteLine(e.ToString());
                //}
            }
        }

        private void doItButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("doItButton_Click");
            string text = textBox2.Text;
            int numberOfShots;
            if(!int.TryParse(text, out numberOfShots))
            {
                return;
            }
            Hide();
            Thread.Sleep(1000); //weil das fenster langsam ausgeblendet wird
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < numberOfShots; i++)
            {
                if (myHandle == IntPtr.Zero)
                {
                    MessageBox.Show("Target is not running.");
                    return;
                }
                SetForegroundWindow(myHandle);
                
                Bitmap captureBitmap = new Bitmap(_rectScaled.Width, _rectScaled.Height, PixelFormat.Format32bppArgb);
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                captureGraphics.CopyFromScreen(_rectScaled.X, _rectScaled.Y, 0, 0, _rectScaled.Size);
                sb.Append(filePath).Append("csharp").Append(i).Append(".png");               
                captureBitmap.Save(sb.ToString(), ImageFormat.Png);
                sb.Clear();                
                captureGraphics.Dispose();
                captureBitmap.Dispose();
                SendKeys.SendWait("{PGDN}");
                Thread.Sleep(500); //sendwait wartet nicht?
            }          
            Show();
        }

        //private void label1_Click(object sender, EventArgs e)
        //{

        //}

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int val;
            if (textBox2.Text.Trim().StartsWith("0"))
            {
                textBox2.BackColor = Color.Red;
                doItButton.Enabled = false;
                return;
            }
            int.TryParse(textBox2.Text, out val);
            if (val <= 0)
            {
                textBox2.BackColor = Color.Red;
                doItButton.Enabled = false;
            } else
            {
                textBox2.BackColor = pagesTextBoxBackColor;
                doItButton.Enabled = true;
            }
        }

        private void HandlesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("HandlesForm_FormClosing");
        }

        //private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    if (e.CloseReason == CloseReason.UserClosing)
        //    {
        //        DialogResult result = MessageBox.Show("Do you really want to exit?", "Dialog Title", MessageBoxButtons.YesNo);
        //        if (result == DialogResult.Yes)
        //        {
        //            Environment.Exit(0);
        //        }
        //        else
        //        {
        //            e.Cancel = true;
        //        }
        //    }
        //    else
        //    {
        //        e.Cancel = true;
        //    }
        //}
    }
}
