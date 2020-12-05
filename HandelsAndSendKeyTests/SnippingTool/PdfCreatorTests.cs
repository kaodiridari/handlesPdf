using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApplication2.SnippingTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //"where the pdf should be";
            const string filename = 
                @"C:\Users\user\Documents\result.pdf";
            int numberOfPages = 627;
            //"where the screenshots are";
            string template = 
                @"C:\Users\user\Documents\screenshots\screenshot{0}.png";

            PdfCreator.hallo(155, 235, filename, template, numberOfPages, false);
        }
    }
}