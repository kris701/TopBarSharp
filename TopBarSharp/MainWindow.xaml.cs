using Nanotek.WPF.WindowsTrayItemFramework;
using System.ComponentModel;
using System.IO;
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
