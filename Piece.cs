using Puzzle;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Puzzle
{
    [Serializable]
    public class Piece
    {
        public string Name { get; set; }
        public string Minipath { get; set; }
        public List<MyColor> LeftSide = new List<MyColor>();
        public List<MyColor> RightSide = new List<MyColor>();
        public List<MyColor> UpSide = new List<MyColor>();
        public List<MyColor> BottomSide = new List<MyColor>();


        public Piece(string imagePath, string path)
        {
            Name = imagePath;
            Minipath = path;            
        }

        public void FillSides()
        {
            var img = new Bitmap(Image.FromFile(Minipath));            
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (i == 0)
                    {
                        if (j % 3 == 0)
                        {
                            LeftSide.Add(ControlPoint(i, j));
                        }
                    }
                    if (i == img.Width-1)
                    {
                        if (j % 3 == 0)
                        {
                            RightSide.Add(ControlPoint(i, j));
                        }
                    }
                    if (j == 0)
                    {
                        if (i % 3 == 0)
                        {
                            UpSide.Add(ControlPoint(i, j));
                        }
                    }
                    if (j == img.Width - 1)
                    {
                        if (i % 3 == 0)
                        {
                            BottomSide.Add(ControlPoint(i, j));
                        }
                    } 
                }
            }
        }     

        private MyColor ControlPoint(int i, int j)
        {
            using (Bitmap bmp = new Bitmap(System.Drawing.Image.FromFile(Minipath)))
            {
                var clr = bmp.GetPixel(i, j); 
                byte red = clr.R;
                byte green = clr.G;
                byte blue = clr.B;
                byte a = clr.A;
                return new MyColor(red, green, blue, a);
            }
        }

        public Piece() { }
    }
}
