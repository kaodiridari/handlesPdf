// Original from: http://stackoverflow.com/a/3124252/122195
// Modified version multiple monitors aware and text scaling aware
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using WindowsFormsApplication2.SnippingTool;

namespace Snipping_OCR
{
    public class RectangleEventArgs : EventArgs
    {
        public Rectangle rectScaled { get; }
        public Rectangle rectUnscaled { get; }
        public int screenNumber { get; }

        public RectangleEventArgs(Rectangle rectScaled, Rectangle rectUnscaled, int screenNumber)
        {
            this.rectScaled = rectScaled;
            this.rectUnscaled = rectUnscaled;
            this.screenNumber = screenNumber;
        }
    }

    public class ChooseAreaToolFac
    {
        public static ChooseAreaTool get(Image desktopScreenShot, int x, int y, int width, int height, int screenNumber, bool searchAuto)
        {
            if (searchAuto)
            {               
                return new ChooseAreaToolAutomatic(desktopScreenShot, x, y, width, height, screenNumber);
            } else
            {
                return new ChooseAreaTool(desktopScreenShot, x, y, width, height, screenNumber);
            }
        }
    }

    public partial class ChooseAreaTool : Form
    {
        public delegate void EventHandler(object Sender, EventArgs e);       
        public event EventHandler AreaSelected;       
       
        protected Rectangle _rectSelection;
        private Point _pointStart;
        private int myScreenNumber;
        protected Image _deskTopScreenShot;
        
        public ChooseAreaTool(Image desktopScreenShot, int x, int y, int width, int height, int screenNumber)
        {
            Console.WriteLine("ChooseAreaTool x=" + x + "y=" + y + "width=" + width + "height=" + height);
            InitializeComponent();            
            myScreenNumber = screenNumber;
            BackgroundImage = desktopScreenShot;
            _deskTopScreenShot = desktopScreenShot;
            BackgroundImageLayout = ImageLayout.Stretch;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            SetBounds(x, y, width, height);
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            Cursor = Cursors.Cross;
            TopMost = true;
        }
               
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            _pointStart = e.Location;
            _rectSelection = new Rectangle(e.Location, new Size(0, 0));
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            int x1 = Math.Min(e.X, _pointStart.X);
            int y1 = Math.Min(e.Y, _pointStart.Y);
            int x2 = Math.Max(e.X, _pointStart.X);
            int y2 = Math.Max(e.Y, _pointStart.Y);
            _rectSelection = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Console.WriteLine("ChooseAreaTool.OnMouseUp 1");
            
            Invalidate();
            Done();
        }

        

        protected void Done()
        {
            Rectangle rectScaled = ScaleRectSelection();
            Rectangle rectUnScaled = new Rectangle(_rectSelection.X, _rectSelection.Y, _rectSelection.Width, _rectSelection.Height);            

            //NULL-Bedingungsoperator https://msdn.microsoft.com/de-de/library/dn986595.aspx
            AreaSelected?.Invoke(this, new RectangleEventArgs(rectScaled, rectUnScaled, myScreenNumber));
        }

        protected Rectangle ScaleRectSelection()
        {
            double hScale = BackgroundImage.Width / (double)Width;
            double vScale = BackgroundImage.Height / (double)Height;
            int xScaled = (int)(_rectSelection.X * hScale);
            int yScaled = (int)(_rectSelection.Y * vScale);
            int widthScaled = (int)(_rectSelection.Width * hScale);
            int heightScaled = (int)(_rectSelection.Height * vScale);
            Rectangle rectScaled = new Rectangle(xScaled, yScaled, widthScaled, heightScaled);

            return rectScaled;
        }

        protected void UnScaleRectSelection(Rectangle r)
        {
            _rectSelection = r;
            double hScale = BackgroundImage.Width / (double)Width;
            double vScale = BackgroundImage.Height / (double)Height;
            int xScaled = (int)(_rectSelection.X / hScale);
            int yScaled = (int)(_rectSelection.Y / vScale);
            int widthScaled = (int)(_rectSelection.Width / hScale);
            int heightScaled = (int)(_rectSelection.Height / vScale);
            _rectSelection = new Rectangle(xScaled, yScaled, widthScaled, heightScaled);            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Console.WriteLine("OnPaint()");
           
            using (Brush br = new SolidBrush(Color.FromArgb(120, Color.White)))
            {
                int x1 = _rectSelection.X;
                int x2 = _rectSelection.X + _rectSelection.Width;
                int y1 = _rectSelection.Y;
                int y2 = _rectSelection.Y + _rectSelection.Height;
                e.Graphics.FillRectangle(br, new Rectangle(0, 0, x1, Height));
                e.Graphics.FillRectangle(br, new Rectangle(x2, 0, Width - x2, Height));
                e.Graphics.FillRectangle(br, new Rectangle(x1, 0, x2 - x1, y1));
                e.Graphics.FillRectangle(br, new Rectangle(x1, y2, x2 - x1, Height - y2));
                Console.WriteLine("x1:" + x1 + " height:" + Height + " x2:" + x2 + " Width " + Width + " y1:" + y1 + " y2:" + y2);
            } 
            using (Pen pen = new Pen(Color.Pink, 2))
            {
                e.Graphics.DrawRectangle(pen, _rectSelection);
                Console.WriteLine("Rectangled.");
                //Thread.Sleep(3000);
            }
        }

        public virtual void start()
        {

        }

        protected virtual void HandlesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("HandlesForm_FormClosing");
        }
    }

    ///// <summary>
    ///// Dient zum korrigieren des automatisch gefundenen Fensterbereichs.
    ///// 
    ///// </summary>
    //public class ChooseAreaToolAdjustRectangle : Form//ChooseAreaTool
    //{
    //    //public ChooseAreaToolAdjustRectangle(Image desktopScreenShot, int x, int y, int width, int height, int screenNumber) : base (desktopScreenShot, x, y, width, height, screenNumber)
    //    //{
    //    //}

    //    Boolean bHaveMouse;
    //    Point ptOriginal = new Point();
    //    Point ptLast = new Point();

    //    // Called when the left mouse button is pressed. 
    //    public void MyMouseDown(Object sender, MouseEventArgs e)
    //    {
    //        // Make a note that we "have the mouse".
    //        bHaveMouse = true;
    //        // Store the "starting point" for this rubber-band rectangle.
    //        ptOriginal.X = e.X;
    //        ptOriginal.Y = e.Y;
    //        // Special value lets us know that no previous
    //        // rectangle needs to be erased.
    //        ptLast.X = -1;
    //        ptLast.Y = -1;
    //    }
    //    // Convert and normalize the points and draw the reversible frame.
    //    private void MyDrawReversibleRectangle(Point p1, Point p2)
    //    {
    //        Rectangle rc = new Rectangle();

    //        // Convert the points to screen coordinates.
    //        p1 = PointToScreen(p1);
    //        p2 = PointToScreen(p2);
    //        // Normalize the rectangle.
    //        if (p1.X < p2.X)
    //        {
    //            rc.X = p1.X;
    //            rc.Width = p2.X - p1.X;
    //        }
    //        else
    //        {
    //            rc.X = p2.X;
    //            rc.Width = p1.X - p2.X;
    //        }
    //        if (p1.Y < p2.Y)
    //        {
    //            rc.Y = p1.Y;
    //            rc.Height = p2.Y - p1.Y;
    //        }
    //        else
    //        {
    //            rc.Y = p2.Y;
    //            rc.Height = p1.Y - p2.Y;
    //        }
    //        // Draw the reversible frame.
    //        Console.WriteLine("Rectangle " + rc.ToString());
    //        ControlPaint.DrawReversibleFrame(rc, Color.Red, FrameStyle.Dashed);  //geht nicht gescheit
    //        //ControlPaint.DrawGrabHandle(e.);
    //    }

    //    // Called when the left mouse button is released.
    //    public void MyMouseUp(Object sender, MouseEventArgs e)
    //    {
    //        // Set internal flag to know we no longer "have the mouse".
    //        bHaveMouse = false;
    //        // If we have drawn previously, draw again in that spot
    //        // to remove the lines.
    //        if (ptLast.X != -1)
    //        {
    //            Point ptCurrent = new Point(e.X, e.Y);
                
    //            MyDrawReversibleRectangle(ptOriginal, ptLast);
    //        }
    //        // Set flags to know that there is no "previous" line to reverse.
    //        ptLast.X = -1;
    //        ptLast.Y = -1;
    //        ptOriginal.X = -1;
    //        ptOriginal.Y = -1;
    //    }

    //    protected override void OnPaint(PaintEventArgs e)
    //    {
    //        base.OnPaint(e);
    //        var g = e.Graphics;
    //    }
    //    // Called when the mouse is moved.
    //    public void MyMouseMove(Object sender, MouseEventArgs e)
    //    {
    //        Point ptCurrent = new Point(e.X, e.Y);
    //        // If we "have the mouse", then we draw our lines.
    //        if (bHaveMouse)
    //        {
    //            // If we have drawn previously, draw again in
    //            // that spot to remove the lines.
    //            if (ptLast.X != -1)
    //            {
    //                MyDrawReversibleRectangle(ptOriginal, ptLast);
    //            }
    //            // Update last point.
    //            ptLast = ptCurrent;
    //            // Draw new lines.
    //            MyDrawReversibleRectangle(ptOriginal, ptCurrent);
    //        }
    //    }
    //    // Set up delegates for mouse events.
    //    protected override void OnLoad(System.EventArgs e)
    //    {
    //        MouseDown += new MouseEventHandler(MyMouseDown);
    //        MouseUp += new MouseEventHandler(MyMouseUp);
    //        MouseMove += new MouseEventHandler(MyMouseMove);
    //        bHaveMouse = false;
    //    }
    //}   

 

    public class ChooseAreaToolAutomatic : ChooseAreaTool, RepaintAble
    {
        private bool _showGrabRectangle;
        private GrabRectangle _grabRectangle;

        public ChooseAreaToolAutomatic(Image desktopScreenShot, int x, int y, int width, int height, int screenNumber) : base (desktopScreenShot, x, y, width, height, screenNumber)
        {
            
        }

        private GrabRectangle getGrabRectangle(Rectangle r)
        {
            if (_grabRectangle == null)
            {
                _grabRectangle = new GrabRectangle(this.Cursor, r, this.Size, this);
            }
            _grabRectangle.setRectangle(r);
            return _grabRectangle;
        }

        protected override void HandlesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.HandlesForm_FormClosing(sender, e);
            Console.WriteLine("ChooseAreaToolAutomatic closing");
            Environment.Exit(0);
            Process.GetCurrentProcess().Close();
        }

        public override void start()
        {
            Console.WriteLine("start()");
            base.start();
            List<Bitmap> im = new List<Bitmap>();
            im.Add((Bitmap)BackgroundImage);
            var rects = PageSizeFinder.findPageInFullScreenImages(im);

            //skallierung            
            UnScaleRectSelection(rects[0]);
            Refresh(); // -> OnPaint
            DialogResult result = MessageBox.Show("Auswahl ok?", "Title", MessageBoxButtons.YesNo, MessageBoxIcon.Question,  MessageBoxDefaultButton.Button1);
            if (result.Equals(DialogResult.No)) {
                _showGrabRectangle = true;
                getGrabRectangle(_rectSelection);
                BackgroundImage = _deskTopScreenShot;
                Refresh();
                DialogResult result2 = MessageBox.Show("Rahmen ok?", "Title", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                //this.Close();
                //throw new NotImplementedException();   //blockiert dann alles

            } else
            {
                _showGrabRectangle = false;
                Refresh();
                Done();
            }
            
        }

        //Maus ziehen abschalten
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_showGrabRectangle)
            {
                //ziehen vom Rahmen, wenn auf Quadrat sonst Rahmen verschieben.
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_showGrabRectangle)
            {
                //schauen ob Maus über quadrat. -> anderer Cursor
                //über Bild -> anderer Cursor
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Console.WriteLine("ChooseAreaTool.OnMouseUp 2");
            if (_showGrabRectangle)
            {
                //aus is
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Console.WriteLine("OnPaint()");

            int x1 = _rectSelection.X;
            int x2 = _rectSelection.X + _rectSelection.Width;
            int y1 = _rectSelection.Y;
            int y2 = _rectSelection.Y + _rectSelection.Height;
            Console.WriteLine("x1:" + x1 + " height:" + Height + " x2:" + x2 + " Width " + Width + " y1:" + y1 + " y2:" + y2);

            using (Brush br = new SolidBrush(Color.FromArgb(120, Color.White)))
            {
                e.Graphics.FillRectangle(br, new Rectangle(0, 0, x1, Height));
                e.Graphics.FillRectangle(br, new Rectangle(x2, 0, Width - x2, Height));
                e.Graphics.FillRectangle(br, new Rectangle(x1, 0, x2 - x1, y1));
                e.Graphics.FillRectangle(br, new Rectangle(x1, y2, x2 - x1, Height - y2));
            }

            if (_showGrabRectangle)
            {
                Console.WriteLine("showGrabRectangle: " + _showGrabRectangle + " _rectSelection " + _rectSelection);
                getGrabRectangle(_rectSelection).draw(e.Graphics);                
            }
            else
            {
                using (Pen pen = new Pen(Color.Aqua, 5))
                {
                    //ControlPaint.DrawGrabHandle(e.Graphics, _rectSelection, false, false); geht nicht
                    Console.WriteLine("showGrabRectangle: " + _showGrabRectangle);
                    e.Graphics.DrawRectangle(pen, _rectSelection);
                    Console.WriteLine("Rectangled.");
                    //Thread.Sleep(3000);
                }
            }            
        }

        public void needRepaint()
        {
            throw new NotImplementedException();
        }
    }
}