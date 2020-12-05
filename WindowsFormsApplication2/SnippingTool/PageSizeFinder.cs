using System;
using System.Collections.Generic;
using System.Drawing;

namespace WindowsFormsApplication2.SnippingTool
{
	/**
	* Geht nur für richtige pdf in Adobe-Reader. Vollbildmodus.
	*/
    public class PageSizeFinder
    {
        public static List<Rectangle> findPageInFullScreenImages(List<Bitmap> liOfFullScreenImages)
        {
            //Seite füllt Bildschirm-höhe voll aus. Schwarze Streifen am Rand.
            //->von den Rändern her suchen, bis schwarze Pixel aufhören.
            //Mit ein paar Seiten machen, größtes Rechteck nehmen.
            const byte black = 10;
            List <Rectangle> rectangles = new List<Rectangle>(liOfFullScreenImages.Count);

            foreach (Bitmap im in liOfFullScreenImages)
            {
                int leftUpper = -1;
                int leftUpperY = -1;
                int leftLower = -1;
                int leftLowerY = -1;
                int rightUpper = -1;
                int rightUpperY = -1;
                int rightLower = -1;
                int rightLowerY = -1;
                List<int> borderLeft = new List<int>(im.Height);
                List<int> borderRight = new List<int>(im.Height);
                for (int y = 0; y < im.Height; y++)
                {
                    int x;
                    for (x = 0; x < im.Width; x++)
                    {
                        Color color = im.GetPixel(x, y);
                        if (color.R > black && color.G > black && color.B > black)
                        {
                            borderLeft.Add(x);
                            break;
                        }                        
                    }
                    if (x == im.Width)
                    {
                        borderLeft.Add(-1);
                        borderRight.Add(-1);
                        continue;
                    }

                    for (x = im.Width -1; x >= 0; x--)
                    {
                        Color color = im.GetPixel(x, y);
                        if (color.R > black && color.G > black && color.B > black)
                        {
                            borderRight.Add(x);
                        }
                    }                    
                }

                //links oben
                
                for (int i = 0; i < borderLeft.Count; i++)
                {
                    if (borderLeft[i] > 0)
                    {
                        leftUpper = borderLeft[i];
                        leftUpperY = i;
                        break;
                    }
                }

                //kein links oben kein Rechteck
                if (leftUpper < 0)
                {
                    throw new Exception("There is no Rectangle in the picture! No top left corner found.");
                }

                for (int i = borderLeft.Count - 1; i >= 0; i--)
                {
                    if (borderLeft[i] > 0)
                    {
                        leftLower = borderLeft[i];
                        leftLowerY = i;
                        break;
                    }
                }

                //kein links unten kein Rechteck
                if (leftLower < 0)
                {
                    throw new Exception("There is no Rectangle in the picture! No down left corner found.");
                }

                for (int i = 0; i < borderRight.Count; i++)
                {
                    if (borderRight[i] > 0)
                    {
                        rightUpper = borderRight[i];
                        rightUpperY = i;
                        break;
                    }
                }

                //kein rechts oben kein Rechteck
                if (rightUpper < 0)
                {
                    throw new Exception("There is no Rectangle in the picture! No top right corner found.");
                }

                for (int i = borderRight.Count -1; i >= 0; i--)
                {
                    if (borderRight[i] > 0)
                    {
                        rightLower = borderRight[i];
                        rightLowerY = i;
                        break;
                    }
                }

                //kein rechts unten kein Rechteck
                if (rightLower < 0)
                {
                    throw new Exception("There is no Rectangle in the picture! No lower right corner found.");
                }
                var r = new Rectangle(leftUpper, leftUpperY, Math.Abs(rightUpper - leftUpper), Math.Abs(leftUpperY - leftLowerY));
                rectangles.Add(r);
            }            
            return rectangles;
        }
    }
}
