using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApplication2.SnippingTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace WindowsFormsApplication2.SnippingTool.Tests
{
    [TestClass()]
    public class PdfCreatorTests
    {
        /// <summary>
        /// Großes pdf zusammenbauen. Da kommt nach ungefähr einer Minute eine Ausnahme von VS.
       ///  Die Ausschalten dann geht's.
        /// </summary>
        //[TestMethod()]
        //public void HalloTest1()
        //{
        //    const string filename =
        //        @"C:\Users\user\Documents\Visual Studio 2015\Projects\handlesPdf\HandelsAndSendKeyTests\SnippingTool\results\kompakt.pdf";
        //    int numberOfPages = 395;
        //    string template = @"C:\Users\user\Documents\bücher\gravitation und physik kompakter objekte\kompakt{0}.png";

        //    PdfCreator.hallo(155, 235, filename, template, numberOfPages);
        //}

        [TestMethod()]
        public void HalloTest2()
        {
            const string dir = @"C:\Users\user\Documents\screenshoots\temp";

            //A4 210 mm x 297 mm
            const double pageWidthMM = 210.0;
            //const double r = 210.0 / 297.0;

            //"where the pdf should be";
            string filename = Path.Combine(dir, "allinone.pdf");
            
            //only screenshots in folder
            int numberOfPages = Directory.EnumerateFiles(dir).Count(); 
            
            //"where the screenshots are";
            string template = Path.Combine(dir, "csharp{0}.png"); 

            //get the common size: first picture -> size of all
            string firstPageDir = template.Replace("{0}", "1");
            System.Drawing.Image img = System.Drawing.Image.FromFile(firstPageDir);
            int w = img.Width;
            int h = img.Height;
            double rimg = (double)w / (double)h;
            Console.WriteLine("image: " + firstPageDir + "w: " + w + " h: " + h + " r: " + rimg);

            PdfCreator.hallo(pageWidthMM, pageWidthMM / rimg, filename, template, numberOfPages, false);
        }
    }
}