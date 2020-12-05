using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApplication2.SnippingTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsFormsApplication2.SnippingTool.Tests
{
    [TestClass()]
    public class PageSizeFinderTests
    {
        [TestMethod()]
        public void findPageInFullScreenImagesTest()
        {
            //Testbilder holen
            List<Bitmap> testPictures = new List<Bitmap>();            
            testPictures.Add(new Bitmap(Image.FromFile(@"C:\Users\user\Documents\Visual Studio 2015\Projects\handlesPdf\WindowsFormsApplication2\testShoots\a_Page.png")));
            testPictures.Add(new Bitmap(Image.FromFile(@"C:\Users\user\Documents\Visual Studio 2015\Projects\handlesPdf\WindowsFormsApplication2\testShoots\coverFull.png")));
            testPictures.Add(new Bitmap(Image.FromFile(@"C:\Users\user\Documents\Visual Studio 2015\Projects\handlesPdf\WindowsFormsApplication2\testShoots\rectangles on black.png")));

            List<Rectangle> rects = PageSizeFinder.findPageInFullScreenImages(testPictures);

            Rectangle r;
            r = rects[0];
            Console.WriteLine(r);
            Assert.IsTrue(r.X == 141 && r.Y == 0 && r.Width == 1636 && r.Height == 1078);
            r = rects[1];
            Console.WriteLine(r);
            Assert.IsTrue(r.X == 141 && r.Y == 0 && r.Width == 1636 && r.Height == 1078);
            r = rects[2];
            Console.WriteLine(r);
            Assert.IsTrue(r.X == 222 && r.Y == 88 && r.Width == 1518 && r.Height == 647);
        }
    }
}