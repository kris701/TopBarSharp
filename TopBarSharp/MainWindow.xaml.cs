using TopBarSharp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nanotek.WPF.WindowsTrayItemFramework;
using TopBarSharp.Views;

namespace TopBarSharp
{
    public partial class MainWindow : TrayWindow
    {
        private static MainView _mainView = new MainView();
        public MainWindow() : base("TopBarSharp", GetEmbeddedIcon(), _mainView)
        {
            InitializeComponent();
        }

        private static System.Drawing.Icon GetEmbeddedIcon()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            var st = a.GetManifestResourceStream("TopBarSharp.icon.ico");
            if (st == null)
                throw new FileNotFoundException("Error! Icon file not found!");
            return new System.Drawing.Icon(st);
        }

        private void TrayWindow_Closing(object sender, CancelEventArgs e)
        {
            _mainView.ReturnWindowToView();
        }
    }
}
