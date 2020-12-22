using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Snipping_OCR;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using WindowsFormsApplication2.SnippingTool;

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
        //const string filePath = @"C:\Users\user\temp\";
        private const int SecondsTimeout = 60;
        private const int MillisecondsSleep = 250;
        private Label label1;
        private TextBox numberOfPages;
        private Color pagesTextBoxBackColor;
        private ChooseAreaTool[] _formsForScreens;
        private Button findPageButton;
        private CheckBox checkBoxPdf;
        private CheckBox checkBoxKeep;
        private TextBox pathOfShoots;
        private Label label2;
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
            pagesTextBoxBackColor = numberOfPages.BackColor;
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
            findTargetWindow();
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

        /**
         * Determines the window we want to have screenshots from.
         * _targetScreenPos must contain a point within the window.
         * Finally myHandle is the handle of the target window.
         * This is called after the user has a 'choosing-rectangle' drawn or
         * after the user used the moveToTargetWindowButton.
         */
        private void findTargetWindow()
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
        }


    private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.moveToTargetWindowButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.doItButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numberOfPages = new System.Windows.Forms.TextBox();
            this.findPageButton = new System.Windows.Forms.Button();
            this.checkBoxPdf = new System.Windows.Forms.CheckBox();
            this.checkBoxKeep = new System.Windows.Forms.CheckBox();
            this.pathOfShoots = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
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
            this.moveToTargetWindowButton.Enabled = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 306);
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
            this.label1.Location = new System.Drawing.Point(12, 216);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Seiten:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // numberOfPages
            // 
            this.numberOfPages.Location = new System.Drawing.Point(60, 213);
            this.numberOfPages.Name = "numberOfPages";
            this.numberOfPages.Size = new System.Drawing.Size(64, 20);
            this.numberOfPages.TabIndex = 5;
            this.numberOfPages.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
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
            // checkBoxPdf
            // 
            this.checkBoxPdf.AutoSize = true;
            this.checkBoxPdf.Checked = true;
            this.checkBoxPdf.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPdf.Location = new System.Drawing.Point(17, 283);
            this.checkBoxPdf.Name = "checkBoxPdf";
            this.checkBoxPdf.Size = new System.Drawing.Size(41, 17);
            this.checkBoxPdf.TabIndex = 7;
            this.checkBoxPdf.Text = "pdf";
            this.checkBoxPdf.UseVisualStyleBackColor = true;
            this.checkBoxPdf.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBoxKeep
            // 
            this.checkBoxKeep.AutoSize = true;
            this.checkBoxKeep.Checked = true;
            this.checkBoxKeep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxKeep.Location = new System.Drawing.Point(147, 283);
            this.checkBoxKeep.Name = "checkBoxKeep";
            this.checkBoxKeep.Size = new System.Drawing.Size(78, 17);
            this.checkBoxKeep.TabIndex = 8;
            this.checkBoxKeep.Text = "keep shots";
            this.checkBoxKeep.UseVisualStyleBackColor = true;
            this.checkBoxKeep.CheckedChanged += new System.EventHandler(this.checkBoxKeep_CheckedChanged);
            // 
            // pathOfShoots
            // 
            this.pathOfShoots.Location = new System.Drawing.Point(60, 243);
            this.pathOfShoots.Name = "pathOfShoots";
            this.pathOfShoots.Size = new System.Drawing.Size(321, 20);
            this.pathOfShoots.TabIndex = 9;
            this.pathOfShoots.Text = "C:\\Users\\user\\Documents\\screenshoots\\<enter subpath here>";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 246);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Pfad:";
            // 
            // HandlesForm
            // 
            this.ClientSize = new System.Drawing.Size(393, 486);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pathOfShoots);
            this.Controls.Add(this.checkBoxKeep);
            this.Controls.Add(this.checkBoxPdf);
            this.Controls.Add(this.findPageButton);
            this.Controls.Add(this.numberOfPages);
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
            textBox1.AppendText(" Upper left x: " + rea.rectScaled.X.ToString() + " Upper left y: " + rea.rectScaled.Y.ToString());
            //Thread.Sleep(10000);

            this._targetScreenPos.X = rea.rectUnscaled.X;
            this._targetScreenPos.Y = rea.rectUnscaled.Y;

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

            findTargetWindow();
        }

        private void doItButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("doItButton_Click");

            string tempPath = System.IO.Path.GetTempPath();
            string captureDir = System.IO.Path.Combine(tempPath, "capture"); //captureDir = "C:\\Users\\user\\AppData\\Local\\Temp\\capture"
            System.IO.Directory.CreateDirectory(captureDir);

            // Create a file name for the file you want to create.   
            string text = numberOfPages.Text;
            int numberOfShots;
            if(!int.TryParse(text, out numberOfShots))
            {
                return;
            }
            Hide();
            Thread.Sleep(1000); //weil das fenster langsam ausgeblendet wird
            StringBuilder sb = new StringBuilder();
            long lastFilesSize = -1;
            string lastFilesName = "";
            double now = ((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds) / 1000;
            string posh = pathOfShoots.Text.Trim();
            if (!Directory.Exists(posh))
            {
                MessageBox.Show("Directory does not exist.");
                return;
            }
            for (int i = 0; i < numberOfShots; i++)
            {
                doitagain:
                if (myHandle == IntPtr.Zero)
                {
                    MessageBox.Show("Target is not running.");
                    return;
                }
                SetForegroundWindow(myHandle);
                
                Bitmap captureBitmap = new Bitmap(_rectScaled.Width, _rectScaled.Height, PixelFormat.Format32bppArgb);
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                captureGraphics.CopyFromScreen(_rectScaled.X, _rectScaled.Y, 0, 0, _rectScaled.Size);
                                
                string tempFileName = System.IO.Path.GetRandomFileName(); //tempFileName = "a4it1izv.xq5"
                Console.WriteLine("tempFileName: " + tempFileName);
                string tempSreenshotFile = System.IO.Path.Combine(captureDir, tempFileName);
                Console.WriteLine("tempSreenshot: " + tempSreenshotFile);               
                captureBitmap.Save(tempSreenshotFile, ImageFormat.Png);
                long lengthTempScreenshot = new System.IO.FileInfo(tempSreenshotFile).Length;
                Console.WriteLine("tempSreenshot.length: " + lengthTempScreenshot);
                               
                //get the file-size -> same file size for subsequent screenshots
                //are most certainly a sign for a stuck application.                
                if (lengthTempScreenshot == lastFilesSize)
                {
                    Console.WriteLine("Same filesize: " + lastFilesSize + " for index i = " + i);                    
                    double nowagain = ((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds) / 1000;
                    if (nowagain - now > SecondsTimeout)
                    {
                        //TODO handle the rare possibilty of two screenshoots having the same size.
                        throw new Exception("ups. This is a timeout");
                    }
                    if (AreFileContentsEqual(tempSreenshotFile, lastFilesName))
                    {
                        Thread.Sleep(MillisecondsSleep);
                        goto doitagain;
                    } else
                    {
                        Console.WriteLine("Same filesize but different content - isn't this really quite exceptional? " + lastFilesName + " " + tempSreenshotFile);
                    } 
                }                 
                lastFilesSize = lengthTempScreenshot;
                sb.Clear();
                captureGraphics.Dispose();
                captureBitmap.Dispose();
                string shot = sb.Append("csharp").Append(i).Append(".png").ToString();
                String realScreenshotPath = Path.Combine(posh, shot);                
                if (File.Exists(realScreenshotPath))
                {
                    Console.WriteLine("Deleting: " + realScreenshotPath);
                    File.Delete(realScreenshotPath);
                }
                File.Move(/*from*/tempSreenshotFile, /*to*/realScreenshotPath);
                Console.WriteLine("Moved " + tempSreenshotFile + " to " + realScreenshotPath); 
                lastFilesName = realScreenshotPath;
                now = ((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds) / 1000;
                SendKeys.SendWait("{PGDN}");
                Thread.Sleep(MillisecondsSleep);                
            }  //endfor
            if (checkBoxPdf.Checked)
            {
                Console.WriteLine("Creating a pdf.");
                const double pageWidthMM = 210.0;
                string filenamePdf = Path.Combine(posh, "allinone.pdf");
                string template = Path.Combine(posh, "csharp{0}.png");
                //get the common size: first picture -> size of all
                string firstPageDir = template.Replace("{0}", "1");
                System.Drawing.Image img = System.Drawing.Image.FromFile(firstPageDir);
                int w = img.Width;
                int h = img.Height;
                img.Dispose();
                double rimg = (double)w / (double)h;
                Console.WriteLine("image: " + firstPageDir + " w: " + w + " h: " + h + " r: " + rimg + " numberOfShots: " + numberOfShots);

                PdfCreator.hallo(pageWidthMM, pageWidthMM / rimg, filenamePdf, template, numberOfShots, false);
                if (!checkBoxKeep.Checked)
                {
                    Console.WriteLine("Deleting shots.");
                    IEnumerable<string> files = Directory.EnumerateFiles(posh);
                    foreach (string f in files)
                    {
                        if (f.EndsWith(".png"))
                        {
                            try
                            {
                                File.Delete(f);
                                Console.WriteLine("Deleted: " + f);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Not deleted: " + f);
                            }                            
                        }
                    }
                }
            }             
            Show();
        }

        public static bool AreFileContentsEqual(String path1, String path2) => File.ReadAllBytes(path1).SequenceEqual(File.ReadAllBytes(path2));        

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int val;
            if (numberOfPages.Text.Trim().StartsWith("0"))
            {
                numberOfPages.BackColor = Color.Red;
                doItButton.Enabled = false;
                return;
            }
            int.TryParse(numberOfPages.Text, out val);
            if (val <= 0)
            {
                numberOfPages.BackColor = Color.Red;
                doItButton.Enabled = false;
            } else
            {
                numberOfPages.BackColor = pagesTextBoxBackColor;
                doItButton.Enabled = true;
            }
        }

        private void HandlesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("HandlesForm_FormClosing");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxKeep_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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
