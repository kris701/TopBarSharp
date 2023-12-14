using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TopBarSharp.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MyPoint
    {
        public int X;
        public int Y;

        public static implicit operator Point(MyPoint point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
