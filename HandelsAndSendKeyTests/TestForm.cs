using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HandelsAndSendKeyTests
{
    public partial class TestForm : Form
    {
        private bool _showGrabRectangle;
        private GrabRectangle _grabRectangle;

        public TestForm()
        {
            InitializeComponent();
            _grabRectangle = new GrabRectangle(Cursor, new Rectangle(10, 10, 100, 100));
        }        

        private void TestForm_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("TestForm_MouseDown " + e.Location);
        }

        private void TestForm_MouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("TestForm_MouseMove " + e.Location);
        } 

        private void TestForm_MouseUp(object sender, MouseEventArgs e)
        {
            Console.WriteLine("TestForm_MouseUp " + e.Location);
        }

        //Maus ziehen abschalten
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Console.WriteLine("OnMouseDown()");
            if (_showGrabRectangle)
            {
                //ziehen vom Rahmen, wenn auf Quadrat sonst Rahmen verschieben.
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Console.WriteLine("OnMouseMove()");
            if (_showGrabRectangle)
            {
                //schauen ob Maus über quadrat. -> anderer Cursor
                //über Bild -> anderer Cursor
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Console.WriteLine("OnMouseUp()");
            if (_showGrabRectangle)
            {
                //aus is
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Console.WriteLine("OnPaint()");
            if (_showGrabRectangle)
            {
                _grabRectangle.draw(e.Graphics);
            }
        }
    }
}
