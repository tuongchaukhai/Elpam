using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Elpam
{
    /// <summary>
    /// Interaction logic for SettingForm.xaml
    /// </summary>
    public partial class SettingForm : Window
    {
        public SettingForm()
        {
            InitializeComponent();

            //Scan
            RefreshGrid();
            //Timer
            //TimerSetting.Load();
            GetTimerList();
            //Familiar
            GetFamiliar();
            //CaptureMeso
            GetCaptureMeso();
            //MobCount
            GetMobCount();
            //RandomNumber;
            GetRandomNumber();
            //ETC
            GetETC();
        }


        private void btnScanAdd_Click(object sender, RoutedEventArgs e)
        {
            ScanAddForm sAForm = new ScanAddForm();
            sAForm.ShowDialog();
            RefreshGrid();
        }

        //public static string nameScanGet { get; set; }
        private void btnScanEdit_Click(object sender, RoutedEventArgs e)
        {
            ScanSetting select = (ScanSetting)dgScan.SelectedItem;
            ScanEditForm.scan = select;
            if (select != null)
            {
                //nameScanGet = select.Name;
                ScanEditForm edit = new ScanEditForm();
                edit.ShowDialog();
                RefreshGrid();
                dgScan.UnselectAll();
            }
        }

        string[] name = { "RedPoint", "BuddyPoint", "GuildPoint", "Warp", "Wrong", "Familiar" };
        private void btnScanDelete_Click(object sender, RoutedEventArgs e)
        {
            ScanSetting select = (ScanSetting)dgScan.SelectedItem;
            bool exists = Array.Exists(name, element => element == select.Name);
            if (select != null && !exists)
            {
                if (MessageBox.Show("Delete " + select.Name + "?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ScanSetting.Delete(select);
                    ScanSetting.SaveToFile();
                    RefreshGrid();
                }
            }
        }

        List<ScanSetting> combineList;
        private void RefreshGrid()
        {
            combineList = new List<ScanSetting>();
            combineList.AddRange(ScanSetting.listScan1);
            combineList.AddRange(ScanSetting.listScan2);
            dgScan.ItemsSource = combineList;
        }

        private void dgScan_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ScanSetting.UpdateState(combineList);
        }

        /// <summary>
        /// Timer
        /// </summary>
        private void GetTimerList()
        {
            txtTimer1.Text = TimerSetting.Timer1Delay.ToString();
            txtTimer2.Text = TimerSetting.Timer2Delay.ToString();
            txtTimerPoint.Text = TimerSetting.TimerPointDelay.ToString();
            txtTimerATK.Text = TimerSetting.TimerATKDelay.ToString();
            txtTimerSuperSlow.Text = TimerSetting.TimerSuperSlowDelay.ToString();
            txtTimerSlow.Text = TimerSetting.TimerSlowDelay.ToString();
            txtTimerNormal.Text = TimerSetting.TimerNormalDelay.ToString();
            txtTimerFast.Text = TimerSetting.TimerFastDelay.ToString();
        }
        private void UpdateTimerList()
        {
            TimerSetting timerSetting = new TimerSetting();
            timerSetting.Update(Int32.Parse(txtTimer1.Text), Int32.Parse(txtTimer2.Text), Int32.Parse(txtTimerPoint.Text), Int32.Parse(txtTimerATK.Text), Int32.Parse(txtTimerSuperSlow.Text), Int32.Parse(txtTimerSlow.Text), Int32.Parse(txtTimerNormal.Text), Int32.Parse(txtTimerFast.Text));
        }
        
        /// <summary>
        /// Familiar
        /// </summary>
        private void GetFamiliar()
        {
            txtMaxCountIfFound.Text = FamiliarSetting.MaxCountIfFound.ToString();
            if (FamiliarSetting.StateAutoFuel)
                cbbStateAutoFuel.SelectedIndex = 0;
            else
                cbbStateAutoFuel.SelectedIndex = 1;
            txtMaxFuelPress.Text = FamiliarSetting.MaxFuelPress.ToString();
        }

        private void UpdateFamiliar()
        {
            FamiliarSetting familiarSetting = new FamiliarSetting();
            familiarSetting.Update(Int32.Parse(txtMaxCountIfFound.Text), bool.Parse(cbbStateAutoFuel.Text), Int32.Parse(txtMaxFuelPress.Text)); 
        }


        /// <summary>
        /// CaptureMeso
        /// </summary>
        private void GetCaptureMeso()
        {
            if (CaptureMesoSetting.State)
                cbbStateCaptureMeso.SelectedIndex = 0;
            else
                cbbStateCaptureMeso.SelectedIndex = 1;

            txtCMRegionX.Text = CaptureMesoSetting.splitCMRegionX.ToString();
            txtCMRegionY.Text = CaptureMesoSetting.splitCMRegionY.ToString();
            txtCMRegionXX.Text = CaptureMesoSetting.splitCMRegionXX.ToString();
            txtCMRegionYY.Text = CaptureMesoSetting.splitCMRegionYY.ToString();
            txtCaptureMesoTimer.Text = CaptureMesoSetting.Timer.ToString();
            txtCaptureMesoLoop.Text = CaptureMesoSetting.Loop.ToString();
        }

        private void UpdateCaptureMeso()
        {
            CaptureMesoSetting captureMesoSetting = new CaptureMesoSetting();
            captureMesoSetting.Update(bool.Parse(cbbStateCaptureMeso.Text), Int32.Parse(txtCMRegionX.Text), Int32.Parse(txtCMRegionY.Text), Int32.Parse(txtCMRegionXX.Text), Int32.Parse(txtCMRegionYY.Text), Int32.Parse(txtCaptureMesoTimer.Text), Int32.Parse(txtCaptureMesoLoop.Text));
        }

        /// <summary>
        /// MobCount
        /// </summary>
        private void GetMobCount()
        {
            txtMCWidth.Text = MobCountSetting.Width.ToString();
            txtMCColumn.Text = MobCountSetting.Col.ToString();
            txtMCCount.Text = MobCountSetting.Count.ToString();
            txtMCRow.Text = MobCountSetting.Row;
        }

        private void UpdateMobCount()
        {
            MobCountSetting mobCountSetting = new MobCountSetting();
            mobCountSetting.Update(Int32.Parse(txtMCWidth.Text), Int32.Parse(txtMCColumn.Text), Int32.Parse(txtMCCount.Text), txtMCRow.Text);
        }

        /// <summary>
        /// RandomNumber
        /// </summary>
        private void GetRandomNumber()
        {
            txtRNRdSize.Text = RandomNumberSetting.Size.ToString();
            txtRNMaxLoop.Text = RandomNumberSetting.Loop.ToString();
        }

        private void UpdateRandomNumber()
        {
            RandomNumberSetting randomNumberSetting = new RandomNumberSetting();
            randomNumberSetting.Update(Int32.Parse(txtRNRdSize.Text), Int32.Parse(txtRNMaxLoop.Text));
        }

        private void GetETC()
        {
            txtWrongLoop.Text = ETCSetting.WrongLoop.ToString();
            txtETTitle.Text = ETCSetting.EndTaskTitle;
            txtETMaxPress.Text = ETCSetting.EndTaskMaxPress.ToString();
            if (ETCSetting.StateTopmost)
                cbbTopmost.SelectedIndex = 0;
            else
                cbbTopmost.SelectedIndex = 1;
            txtWinPosX.Text = ETCSetting.leftWin.ToString();
            txtWinPosY.Text = ETCSetting.topWin.ToString();
        }

        private void UpdateETC()
        {
            ETCSetting eTCSetting = new ETCSetting();
            eTCSetting.Update(Int32.Parse(txtWrongLoop.Text), txtETTitle.Text, Int32.Parse(txtETMaxPress.Text), bool.Parse(cbbTopmost.Text), Int32.Parse(txtWinPosX.Text), Int32.Parse(txtWinPosY.Text));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateTimerList();
            UpdateCaptureMeso();
            Timer timer = new Timer();



            UpdateFamiliar();

            UpdateCaptureMeso();

            UpdateMobCount();

            UpdateRandomNumber();

            UpdateETC();

            IMS ims = new IMS();
            Action act = new Action();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void btnGetPosWin_Click(object sender, RoutedEventArgs e)
        {
            txtWinPosX.Text = Application.Current.MainWindow.Left.ToString();
            txtWinPosY.Text = Application.Current.MainWindow.Top.ToString();
        }
    }
}
