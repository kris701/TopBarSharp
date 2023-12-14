﻿using TopBarSharp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TopBarSharp
{
    public static class Win32APIManager
    {
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref MyRect rectangle);
        public static MyRect GetWindowLocation(IntPtr hwnd)
        {
            var rect = new MyRect();
            GetWindowRect(hwnd, ref rect);
            return rect;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref MyPoint lpPoint);
        public static MyPoint GetCursorPosition()
        {
            var lpPoint = new MyPoint();
            GetCursorPos(ref lpPoint);
            return lpPoint;
        }

        public static void Move(IntPtr hwnd, int x, int y)
        {
            //const short SWP_NOMOVE = 0X2;
            const short SWP_NOSIZE = 1;
            const short SWP_NOZORDER = 0X4;
            //const int SWP_SHOWWINDOW = 0x0040;

            SetWindowPos(hwnd, 0, x, y, 0, 0, SWP_NOZORDER | SWP_NOSIZE);
        }
    }
}
