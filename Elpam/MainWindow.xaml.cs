using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;


namespace Elpam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IMS ims;
        Action act;
        AUTH auth;
        Timer timer;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExecDrv();
            Process current = Process.GetCurrentProcess();
            Process[] getProcess = Process.GetProcessesByName(current.ProcessName);
            if (getProcess.Count() > 1)
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " Elpam is running" + Environment.NewLine);
                Process.GetCurrentProcess().Kill();
            }

            auth = new AUTH();

            try
            {
                if (auth.FirstAuthen())
                {
                    AUTH.Authen();

                    InitializeComponent();

                    ExecDrv();
                    ETCSetting.Load();
                    if (ETCSetting.StateTopmost)
                        this.Topmost = true;
                    else
                        this.Topmost = false;
                    this.Left = ETCSetting.leftWin;
                    this.Top = ETCSetting.topWin;

                    TimerSetting.Load();
                    CaptureMesoSetting.Load();
                    Timer timer = new Timer();
                    MobCountSetting.Load();
                    RandomNumberSetting.Load();
                    FamiliarSetting.Load();
                    ScanSetting.Load();
                    ims = new IMS();
                    act = new Action();
                }
                else
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + ex.Message.ToString() + Environment.NewLine);
                Process.GetCurrentProcess().Kill();
            }
        }

        public MainWindow()
        { }

        public void Timer1_Action()
        {
            ims.Searches(this, ScanSetting.listScan1);
        }

        public void Timer2_Action()
        {
            ims.Searches(this, ScanSetting.listScan2);
        }

        private async void btnMain_Click(object sender, RoutedEventArgs e)
        {
            if (btnMain.Content.ToString() == "PAUSE")
            {
                btnMain.Content = "START";
                Timer.Timer1.Stop();
                Timer.Timer2.Stop();
                Timer.TimerPoint.Stop();
                Timer.TimerCurrentMeso.Stop();
                Background = new SolidColorBrush(Colors.White);
                btnMain.Background = new SolidColorBrush(Colors.White);
                mnuCaiDat.IsEnabled = true;

            }
            else if (btnMain.Content.ToString() == "S.OFF")
            {
                btnMain.Content = "START";
                this.Title = "Elpam";
                Timer.Timer1.Stop();
                Timer.Timer2.Stop();
                Timer.TimerPoint.Stop();
                Timer.TimerCurrentMeso.Stop();
                Action.SoundOff();
                Background = new SolidColorBrush(Colors.White);
                btnMain.Background = new SolidColorBrush(Colors.White);
                mnuCaiDat.IsEnabled = true;
            }
            else
            {
                btnMain.Content = "PAUSE";
                IMS.flagSearches = true;
                Timer.Timer1.Start();
                Timer.Timer2.Start();
                IMS.countCurrentMeso = 0;
                if (CaptureMesoSetting.State)
                {
                    Action.CaptureCurrentMeso();
                    Timer.TimerCurrentMeso.Start();
                }
                Background = new SolidColorBrush(Colors.Blue);
                btnMain.Background = new SolidColorBrush(Colors.Blue);
                mnuCaiDat.IsEnabled = false;
                IMS.flagATK = true;

                await Task.Run(() => AUTH.Authen());
            }

        }

        #region Colors
        public void SetColor_ATK_Attack()
        {
            mnuCaiDat.Background = new SolidColorBrush(Colors.Green);
        }
        public void SetColor_ATK_Default()
        {
            mnuCaiDat.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF0F0F0");
        }
        #endregion

        public void ExecDrv()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.Verb = "runas";
            p.StartInfo.FileName = "drv.bat";
            p.Start();
            string output = p.StandardOutput.ReadToEnd();

            if(output.Contains("Could not write"))
            { }
            else
            {
                MessageBox.Show("Restart ur PC");
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + output + Environment.NewLine);
                Process.GetCurrentProcess().Kill();
            }
        }

        private void btnATK_Click(object sender, RoutedEventArgs e)
        {
            ims.MobCheck(this);
        }

        private void mnuRdNumber_Click(object sender, RoutedEventArgs e)
        {
            int getRd = act.RandomAttackNumber();
            mnuRdNumber.Header = getRd.ToString();
        }

        private void btnEndTask_Click(object sender, RoutedEventArgs e)
        {
            act.EndTask();
        }

        bool flagCaiDat = true;
        private void mnuCaiDat_Click(object sender, RoutedEventArgs e)
        {
            if (btnMain.Content.ToString() == "START")
            {
                flagCaiDat = false;
                SettingForm setting = new SettingForm();
                setting.ShowDialog();
                if (ETCSetting.StateTopmost)
                    this.Topmost = true;
                else
                    this.Topmost = false;
                flagCaiDat = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Dispatcher.InvokeShutdown();
        }

        #region HOTKEY
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //(none)
        private const uint VK_F10 = 0x79;
        private const uint VK_F11 = 0x7A;
        private const uint VK_F1 = 0x70;
        private const uint VK_F3 = 0x72;
        private const uint VK_F4 = 0x73;

        private IntPtr _windowHandle;
        private HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);
            RegisterHotKey(_windowHandle, 0, MOD_NONE, VK_F1);
            RegisterHotKey(_windowHandle, 1, MOD_NONE, VK_F10);
            RegisterHotKey(_windowHandle, 2, MOD_NONE, VK_F11);
            RegisterHotKey(_windowHandle, 3, MOD_NONE, 0xC0); // Console
            RegisterHotKey(_windowHandle, 4, MOD_NONE, VK_F4);
            RegisterHotKey(_windowHandle, 5, MOD_NONE, VK_F3);
        }
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    int vkey = (((int)lParam >> 16) & 0xFFFF);
                    switch (wParam.ToInt32())
                    {
                        case 0:
                            if (flagCaiDat)
                                btnMain_Click(null, null);
                            handled = true;
                            break;
                        case 1:
                            btnATK_Click(null, null);
                            handled = true;
                            break;
                        case 2:
                            mnuRdNumber_Click(null, null);
                            handled = true;
                            break;
                        case 3:
                            btnEndTask_Click(null, null);
                            handled = true;
                            break;
                        case 4:
                            Process[] getProcess = Process.GetProcessesByName("AK");
                            if (getProcess.Count() == 0)
                                System.Diagnostics.Process.Start(@"AK.exe");
                            handled = true;
                            break;
                        case 5:
                            foreach (var process in Process.GetProcessesByName("AK"))
                            {
                                process.Kill();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("AK"))
            {
                process.Kill();
            }
        }

        private void mnuEditScript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@".AK\.script.mcr");
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show(".AK\\.script is not exist!", "Error", MessageBoxButton.OK);
            }
            catch(Exception)
            {

            }
        }
    }
}