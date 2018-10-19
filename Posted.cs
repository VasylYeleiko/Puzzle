using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle
{
    public class Posted
    {
        public int Place { get; set; }
        public int Index { get; set; }        

        public Posted(int place,int index)
        {
            Index = index;
            Place = place;           
        }
    }
}
