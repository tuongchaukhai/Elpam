using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Elpam
{
    public class Timer
    {
        SoundPlayer soundP = new SoundPlayer(@".Music\music.wav");
        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        

        private static DispatcherTimer timer1;
        private static DispatcherTimer timer2;

        private static DispatcherTimer timerSuperSlow;
        private static DispatcherTimer timerSlow;
        private static DispatcherTimer timerNormal;
        private static DispatcherTimer timerFast;
        private static DispatcherTimer timerATK;
        private static DispatcherTimer timerPoint;
        private static DispatcherTimer timerCurrentMeso;
        private static DispatcherTimer timerEndTask;

        public static int timerAKDelay, timerCurrentMesoDelay;

        public Timer()
        {
            #region Input delay
            #endregion

            #region Sounds
            TimerSuperSlow = new DispatcherTimer();
            TimerSuperSlow.Tick += TimerSuperSlow_Tick;
            TimerSuperSlow.Interval = new TimeSpan(0, 0, 0, 0, TimerSetting.TimerSuperSlowDelay);

            TimerSlow = new DispatcherTimer();
            TimerSlow.Tick += TimerSlow_Tick;
            TimerSlow.Interval = new TimeSpan(0, 0, 0, 0, TimerSetting.TimerSlowDelay);

            TimerNormal = new DispatcherTimer();
            TimerNormal.Tick += TimerNormal_Tick;
            TimerNormal.Interval = new TimeSpan(0, 0, 0, 0, TimerSetting.TimerNormalDelay);

            TimerFast = new DispatcherTimer();
            TimerFast.Tick += TimerFast_Tick;
            TimerFast.Interval = new TimeSpan(0, 0, 0, 0, TimerSetting.TimerFastDelay);
            #endregion

            Timer1 = new DispatcherTimer();
            Timer1.Tick += Timer1_Tick;
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, TimerSetting.Timer1Delay);

            Timer2 = new DispatcherTimer();
            Timer2.Tick += Timer2_Tick;
            Timer2.Interval = new TimeSpan(0, 0, 0, 0, TimerSetting.Timer2Delay);

            TimerATK = new DispatcherTimer();
            TimerATK.Tick += TimerATK_Tick;
            TimerATK.Interval = new TimeSpan(0, 0, 0, 0, TimerSetting.TimerATKDelay);

            TimerPoint = new DispatcherTimer();
            TimerPoint.Tick += TimerPoint_Tick;
            TimerPoint.Interval = new TimeSpan(0, 0, 0, 0, TimerSetting.TimerPointDelay);

            TimerCurrentMeso = new DispatcherTimer();
            TimerCurrentMeso.Tick += TimerCurrentMeso_Tick;
            TimerCurrentMeso.Interval = new TimeSpan(0, 0, 0, 0, CaptureMesoSetting.Timer);

            TimerEndTask = new DispatcherTimer();
            TimerEndTask.Tick += TimerEndTask_Tick;
            TimerEndTask.Interval = new TimeSpan(0, 0, 0, 0, 2000);
        }

        public static DispatcherTimer TimerSuperSlow { get => timerSuperSlow; set => timerSuperSlow = value; }
        public static DispatcherTimer TimerSlow { get => timerSlow; set => timerSlow = value; }
        public static DispatcherTimer TimerNormal { get => timerNormal; set => timerNormal = value; }
        public static DispatcherTimer TimerFast { get => timerFast; set => timerFast = value; }

        public static DispatcherTimer Timer1 { get => timer1; set => timer1 = value; }
        public static DispatcherTimer Timer2 { get => timer2; set => timer2 = value; }

        public static DispatcherTimer TimerATK { get => timerATK; set => timerATK = value; }
        public static DispatcherTimer TimerPoint { get => timerPoint; set => timerPoint = value; }
        public static DispatcherTimer TimerCurrentMeso { get => timerCurrentMeso; set => timerCurrentMeso = value; }
        public static DispatcherTimer TimerEndTask { get => timerEndTask; set => timerEndTask = value; }

        #region Sounds_Tick
        private void TimerSuperSlow_Tick(object sender, EventArgs e)
        {
            soundP.Play();
        }
        private void TimerSlow_Tick(object sender, EventArgs e)
        {
            soundP.Play();
        }
        private void TimerNormal_Tick(object sender, EventArgs e)
        {
            soundP.Play();
        }
        private void TimerFast_Tick(object sender, EventArgs e)
        {
            soundP.Play();
        }
        #endregion

        private void Timer1_Tick(object sender, EventArgs e)
        {
            mainWindow.Timer1_Action();
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            mainWindow.Timer2_Action();
        }

        private void TimerATK_Tick(object sender, EventArgs e)
        {
            TimerATK.Stop();
            mainWindow.SetColor_ATK_Default();
            IMS.flagATK = true;
        }

        private void TimerPoint_Tick(object sender, EventArgs e)
        {
            IMS.ScanPoint();
        }

        //private void TimerAK_Tick(object sender, EventArgs e)
        //{
        //    AK.AtKs();
        //}

        private void TimerCurrentMeso_Tick(object sender, EventArgs e)
        {
            IMS.CurrenMesoScan();
        }


        private void TimerEndTask_Tick(object sender, EventArgs e)
        {
            TimerEndTask.Stop();
            Action.endTaskCount = 0;

            if (mainWindow.btnMain.Content.ToString() == "PAUSE")
                mainWindow.Background = new SolidColorBrush(Colors.Blue);
            else if (mainWindow.btnMain.Content.ToString() == "S.OFF")
                mainWindow.Background = new SolidColorBrush(Colors.Red);
            else
                mainWindow.Background = new SolidColorBrush(Colors.White);
        }
    }
}
