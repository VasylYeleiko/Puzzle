using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle
{
    [Serializable]
    public class MyColor
    {
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
        public int a { get; set; }

        public MyColor(int _r,int _g,int _b,int _a)
        {
            a = _a;
            r = _r;
            g = _g;
            b = _b;
        }

        public MyColor() { }
    }
}
