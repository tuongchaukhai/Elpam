using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Elpam
{
    internal class IMS
    {
        static MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        static SoundPlayer soundP = new SoundPlayer(@".Music\music.wav");

        [DllImport("IMS.dll")]
        private static extern IntPtr ImageSearch(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPStr)] string imagePath);

        public static bool NormalSearch(int x, int y, int xx, int yy, string imgPath)
        {
            string str = Marshal.PtrToStringAnsi(ImageSearch(x, y, xx, yy, imgPath));
            if (str[0] == '0')
            {
                return false;
            }
            return true;
        }

        private (bool, ScanSetting) Result(List<ScanSetting> getSO)
        {
            flagSearches = false;
            foreach (var scan in getSO)
            {
                try
                {
                    if (scan.State)
                    {
                        if (scan.Name != "Wrong" && scan.Name != "Familiar" && scan.Name != "Warp")
                        {
                            if (scan.ExRatio.Length > 0)
                            {
                                if (AccuracySearch(scan.Path, scan.X, scan.Y, scan.XX, scan.YY, Double.Parse(scan.ExRatio)).Y != -1)
                                {
                                    flagSearches = true;
                                    return (true, scan);
                                }
                            }
                            else
                            {
                                if (NormalSearch(scan.X, scan.Y, scan.XX, scan.YY, scan.Path) == true)
                                {
                                    if (scan.Name.Contains("Point"))
                                    {
                                        if (!Timer.TimerPoint.IsEnabled)
                                        {
                                            if (scan.Path.Contains("Red"))
                                                pointName = "RedPoint";
                                            else if (scan.Path.Contains("Buddy"))
                                                pointName = "BuddyPoint";
                                            else
                                                pointName = "GuildPoint";
                                            pointSoundLevel = scan.SoundLevel;
                                            posPoint = new int[4];
                                            posPoint[0] = scan.X;
                                            posPoint[1] = scan.Y;
                                            posPoint[2] = scan.XX;
                                            posPoint[3] = scan.YY;
                                            Timer.TimerPoint.Start();
                                        }
                                    }
                                    else
                                    {
                                        flagSearches = true;
                                        return (true, scan);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (scan.Name == "Familiar")
                            {
                                if (AccuracySearch(scan.Path, scan.X, scan.Y, scan.XX, scan.YY, 0.94).X == -1)
                                {
                                    countFamiliar++;
                                    if (countFamiliar >= FamiliarSetting.MaxCountIfFound)
                                    {
                                        //if (FamiliarSetting.StateAutoFuel && AccuracySearch(@".Scan\Familiar2.bmp", scan.X, scan.Y, scan.XX, scan.YY, 0.94))
                                        //{
                                        //    if (countFuelPress >= FamiliarSetting.MaxFuelPress)
                                        //    {
                                        //        Action.DoSthIfFound(mainWindow, scan.Name + " Fuel", scan.SoundLevel, scan.PauseMacro, scan.PauseDelay);
                                        //        countFuelPress = 0;
                                        //    }
                                        //    else
                                        //    {
                                        //        Action.Send(Action.ScanCodeShort.HOME);
                                        //        Thread.Sleep(1000);
                                        //        countFuelPress++;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        countFamiliar = 0;
                                        return (true, scan);
                                        //}
                                    }
                                }
                                else
                                {
                                    countFamiliar = 0;
                                    //countFuelPress = 0;
                                }
                            }
                            else if (scan.Name == "Wrong")
                            {
                                if (NormalSearch(scan.X, scan.Y, scan.XX, scan.YY, scan.Path) == false)
                                {
                                    countMe++;
                                    if (countMe >= ETCSetting.WrongLoop)
                                    {
                                        countMe = 0;
                                        return (true, scan);
                                    }
                                }
                                else
                                    countMe = 0;
                            }
                            else
                            {
                                if (NormalSearch(scan.X, scan.Y, scan.XX, scan.YY, scan.Path) == false)
                                {
                                    return (true, scan);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + e.Message.ToString() + " (" + scan.Name + ") " + Environment.NewLine);
                    Process.GetCurrentProcess().Kill();
                }
            }
            flagSearches = true;
            return (false, null);
        }

        public void Searches(MainWindow mainWindow, List<ScanSetting> getSO)
        {
            //if (flagSearches)
            //{
                //var getResult = await Task.Run(() => Result(getSO));
                var getResult = Result(getSO);
                if (getResult.Item1)
                {
                    var scan = getResult.Item2;
                    if (scan.TimerId == 1)
                    {
                        Timer.Timer1.Stop();
                        Timer.TimerPoint.Stop();
                    }
                    else
                    {
                        Timer.Timer1.Stop();
                        Timer.Timer2.Stop();
                        Timer.TimerPoint.Stop();
                        Timer.TimerCurrentMeso.Stop();
                    }
                    Action.DoSthIfFound(mainWindow, scan.Name, scan.SoundLevel, scan.PauseMacro, scan.PauseDelay);
                    if (scan.Name.Contains("Code"))
                    {
                        var resultAS = AccuracySearch(@".Scan\Code3.bmp", scan.X, scan.Y, scan.XX, scan.YY, Double.Parse(scan.ExRatio));
                        Action.SetCursorPos(Convert.ToInt32(resultAS.X), Convert.ToInt32(resultAS.Y) - 5);
                        Action.LeftClick();
                    }
                }
            //}
        }

        public static bool flagSearches = true;
        static int[] posPoint;
        int countMe = 0;
        int countFamiliar = 0;
        static string pointName = "";
        static int pointSoundLevel;
        //int countFuelPress = 0;

        #region AccuraySearch

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        public static System.Windows.Point AccuracySearch(string imagePath, int x, int y, int xx, int yy, double accuracy = 0.96)
        {
            Bitmap bitmap = new Bitmap(xx - x, yy - y);

            // Draw the screenshot into our bitmap.
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, bitmap.Size);
            }

            Image<Bgr, byte> source = new Image<Bgr, byte>(bitmap);

            bitmap.Dispose();

            Image<Bgr, byte> template = new Image<Bgr, byte>(imagePath);

            using (Image<Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                System.Drawing.Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                if (maxValues[0] > accuracy)
                {
                    return new System.Windows.Point(maxLocations[0].X + (template.Size.Width / 2), maxLocations[0].Y + (template.Size.Height) / 2); //lấy ra toạ độ giữa ảnh
                }
                else
                {
                    return new System.Windows.Point(-1,-1);
                }
            }
        }
        #endregion

        #region Mob_Check
        private int[][][] pos;
        private int colSize;
        private int row;

        public void MobCheckSetup()
        {
            MobCountSetting.Load();
            SetColSize();
            SetRowSize();
        }

        private void SetColSize() //input số cột
        {
            colSize = Convert.ToInt32(Math.Round((double)(MobCountSetting.Width / MobCountSetting.Col)));
        }

        private void SetRowSize()
        {
            char[] separator = new char[] { '|' };
            string[] str = MobCountSetting.Row.Split(separator);

            row = str.Length - 1;

            pos = new int[row][][];

            for (int i = 0; i < row; i++)
            {
                pos[i] = new int[MobCountSetting.Col][];

                for (int j = 0; j < MobCountSetting.Col; j++)
                {
                    pos[i][j] = new int[4];
                }
            }

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < MobCountSetting.Col; j++)
                {
                    if (j == 0)
                    {
                        pos[i][j][0] = 0;
                        pos[i][j][1] = Convert.ToInt32(str[i]);
                        pos[i][j][2] = colSize;
                        pos[i][j][3] = Convert.ToInt32(str[i + 1]);
                    }

                    if (j != 0)
                    {
                        pos[i][j][0] = pos[i][j - 1][2];
                        pos[i][j][1] = Convert.ToInt32(str[i]);
                        pos[i][j][2] = pos[i][j - 1][2] + colSize;
                        pos[i][j][3] = Convert.ToInt32(str[i + 1]);
                    }
                }
            }
        }

        private int MobCheckSearch(string imgPath1, string imgPath2)
        {
            int count = 0;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < MobCountSetting.Col; j++)
                {
                    string str = Marshal.PtrToStringAnsi(ImageSearch(pos[i][j][0], pos[i][j][1], pos[i][j][2], pos[i][j][3], imgPath1));
                    if (str[0] == '0')
                    {
                    }
                    else
                    {
                        count++;
                        if (count >= MobCountSetting.Count)
                            return count;
                    }
                }
            }

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < MobCountSetting.Col; j++)
                {
                    string str = Marshal.PtrToStringAnsi(ImageSearch(pos[i][j][0], pos[i][j][1], pos[i][j][2], pos[i][j][3], imgPath2));
                    if (str[0] == '0')
                    {
                    }
                    else
                    {
                        count++;
                        if (count >= MobCountSetting.Count)
                            return count;
                    }
                }
            }
            return count;
        }


        public static bool flagATK = true;
        public static int countCurrentMeso = 0;
        public void MobCheck(MainWindow mainWindow)
        {
            int result = MobCheckSearch(@".Scan\Mob1.bmp", @".Scan\Mob2.bmp");
            if ((result >= MobCountSetting.Count) && flagATK == true)
            {
                mainWindow.SetColor_ATK_Attack();
                flagATK = false;
                Timer.TimerATK.Start();
            }
        }

        public static void CurrenMesoScan()
        {
            //GM Check
            if (NormalSearch(CaptureMesoSetting.splitCMRegionX - 5, CaptureMesoSetting.splitCMRegionY - 5, CaptureMesoSetting.splitCMRegionXX + 5, CaptureMesoSetting.splitCMRegionYY + 5, @".Scan\currentMeso.bmp") == true && mainWindow.btnMain.Content.ToString() == "PAUSE")
            {
                countCurrentMeso++;
                if (countCurrentMeso >= CaptureMesoSetting.Loop)
                {
                    Timer.Timer1.Stop();
                    Timer.Timer2.Stop();
                    Timer.TimerPoint.Stop();
                    Timer.TimerCurrentMeso.Stop();
                    countCurrentMeso = 0;
                    Action.DoSthIfFound(mainWindow, "CurrentMeso", 4, true);
                }
            }
            else
            {
                countCurrentMeso = 0;
            }
            Action.CaptureCurrentMeso();
        }

        #endregion

        public static void ScanPoint()
        {
            Timer.TimerPoint.Stop();
            if (NormalSearch(posPoint[0], posPoint[1], posPoint[2], posPoint[3], @".Scan\" + pointName + ".bmp") == true)
            {
                soundP.Play();
                Action.DoSthIfFound(mainWindow, pointName, pointSoundLevel, false);
                Timer.Timer1.Stop();
            }
            pointName = "";
        }
    }
}
