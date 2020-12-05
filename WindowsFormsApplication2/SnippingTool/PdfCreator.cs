using System.Diagnostics;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Drawing;
using System.IO;
using System;

namespace WindowsFormsApplication2.SnippingTool
{
    public class PdfCreator
    {
        private const double mmToPoint = 72 / 25.4;

        static public void hallo(double pageWidthMM, double pageHeightMM, string pdfFileName, string fileNameTemplate, int numberOfPages, bool rotateit)
        {
            double pageWidthPoint = pageWidthMM * mmToPoint;
            double pageHeightPoint = pageHeightMM * mmToPoint;

            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "ebook";

            for (int n = 0; n < numberOfPages; n++)
            {
                try {
                    // Create an empty page
                    PdfPage page = document.AddPage();
                    page.Width = new XUnit(pageWidthPoint, XGraphicsUnit.Point); //das ist intern points            
                    page.Height = new XUnit(pageHeightPoint, XGraphicsUnit.Point);  //das ist intern points            

                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    var bitmap1 = Image.FromFile(fileNameTemplate.Replace("{0}", n.ToString()));
                    if (rotateit)
                    {
                        bitmap1.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    XImage image = XImage.FromGdiPlusImage(bitmap1);
                    gfx.DrawImage(image, 0, 0, pageWidthPoint, pageHeightPoint);
                    bitmap1.Dispose();
                    image.Dispose();
                    gfx.Dispose();
                    System.Diagnostics.Debug.WriteLine("page: " + n);
                    
                } catch (Exception e)
                {
                   var s = e.StackTrace;
                }
            }
            
            document.Save(pdfFileName);  
        }
    }
}
