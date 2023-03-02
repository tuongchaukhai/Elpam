using FireSharp.Config;
using FireSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AK
{
    public class SetAUTH
    {
        private bool status;
        private bool receive;
        private bool authen;
        private bool roleReceive;
        private string version;

        public bool Status { get => status; set => status = value; }
        public bool Receive { get => receive; set => receive = value; }
        public bool Authen { get => authen; set => authen = value; }
        public bool RoleReceive { get => roleReceive; set => roleReceive = value; }
        public string Version { get => version; set => version = value; }
    }

    public class User
    {
        private string pcName;
        private string videoId;
        private string connectTime;
        private bool role;


        public string PcName { get => pcName; set => pcName = value; }
        public string VideoId { get => videoId; set => videoId = value; }
        public string ConnectTime { get => connectTime; set => connectTime = value; }
        public bool Role { get => role; set => role = value; }
    }

    internal class AUTH
    {
        public static string userName;
        public static string version = "3";
        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "kMbllwroWmxC467Hk6agigmPzGQ7G7gi5hDCXCyo",
            BasePath = "https://authelpam-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        static IFirebaseClient client;

        public AUTH()
        {
            client = new FireSharp.FirebaseClient(ifc);
        }



        public static void FirstAuthen()
        {
            string hwid = Value();

            var resUsers = client.Get(@"USERS");
            Dictionary<string, User> getUsers = resUsers.ResultAs<Dictionary<string, User>>();
            foreach (var user in getUsers)
            {
                var hwids = client.Get(@"USERS/" + user.Key);
                Dictionary<string, User> getHwid = hwids.ResultAs<Dictionary<string, User>>();
                foreach (var id in getHwid)
                {
                    if (id.Key == hwid) //hwid exist
                    {
                        userName = user.Key; //input the username to field

                        if (id.Value.Role)
                        {
                            //update time
                            var set = client.SetAsync(@"USERS/" + userName + "/" + hwid + "/ConnectTime", DateTime.Now.ToString());
                            return;
                        }
                        else
                        {
                            File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " role" + Environment.NewLine);
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                }
            }
            File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " status" + Environment.NewLine);
            Process.GetCurrentProcess().Kill();
        }

        #region GetHWID
        static string fingerPrint = string.Empty;

        public static string Value()
        {
            if (string.IsNullOrEmpty(fingerPrint))
            {
                fingerPrint = GetHash(cpuId() + biosId() + baseId() + diskId() + videoId() + macId());
            }
            return fingerPrint;
        }
        private static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt)).Replace(" ", "");
        }
        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += " - ";
            }
            return s;
        }

        #region Original Device ID Getting Code

        private static string identifier
        (string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }

        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (result == "")
                {
                    try
                    {
                        if (mo[wmiProperty] != null)
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }

        private static string cpuId()
        {

            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "")
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "")
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "")
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }

        private static string biosId()
        {
            return identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version");
        }

        private static string diskId()
        {
            return identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads");
        }

        private static string baseId()
        {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }

        public static string videoId()
        {
            return identifier("Win32_VideoController", "DriverVersion")
            + identifier("Win32_VideoController", "Name");
        }

        private static string macId()
        {
            return identifier("Win32_NetworkAdapterConfiguration",
                "MACAddress", "IPEnabled");
        }
        #endregion

        #endregion
    }
}
