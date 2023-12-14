using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopBarSharp.Models
{
    public class TargetInfo
    {
        public string ProcessName { get; set; }
        public string WindowName { get; set; }

        public TargetInfo(string processName, string windowName)
        {
            ProcessName = processName;
            WindowName = windowName;
        }
    }
}
