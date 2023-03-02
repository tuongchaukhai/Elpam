using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Elpam
{
    /// <summary>
    /// Interaction logic for ScanAddForm.xaml
    /// </summary>
    public partial class ScanAddForm : Window
    {
        public ScanAddForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            List<ScanSetting> combineList = new List<ScanSetting>();
            combineList.AddRange(ScanSetting.listScan1);
            combineList.AddRange(ScanSetting.listScan2);

            foreach (var item in combineList)
            {
                if (item.Name == txtName.Text && item.Path == txtPath.Text)
                {
                    System.Windows.MessageBox.Show("This name & path already exists!");
                    txtName.Focus();
                    return;
                }
            }
            ScanSetting scan = new ScanSetting();
            scan.State = false;
            scan.Name = txtName.Text;
            scan.Path = txtPath.Text;
            scan.X = Int32.Parse(txtX.Text);
            scan.Y = Int32.Parse(txtY.Text);
            scan.XX = Int32.Parse(txtXX.Text);
            scan.YY = Int32.Parse(txtYY.Text);
            if (txtExRatio.Text.Length > 0)
                scan.ExRatio = txtExRatio.Text;
            else
                scan.ExRatio = "";
            scan.TimerId = Int32.Parse(cbbTimer.Text);
            scan.SoundLevel = Int32.Parse(cbbSound.Text);
            scan.PauseMacro = bool.Parse(cbbPause.Text);
            if (txtPsDelay.IsEnabled)
                scan.PauseDelay = Int32.Parse(txtX.Text);
            else
                scan.PauseDelay = 0;
            ScanSetting.Add(scan);
            ScanSetting.SaveToFile();
            this.Close();
        }

        private void cbbPause_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbbPause.Text == "true")
            {
                txtPsDelay.IsEnabled = true;
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnAdd.IsEnabled = true;
            }
        }

        private void txtPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnAdd.IsEnabled = true;
            }
        }

        private void txtX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnAdd.IsEnabled = true;
            }
        }

        private void txtY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnAdd.IsEnabled = true;
            }
        }

        private void txtXX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnAdd.IsEnabled = true;
            }
        }

        private void txtYY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtName.Text.Length > 0 && txtPath.Text.Length > 0 && txtX.Text != txtXX.Text && txtY.Text != txtYY.Text)
            {
                btnAdd.IsEnabled = true;
            }
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
