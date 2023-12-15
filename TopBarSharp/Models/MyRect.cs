namespace TopBarSharp.Models
{
    public struct MyRect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public bool IsPointWithin(MyPoint point) =>
            (point.X >= Left &&
             point.X <= Left + Right &&
             point.Y >= Top &&
             point.Y <= Top + Bottom);
    }
}
