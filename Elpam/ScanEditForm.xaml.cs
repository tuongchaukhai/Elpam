using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Elpam
{
    /// <summary>
    /// Interaction logic for ScanEditForm.xaml
    /// </summary>
    public partial class ScanEditForm : Window
    {
        public static ScanSetting scan;
        public ScanEditForm()
        {
            InitializeComponent();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            ScanSetting scanEdit = new ScanSetting();
            scanEdit.State = state;
            scanEdit.Name = txtName.Text;
            scanEdit.Path = txtPath.Text;
            scanEdit.X = Int32.Parse(txtX.Text);
            scanEdit.Y = Int32.Parse(txtY.Text);
            scanEdit.XX = Int32.Parse(txtXX.Text);
            scanEdit.YY = Int32.Parse(txtYY.Text);
            if (txtExRatio.Text.Length > 0)
                scanEdit.ExRatio = txtExRatio.Text;
            else
                scanEdit.ExRatio = "";
            scanEdit.TimerId = Int32.Parse(cbbTimer.Text);
            scanEdit.SoundLevel = Int32.Parse(cbbSound.Text);
            scanEdit.PauseMacro = bool.Parse(cbbPause.Text);
            if (txtPsDelay.IsEnabled)
                scanEdit.PauseDelay = Int32.Parse(txtX.Text);
            else
                scanEdit.PauseDelay = 0;

            ScanSetting.Delete(scan);
            ScanSetting.Add(scanEdit);
            ScanSetting.SaveToFile();
            this.Close();
        }

        private void cbbPause_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbPause.Text == "true")
            {
                txtPsDelay.IsEnabled = true;
            }
        }

        private void txtPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnApply.IsEnabled = true;
            }
        }

        private void txtX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnApply.IsEnabled = true;
            }
        }

        private void txtY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnApply.IsEnabled = true;
            }
        }

        private void txtXX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnApply.IsEnabled = true;
            }
        }

        private void txtYY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnApply.IsEnabled = true;
            }
        }

        bool state;
        string[] name = { "RedPoint", "BuddyPoint", "GuildPoint", "Warp", "Wrong", "Familiar" };
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool exists = Array.Exists(name, element => element == scan.Name);
            if (exists)
            {
                txtName.IsEnabled = false;
                cbbTimer.IsEnabled = false;
                txtExRatio.IsEnabled = false;
                if (scan.Name.Contains("Point"))
                    txtPath.IsEnabled = false;
            }
            state = scan.State;
            txtName.Text = scan.Name;
            txtPath.Text = scan.Path;
            txtX.Text = scan.X.ToString();
            txtY.Text = scan.Y.ToString();
            txtXX.Text = scan.XX.ToString();
            txtYY.Text = scan.YY.ToString();
            txtExRatio.Text = scan.ExRatio;
            if (scan.TimerId == 1)
                cbbTimer.SelectedIndex = 0;
            else
                cbbTimer.SelectedIndex = 1;

            switch (scan.SoundLevel)
            {
                case 0:
                    cbbSound.SelectedIndex = 0;
                    break;
                case 1:
                    cbbSound.SelectedIndex = 1;
                    break;
                case 2:
                    cbbSound.SelectedIndex = 2;
                    break;
                case 3:
                    cbbSound.SelectedIndex = 3;
                    break;
                case 4:
                    cbbSound.SelectedIndex = 4;
                    break;
            }

            if (scan.PauseMacro)
                cbbPause.SelectedIndex = 0;
            else
                cbbPause.SelectedIndex = 1;

            txtPsDelay.Text = scan.PauseDelay.ToString();
        }

        #region HOTKEY
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr _windowHandle;
        private HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);
            RegisterHotKey(_windowHandle, 0, 0x0002, 0x31);
            RegisterHotKey(_windowHandle, 1, 0x0002, 0x32);
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
                            btnGetXY_Click(null, null);
                            handled = true;
                            break;
                        case 1:
                            btnGetXXYY_Click(null, null);
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
        #endregion

        public Point GetMousePositionWindowsForms()
        {
            var point = System.Windows.Forms.Control.MousePosition;
            return new Point(point.X, point.Y);
        }


        private void btnGetXY_Click(object sender, RoutedEventArgs e)
        {
            var pos = GetMousePositionWindowsForms();
            txtX.Text = pos.X.ToString();
            txtY.Text = pos.Y.ToString();
        }

        private void btnGetXXYY_Click(object sender, RoutedEventArgs e)
        {
            var pos = GetMousePositionWindowsForms();
            txtXX.Text = pos.X.ToString();
            txtYY.Text = pos.Y.ToString();
        }
    }
}
