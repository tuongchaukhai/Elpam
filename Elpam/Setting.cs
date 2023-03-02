using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Elpam
{

    public class ScanSetting
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int XX { get; set; }
        public int YY { get; set; }
        public string ExRatio { get; set; }
        public int TimerId { get; set; }
        public int SoundLevel { get; set; }
        public bool PauseMacro { get; set; }
        public int PauseDelay { get; set; }
        public bool State { get; set; }

        public static void Add(ScanSetting scan)
        {
            if (scan != null)
            {
                if (scan.TimerId == 1)
                    listScan1.Add(scan);
                else
                    listScan2.Add(scan);
            }
        }

        public static List<ScanSetting> listScan1 = new List<ScanSetting>();
        public static List<ScanSetting> listScan2 = new List<ScanSetting>();

        public static void Load()
        {
            try
            {
                listScan1.Clear();
                listScan2.Clear();

                List<ScanSetting> items = JsonConvert.DeserializeObject<List<ScanSetting>>(File.ReadAllText(@".Setting\Scan.json"));

                foreach (var item in items)
                {
                    if (item.TimerId == 1)
                        listScan1.Add(item);
                    if (item.TimerId == 2)
                        listScan2.Add(item);
                }
            }
            catch (Exception e)
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + e.Message.ToString() + Environment.NewLine);
            }
        }

        public static void SaveToFile()
        {
            List<ScanSetting> combineList = new List<ScanSetting>();
            combineList.AddRange(listScan1);
            combineList.AddRange(listScan2);
            var items = JsonConvert.SerializeObject(combineList, Formatting.Indented);
            File.WriteAllText(@".Setting\Scan.json", items);
            combineList.Clear();
        }

        public static void UpdateState(List<ScanSetting> combineList)
        {
            var items = JsonConvert.SerializeObject(combineList, Formatting.Indented);
            File.WriteAllText(@".Setting\Scan.json", items);
            Load();
        }

        public static void Delete(ScanSetting scan)
        {
            if (scan.TimerId == 1)
                listScan1.RemoveAll(x => x.Name == scan.Name && x.Path == scan.Path);
            else
                listScan2.RemoveAll(x => x.Name == scan.Name && x.Path == scan.Path);
        }
    }

    public class TimerSetting
    {
        public static int Timer1Delay { get; set; }
        public static int Timer2Delay { get; set; }
        public static int TimerPointDelay { get; set; }
        public static int TimerATKDelay { get; set; }

        public static int TimerSuperSlowDelay { get; set; }
        public static int TimerSlowDelay { get; set; }
        public static int TimerNormalDelay { get; set; }
        public static int TimerFastDelay { get; set; }

        public int tempTimer1Delay, tempTimer2Delay, tempTimerPointDelay, tempTimerATKDelay, tempTimerSuperSlowDelay, tempTimerSlowDelay, tempTimerNormalDelay, tempTimerFastDelay;

        public static void Load()
        {
            try
            {
                using (StreamReader file = File.OpenText(@".Setting\Timer.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);

                    Timer1Delay = (int)o[nameof(tempTimer1Delay)];
                    Timer2Delay = (int)o[nameof(tempTimer2Delay)];
                    TimerPointDelay = (int)o[nameof(tempTimerPointDelay)];
                    TimerATKDelay = (int)o[nameof(tempTimerATKDelay)];
                    TimerSuperSlowDelay = (int)o[nameof(tempTimerSuperSlowDelay)];
                    TimerSlowDelay = (int)o[nameof(tempTimerSlowDelay)];
                    TimerNormalDelay = (int)o[nameof(tempTimerNormalDelay)];
                    TimerFastDelay = (int)o[nameof(tempTimerFastDelay)];
                }
            }
            catch (Exception e)
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + e.Message.ToString() + Environment.NewLine);
            }
        }

        public void Update(int timer1Delay, int timer2Delay, int timerPointDelay, int timerATKDelay, int timerSuperSlowDelay, int timerSlowDelay, int timerNormalDelay, int timerFastDelay)
        {
            TimerSetting timerSetting = new TimerSetting();
            timerSetting.tempTimer1Delay = timer1Delay;
            timerSetting.tempTimer2Delay = timer2Delay;
            timerSetting.tempTimerPointDelay = timerPointDelay;
            timerSetting.tempTimerATKDelay = timerATKDelay;
            timerSetting.tempTimerSuperSlowDelay = timerSuperSlowDelay;
            timerSetting.tempTimerSlowDelay = timerSlowDelay;
            timerSetting.tempTimerNormalDelay = timerNormalDelay;
            timerSetting.tempTimerFastDelay = timerFastDelay;
            string output = JsonConvert.SerializeObject(timerSetting, Formatting.Indented);
            File.WriteAllText(@".Setting\Timer.json", output);

            Load();
        }
    }

    public class FamiliarSetting
    {
        public static int MaxCountIfFound { get; set; }
        public static bool StateAutoFuel { get; set; }
        public static int MaxFuelPress { get; set; }

        public bool tempStateAutoFuel;
        public int tempMaxCountIfFound, tempMaxFuelPress;

        public static void Load()
        {
            try
            {
                using (StreamReader file = File.OpenText(@".Setting\Familiar.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);

                    MaxCountIfFound = (int)o[nameof(tempMaxCountIfFound)];
                    StateAutoFuel = (bool)o[nameof(tempStateAutoFuel)];
                    MaxFuelPress = (int)o[nameof(tempMaxFuelPress)];
                }
            }
            catch (Exception e)
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + e.Message.ToString() + Environment.NewLine);
            }
        }

        public void Update(int maxCountIfFound, bool stateAutoFuel, int maxFuelPress)
        {
            FamiliarSetting familiarSetting = new FamiliarSetting();
            familiarSetting.tempMaxCountIfFound = maxCountIfFound;
            familiarSetting.tempStateAutoFuel = stateAutoFuel;
            familiarSetting.tempMaxFuelPress = maxFuelPress;
          
            string output = JsonConvert.SerializeObject(familiarSetting, Formatting.Indented);
            File.WriteAllText(@".Setting\Familiar.json", output);

            Load();
        }
    }

    public class CaptureMesoSetting
    {
        public static bool State { get; set; }
        public static string Region { get; set; }
        public static int Timer { get; set; }
        public static int Loop { get; set; }

        public bool tempState;
        public string tempRegion;
        public int tempTimer, tempLoop;

        public static int splitCMRegionX, splitCMRegionY, splitCMRegionXX, splitCMRegionYY;
        public static void Load()
        {
            try
            {
                using (StreamReader file = File.OpenText(@".Setting\CaptureMeso.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);

                    State = (bool)o[nameof(tempState)];
                    Region = (string)o[nameof(tempRegion)];
                    Timer = (int)o[nameof(tempTimer)];
                    Loop = (int)o[nameof(tempLoop)];

                    string[] splitRegion = Region.Split(new char[] { ' ' }).ToArray();
                    splitCMRegionX = Int32.Parse(splitRegion[0]);
                    splitCMRegionY = Int32.Parse(splitRegion[1]);
                    splitCMRegionXX = Int32.Parse(splitRegion[2]);
                    splitCMRegionYY = Int32.Parse(splitRegion[3]);
                }
                
            }
            catch (Exception e)
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + e.Message.ToString() + Environment.NewLine);
            }
        }

        public void Update(bool state, int regionX, int regionY, int regionXX, int regionYY, int timer, int loop)
        {
        
            CaptureMesoSetting captureMesoSetting = new CaptureMesoSetting();
            captureMesoSetting.tempState = state;
            captureMesoSetting.tempRegion = regionX.ToString() + " " + regionY.ToString() + " " + regionXX.ToString() + " " + regionYY.ToString();
            captureMesoSetting.tempTimer = timer;
            captureMesoSetting.tempLoop = loop;

            string output = JsonConvert.SerializeObject(captureMesoSetting, Formatting.Indented);
            File.WriteAllText(@".Setting\CaptureMeso.json", output);

            Load();
        }
    }

    public class MobCountSetting
    {
        public static int Width { get; set; }
        public static int Col { get; set; }
        public static int Count { get; set; }
        public static string Row { get; set; }

        public int tempWidth, tempCol, tempCount;
        public string tempRow;

        public static void Load()
        {
            try
            {
                using (StreamReader file = File.OpenText(@".Setting\MobCount.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);

                    Width = (int)o[nameof(tempWidth)];
                    Col = (int)o[nameof(tempCol)];
                    Count = (int)o[nameof(tempCount)];
                    Row = (string)o[nameof(tempRow)];
                }

            }
            catch (Exception e)
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + e.Message.ToString() + Environment.NewLine);
            }
        }

        public void Update(int width, int col, int count, string row)
        {
            MobCountSetting mobCountSetting = new MobCountSetting();
            mobCountSetting.tempWidth = width;
            mobCountSetting.tempCol = col;
            mobCountSetting.tempCount = count;
            mobCountSetting.tempRow = row;

            string output = JsonConvert.SerializeObject(mobCountSetting, Formatting.Indented);
            File.WriteAllText(@".Setting\MobCount.json", output);

            Load();
        }
    }

    public class RandomNumberSetting
    {
        public static int Size { get; set; }
        public static int Loop { get; set; }

        public int tempSize, tempLoop;

        public static void Load()
        {
            try
            {
                using (StreamReader file = File.OpenText(@".Setting\RandomNumber.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);

                    Size = (int)o[nameof(tempSize)];
                    Loop = (int)o[nameof(tempLoop)];
                }

            }
            catch (Exception e)
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + e.Message.ToString() + Environment.NewLine);
            }
        }

        public void Update(int size, int loop)
        {
            RandomNumberSetting randomNumberSetting = new RandomNumberSetting();
            randomNumberSetting.tempSize = size;
            randomNumberSetting.tempLoop = loop;

            string output = JsonConvert.SerializeObject(randomNumberSetting, Formatting.Indented);
            File.WriteAllText(@".Setting\RandomNumber.json", output);

            Load();
        }
    }

    public class ETCSetting
    {
        public static int WrongLoop { get; set; }
        public static string EndTaskTitle { get; set; }
        public static int EndTaskMaxPress { get; set; }
        public static bool StateTopmost { get; set; }
        public static string LeftTop { get; set; }

        public int tempWrongLoop, tempEndTaskMaxPress;
        public string tempEndtaskTitle, tempLeftTop;
        public bool tempStateTopmost;

        public static int topWin, leftWin;
        public static void Load()
        {
            try
            {
                using (StreamReader file = File.OpenText(@".Setting\ETCSetting.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);

                    WrongLoop = (int)o[nameof(tempWrongLoop)];
                    EndTaskTitle = (string)o[nameof(tempEndtaskTitle)];
                    EndTaskMaxPress = (int)o[nameof(tempEndTaskMaxPress)];
                    StateTopmost = (bool)o[nameof(tempStateTopmost)];
                    LeftTop = (string)o[nameof(tempLeftTop)];

                    string[] splitLeftTop = LeftTop.Split(new char[] { ',' }).ToArray();
                    leftWin = Int32.Parse(splitLeftTop[0]);
                    topWin = Int32.Parse(splitLeftTop[1]);
                }

            }
            catch (Exception e)
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + e.Message.ToString() + Environment.NewLine);
            }
        }

        public void Update(int wrongLoop, string endTaskTitle, int endTaskMaxPress, bool stateTopmost, int left, int top)
        {
            ETCSetting eTCSetting = new ETCSetting();
            eTCSetting.tempWrongLoop = wrongLoop;
            eTCSetting.tempEndtaskTitle = endTaskTitle;
            eTCSetting.tempEndTaskMaxPress = endTaskMaxPress;
            eTCSetting.tempStateTopmost = stateTopmost;
            eTCSetting.tempLeftTop = left + "," + top;

            string output = JsonConvert.SerializeObject(eTCSetting, Formatting.Indented);
            File.WriteAllText(@".Setting\ETCSetting.json", output);

            Load();
        }
    }
}
