using Nanotek.WPF.WindowsTrayItemFramework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TopBarSharp.Models;

namespace TopBarSharp.Views
{
    public partial class MainView : UserControl, TrayWindowSwitchable
    {
        public UIElement Element { get; }
        public double TargetWidth { get; } = 300;
        public double TargetHeight { get; } = 170;

        private int _selectorOffset = 20;
        private Process? _targetProcess;
        private MyRect _targetProcessRect;
        private bool _shown = false;
        private bool _stop = false;
        private bool _running = false;
        private TargetInfo? _targetInfo;
        private string _saveFile = "save.json";

        public MainView()
        {
            InitializeComponent();
            Element = this;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(_saveFile))
            {
                _targetInfo = new TargetInfo();
                _targetInfo.Load(_saveFile);
                TargetTextbox.Text = _targetInfo.WindowName;
                if (_targetInfo != null)
                {
                    TargetLabel.Content = "Seaching...";
                    while(true)
                    {
                        if (_stop)
                            break;
                        Process? target = FindTarget(_targetInfo.WindowName);
                        if (target != null)
                        {
                            SetTargetProcess(target, _targetInfo.WindowName);
                            await Start();
                            return;
                        }
                        await Task.Delay(1000);
                    }
                    TargetLabel.Content = "Target not found!";
                }
            }
        }

        private async void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            ControlPanel.IsEnabled = false;
            await SelectorCountdown(3);

            var pt = Win32APIManager.GetCursorPosition();
            var ptr = Win32APIManager.WindowFromPoint(pt.X, pt.Y);
            if (ptr != IntPtr.Zero)
            {
                var target = Process.GetProcesses().FirstOrDefault(p => p.MainWindowHandle == ptr);
                if (target != null)
                    SetTargetProcess(target, target.MainWindowTitle);
            }

            ControlPanel.IsEnabled = true;
        }

        private async Task SelectorCountdown(int count)
        {
            SelectorCountdownLabel.Content = $"{count}";
            SelectorCountdownLabel.Visibility = Visibility.Visible;
            for (int i = count; i > 0; i--)
            {
                SelectorCountdownLabel.Content = $"{i}";
                await Task.Delay(1000);
            }
            SelectorCountdownLabel.Visibility = Visibility.Hidden;
        }

        private void SetTargetProcess(Process target, string name)
        {
            _targetProcess = target;
            TargetLabel.Content = $"{_targetProcess.MainWindowTitle}";
            _targetInfo = new TargetInfo(name);
            _targetInfo.Save(_saveFile);
            StartButton.IsEnabled = true;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            await Start();
        }

        private async Task Start()
        {
            if (_targetProcess == null)
                return;

            while (_running)
            {
                _stop = true;
                await Task.Delay(100);
            }

            TargetPanel.IsEnabled = false;
            StartButton.IsEnabled = false;
            _running = true;
            _targetProcessRect = Win32APIManager.GetWindowLocation(_targetProcess.MainWindowHandle);
            Win32APIManager.Move(_targetProcess.MainWindowHandle, _targetProcessRect.Left, 0);
            _targetProcessRect = Win32APIManager.GetWindowLocation(_targetProcess.MainWindowHandle);
            Win32APIManager.Move(_targetProcess.MainWindowHandle, _targetProcessRect.Left, _selectorOffset - _targetProcessRect.Bottom);
            _stop = false;
            while (!_stop)
            {
                await Task.Delay(100);
                var pt = Win32APIManager.GetCursorPosition();
                var active = Win32APIManager.GetForegroundWindow();
                if (!_shown)
                {
                    var ptr = Win32APIManager.WindowFromPoint(pt.X, pt.Y);
                    if (IsPtrTarget(ptr) || IsPtrTarget(active))
                    {
                        _shown = true;
                        Win32APIManager.Move(_targetProcess.MainWindowHandle, _targetProcessRect.Left, 0);
                    }
                }
                else
                {
                    if (!_targetProcessRect.IsPointWithin(pt) &&
                        !IsPtrTarget(active))
                    {
                        await Task.Delay(1000);
                        _shown = false;
                        Win32APIManager.Move(_targetProcess.MainWindowHandle, _targetProcessRect.Left, _selectorOffset - _targetProcessRect.Bottom);
                    }
                }
            }
            _running = false;
            StartButton.IsEnabled = true;
            TargetPanel.IsEnabled = true;
        }

        private bool IsPtrTarget(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero || _targetProcess == null)
                return false;
            return _targetProcess.MainWindowHandle == ptr;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _stop = true;
            ReturnWindowToView();
        }

        public void ReturnWindowToView()
        {
            if (_targetProcess == null)
                return;
            Win32APIManager.Move(_targetProcess.MainWindowHandle, _targetProcessRect.Left, 0);
        }

        private void GitButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://github.com/kris701/TopBarSharp", UseShellExecute = true });
        }

        private async void TargetTextbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (TargetTextbox.Text == "")
                return;

            TargetLabel.Content = "Seaching...";
            await Task.Delay(10);
            Process? target = FindTarget(TargetTextbox.Text);
            if (target != null)
            {
                SetTargetProcess(target, TargetTextbox.Text);
                return;
            }
            await Task.Delay(1000);
            TargetLabel.Content = "Target not found!";
        }

        private Process? FindTarget(string windowName)
        {
            var processes = Process.GetProcesses();
            var regularWindowName = WildCardToRegular(windowName);
            foreach (var process in processes)
            {
                if (Regex.IsMatch(process.MainWindowTitle, regularWindowName))
                    return process;
            }
            return null;
        }

        //https://stackoverflow.com/a/30300521
        private string WildCardToRegular(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }
    }
}
