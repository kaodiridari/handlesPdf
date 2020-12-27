using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

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

        //C:\Users\user\Documents\Visual Studio 2015\Projects\handlesPdf\WindowsFormsApplication2\testShoots
        //testpage.png
        //MyOnMouseUp: {X=98,Y=14}
        //MyOnMouseUp: 424, 756 color at point: Color[A = 255, R = 128, G = 128, B = 128]
        public static List<Rectangle> findPageInAlmostFullScreenImages(List<Bitmap> liOfFullScreenImages, Color background, int x, int y)
        {
            //Seite füllt Bildschirmhöhe nicht voll aus. Streifen links, rechts, oben, unten, aber keine Unterbrechung.
            //Es kann an den Seiten Leisten geben.
            //Seiten sind immer zentriert.
            //
            //In der Bildmitte anfangen
            //Punkt hat zufällig Hintergrundfarbe -> neuer Anfang irgendwo daneben usw.
            //Nach oben, unten, links, rechts laufen
            //Übergang zu Hintergrundfarbe ergibt Kanten.
            //Mehrmals machen.
            //Voraussetzung: Gelieferter Punkt liegt im Hintergrund: prüfen.
            //Mit ein paar Seiten machen, größtes Rechteck nehmen.

            List <Rectangle> rectangles = new List<Rectangle>(liOfFullScreenImages.Count);
            bool[] badImages = new bool[liOfFullScreenImages.Count];
            const int SUBSEQUENTHITS_NEEDED = 5;
            int numberOfImage = -1;
            foreach (Bitmap image in liOfFullScreenImages)
            {
                image.Save(@"C:\Users\user\Documents\Visual Studio 2015\Projects\handlesPdf\WindowsFormsApplication2\testShoots\screenshot.png", ImageFormat.Png);
                numberOfImage++;
                //Center
                int cy = image.Height / 2;
                int cx = image.Width / 2;
                int x_north = cx;
                int y_north = cy;
                Console.WriteLine("image.Size: " + image.Size);

                //Search in all four directions for the background. Five subsequent hits of background-color we like.
                //Be aware of the gray.
                int foundNorthY = -1;
                {
                    //North                    
                    int subsequentHitsNorth = 0;

                    bool northIsDone = false;
                    for (int northY = cy; northY >= 0 && subsequentHitsNorth < 5; northY--)
                    {
                        Color p = image.GetPixel(x_north, northY);
                        if (p.Equals(background))
                        {
                            subsequentHitsNorth++;
                            Console.WriteLine("subsequentHitsNorth: " + subsequentHitsNorth);
                        }
                        else
                        {
                            subsequentHitsNorth = 0;
                        }
                        if (subsequentHitsNorth >= SUBSEQUENTHITS_NEEDED)
                        {
                            foundNorthY = northY + SUBSEQUENTHITS_NEEDED;
                            northIsDone = true;
                            break;
                        }
                    }
                    if (!northIsDone)
                    {
                        badImages[numberOfImage] = true;
                        Console.WriteLine("No northern border found this beauty. " + numberOfImage);
                    }
                    else
                    {
                        Console.WriteLine("Northern border found this beauty. " + numberOfImage + " x = " + foundNorthY);
                    }
                }

                //
                //southborder; pages are symetrical
                int foundSouthY = -1;
                {
                    int dy = Math.Abs(cy - foundNorthY);
                    int start = cy + dy - 20; //few steps back
                    Console.WriteLine("starting at: " + start);

                    bool southIsDone = false;
                    int subsequentHitsSouth = 0;
                    for (int southY = start; southY <= image.Height && subsequentHitsSouth < 5; southY++)
                    {
                        Color p = image.GetPixel(x_north, southY);
                        if (p.Equals(background))
                        {
                            subsequentHitsSouth++;
                            Console.WriteLine("subsequentHitsSouth: " + subsequentHitsSouth);
                        }
                        else
                        {
                            subsequentHitsSouth = 0;
                        }
                        if (subsequentHitsSouth >= SUBSEQUENTHITS_NEEDED)
                        {
                            foundSouthY = southY - SUBSEQUENTHITS_NEEDED;
                            southIsDone = true;
                            break;
                        }
                    }
                    if (!southIsDone)
                    {
                        badImages[numberOfImage] = true;
                        Console.WriteLine("No southern border found for this beauty. " + numberOfImage);
                    }
                    else
                    {
                        Console.WriteLine("Southern border found for this beauty. " + numberOfImage + " y = " + foundSouthY);
                    }
                }

                int foundEastX = -1;
                {
                    //East                    
                    int subsequentHitsEast = 0;

                    bool eastIsDone = false;
                    for (int eastX = cx; eastX >= 0 && subsequentHitsEast < 5; eastX--)
                    {
                        Color p = image.GetPixel(eastX, y_north);
                        if (p.Equals(background))
                        {
                            subsequentHitsEast++;
                            Console.WriteLine("subsequentHitsEast: " + subsequentHitsEast);
                        }
                        else
                        {
                            subsequentHitsEast = 0;
                        }
                        if (subsequentHitsEast >= SUBSEQUENTHITS_NEEDED)
                        {
                            foundEastX = eastX + SUBSEQUENTHITS_NEEDED;
                            eastIsDone = true;
                            break;
                        }
                    }
                    if (!eastIsDone)
                    {
                        badImages[numberOfImage] = true;
                        Console.WriteLine("No eastern border found for this beauty. " + numberOfImage);
                    }
                    else
                    {
                        Console.WriteLine("Eastern border found for this beauty. " + numberOfImage + " x = " + foundEastX);
                    }
                } //end east

                //
                //westborder; pages are symetrical
                int foundWestX = -1;
                {
                    int dx = Math.Abs(cx - foundEastX);
                    int start = cx + dx - 20; //few steps back
                    Console.WriteLine("starting at: " + start);

                    bool westIsDone = false;
                    int subsequentHitsWest = 0;
                    for (int westX = start; westX <= image.Width && subsequentHitsWest < 5; westX++)
                    {
                        Color p = image.GetPixel(westX, y_north);
                        if (p.Equals(background))
                        {
                            subsequentHitsWest++;
                            Console.WriteLine("subsequentHitsWest: " + subsequentHitsWest);
                        }
                        else
                        {
                            subsequentHitsWest = 0;
                        }
                        if (subsequentHitsWest >= SUBSEQUENTHITS_NEEDED)
                        {
                            foundWestX = westX - SUBSEQUENTHITS_NEEDED;
                            westIsDone = true;
                            break;
                        }
                    }
                    if (!westIsDone)
                    {
                        badImages[numberOfImage] = true;
                        Console.WriteLine("No western border found for this beauty. " + numberOfImage);
                    }
                    else
                    {
                        Console.WriteLine("Western border found for this beauty. " + numberOfImage + " y = " + foundWestX);
                    }
                } //next image
                int width = Math.Abs(foundWestX - foundEastX);
                int height = Math.Abs(foundSouthY - foundNorthY);
                int upperLeftX = foundEastX;
                int upperLeftY = foundNorthY;
                rectangles.Add(new Rectangle(upperLeftX, upperLeftY, width, height));
            }

            return rectangles;
        }
    }
}
