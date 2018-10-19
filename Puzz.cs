using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Xml.Serialization;

namespace Puzzle
{
    [Serializable]
    public class Puzz
    {
        public string Name { get; set; }
        public string path { get; set; }
        public string oldpath { get; set; }
        public int Divider { get; set; }
        public string Size { get; set; }
        public List<Piece> Pieces { get; set; }
        

        public Puzz(string _path, int divider)
        {
            Name = _path.ToString().Split('/', '\\').Last().Split('.').First();
            Divider = divider;
            path = (Directory.GetCurrentDirectory() + "\\" + "puzzles\\" + Name + Divider + "\\"+Name+".jpg");
            oldpath = _path;
            Pieces = new List<Piece>();
            Size = " "+divider + " X " + divider;
            
        }

        public Puzz() { }

        public string CreateDirectory() {
            string dirname = "puzzles\\" + Name + Divider + "\\";            
            if (!Directory.Exists(dirname))
                Directory.CreateDirectory(dirname);
            return dirname;
        }
           
        public void CopyFile() {
            if (!File.Exists(path))
            {
                File.Copy(oldpath, path);                
            }
        }

        public void CreatePuzzle()
        {
            var dest=CreateDirectory();
            CopyFile();

            Point point = (Divider == 4) ? new Point(192,192) : (Divider == 6) ? new Point(128, 128) : (Divider == 8) ? new Point(96, 96) : new Point(0, 0);

            var imgarray = new Image[Divider * Divider];
            var img = Image.FromFile(path);
            img = resizeImage(img);
            for (int i = 0; i < Divider; i++)
            {
                for (int j = 0; j < Divider; j++)
                {
                    var index = i * Divider + j;
                    imgarray[index] = new Bitmap(point.X, point.Y);
                    var graphics = Graphics.FromImage(imgarray[index]);
                    graphics.DrawImage(img, new Rectangle(0, 0, point.X, point.Y), new Rectangle(i * point.X, j * point.Y, point.X, point.Y), GraphicsUnit.Pixel);
                    graphics.Dispose();
                }
            }
            dest = Directory.GetCurrentDirectory() + "\\"+dest;
            for (var i = 0; i < imgarray.Length; i++) {
                var miniName = i + "-" + Name + ".jpg";
                var miniPath = dest + miniName;
                Pieces.Add(new Piece(miniName,miniPath));
                imgarray[i].Save(dest + "\\" + miniName);
                Pieces[i].FillSides();
            }
            MyCollection.Save(this);
        }

        public Image resizeImage(Image imgToResize)
        {
            return (Image)(new Bitmap(imgToResize, new Size(768, 768)));
        }       
    }
}
