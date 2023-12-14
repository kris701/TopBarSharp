using TopBarSharp.Models;
using TopBarSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
using System.IO;
using Nanotek.WPF.WindowsTrayItemFramework;

namespace TopBarSharp.Views
{
    public partial class MainView : UserControl, TrayWindowSwitchable
    {
        public UIElement Element { get; }
        public double TargetWidth { get; } = 300;
        public double TargetHeight { get; } = 140;

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
                _targetInfo = JsonSerializer.Deserialize<TargetInfo>(File.ReadAllText(_saveFile));
                if (_targetInfo != null)
                {
                    TargetLabel.Content = "Seaching...";
                    Process? target = null;
                    for (int i = 0; i < 10; i++)
                    {
                        if (_stop)
                            break;
                        target = Process.GetProcesses().SingleOrDefault(p => p.ProcessName == _targetInfo.ProcessName && p.MainWindowTitle == _targetInfo.WindowName);
                        if (target != null)
                        {
                            SetTargetProcess(target);
                            await Start();
                            return;
                        }
                        await Task.Delay(5000);
                    }
                    TargetLabel.Content = "Target not found!";
                }
            }
        }

        private async void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            ControlPanel.IsEnabled = false;
            SelectorCountdownLabel.Visibility = Visibility.Visible;
            for (int i = 3; i > 0; i--)
            {
                SelectorCountdownLabel.Content = $"{i}";
                await Task.Delay(1000);
            }
            SelectorCountdownLabel.Visibility = Visibility.Hidden;

            var pt = Win32APIManager.GetCursorPosition();
            var ptr = Win32APIManager.WindowFromPoint(pt.X, pt.Y);
            if (ptr != IntPtr.Zero)
            {
                var target = Process.GetProcesses().SingleOrDefault(p => p.MainWindowHandle == ptr);
                if (target != null)
                    SetTargetProcess(target);
            }
            if (_targetProcess != null)
            {
                _targetInfo = new TargetInfo(_targetProcess.ProcessName, _targetProcess.MainWindowTitle);
                var text = JsonSerializer.Serialize(_targetInfo);
                if (File.Exists(_saveFile))
                    File.Delete(_saveFile);
                File.WriteAllText(_saveFile, text);
            }

            ControlPanel.IsEnabled = true;
        }

        private void SetTargetProcess(Process target)
        {
            _targetProcess = target;
            TargetLabel.Content = $"({_targetProcess.ProcessName}) {_targetProcess.MainWindowTitle}";
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
                if (!_shown)
                {
                    var ptr = Win32APIManager.WindowFromPoint(pt.X, pt.Y);
                    if (ptr != IntPtr.Zero && _targetProcess.MainWindowHandle == ptr)
                    {
                        _shown = true;
                        Win32APIManager.Move(_targetProcess.MainWindowHandle, _targetProcessRect.Left, 0);
                    }
                }
                else
                {
                    if (!(pt.X >= _targetProcessRect.Left &&
                        pt.X <= _targetProcessRect.Left + _targetProcessRect.Right &&
                        pt.Y >= _targetProcessRect.Top &&
                        pt.Y <= _targetProcessRect.Top + _targetProcessRect.Bottom))
                    {
                        await Task.Delay(1000);
                        _shown = false;
                        Win32APIManager.Move(_targetProcess.MainWindowHandle, _targetProcessRect.Left, _selectorOffset - _targetProcessRect.Bottom);
                    }
                }
            }
            _running = false;
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
    }
}
