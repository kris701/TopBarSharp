using System.Drawing;
using System.Runtime.InteropServices;

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
