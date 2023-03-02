using AutoHotkey.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxOptions = System.Windows.Forms.MessageBoxOptions;

namespace AK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AUTH auth = new AUTH();
        private static DispatcherTimer timerElpamCheck;
        private static DispatcherTimer timerNormal;
        SoundPlayer soundP = new SoundPlayer(@".Music\music.wav");
        
        public MainWindow()
        {
            InitializeComponent();
            timerElpamCheck = new DispatcherTimer();
            timerElpamCheck.Tick += TimerElpamCheck_Tick;
            timerElpamCheck.Interval = new TimeSpan(0, 0, 0, 0, 20000);
            timerElpamCheck.Start();

            timerNormal = new DispatcherTimer();
            timerNormal.Tick += TimerNormal_Tick;
            timerNormal.Interval = new TimeSpan(0, 0, 0, 0, 3000);
        }

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

        private void TimerNormal_Tick(object sender, EventArgs e)
        {
            soundP.Play();
        }

        private async void TimerElpamCheck_Tick(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("Elpam");
            if (processes.Length > 0)
            {
                Process lol = processes[0];
                IntPtr ptr = lol.MainWindowHandle;
                Rect rect = new Rect();
                GetWindowRect(ptr, ref rect);
                if (NormalSearch(35, 5, 85, 25, @".Scan\AKPlaying.bmp"))
                {
                    if (!NormalSearch(rect.Left, rect.Top, rect.Right, rect.Bottom, @".Scan\ElpamStart.bmp"))
                        soundP.Play();
                }
            }
            else
            {
                timerNormal.Start();
                Process[] processRunning = Process.GetProcesses();
                foreach (Process pr in processRunning)
                {
                    if (pr.ProcessName == "AK")
                    {

                        ShowWindow(pr.MainWindowHandle, 5);
                        SetForegroundWindow(pr.MainWindowHandle); //set to topmost
                    }
                }
                await Task.Run(() =>
                {
                    MessageBox.Show("Elpam isn't existing", "Error!!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Process.GetCurrentProcess().Kill();
                });
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }


        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); //ShowWindow needs an IntPtr

        static string scriptAHK = "";

        public static void CreateAK()
        {
            //AKSetting.Load();

            GetkId();

            scriptAHK = "" +
                "try {\n"
+ "CLR_LoadLibrary(AssemblyName, AppDomain=0) {\n"
+ "	if !AppDomain\n"
+ "		AppDomain := CLR_GetDefaultDomain()\n"
+ "	e := ComObjError(0)\n"
+ "	Loop 1 {\n"
+ "		if assembly := AppDomain.Load_2(AssemblyName)\n"
+ "			break\n"
+ "		static _null := ComObject(13,0)\n"
+ "		args := ComObjArray(0xC, 1),  args[0] := AssemblyName\n"
+ "		typeofAssembly := AppDomain.GetType().Assembly.GetType()\n"
+ "		if assembly := typeofAssembly.InvokeMember_3(\"LoadWithPartialName\", 0x158, _null, _null, args)\n"
+ "			break\n"
+ "		if assembly := typeofAssembly.InvokeMember_3(\"LoadFrom\", 0x158, _null, _null, args)\n"
+ "			break\n"
+ "	}\n"
+ "	ComObjError(e)\n"
+ "	return assembly\n"
+ "}\n"

+ "CLR_CreateObject(Assembly, TypeName, Args*) {\n"
+ "	if !(argCount := Args.MaxIndex())\n"
+ "		return Assembly.CreateInstance_2(TypeName, true)\n"

+ "	vargs := ComObjArray(0xC, argCount)\n"
+ "	Loop % argCount\n"
+ "		vargs[A_Index-1] := Args[A_Index]\n"

+ "	static Array_Empty := ComObjArray(0xC,0), _null := ComObject(13,0)\n"

+ "	return Assembly.CreateInstance_3(TypeName, true, 0, _null, vargs, _null, Array_Empty)\n"
+ "}\n"

+ "CLR_CompileC#(Code, References=\"\", AppDomain=0, FileName=\"\", CompilerOptions=\"\") {\n"
+ "	return CLR_CompileAssembly(Code, References, \"System\", \"Microsoft.CSharp.CSharpCodeProvider\", AppDomain, FileName, CompilerOptions)\n"
+ "}\n"

+ "CLR_CompileVB(Code, References=\"\", AppDomain=0, FileName=\"\", CompilerOptions=\"\") {\n"
+ "	return CLR_CompileAssembly(Code, References, \"System\", \"Microsoft.VisualBasic.VBCodeProvider\", AppDomain, FileName, CompilerOptions)\n"
+ "}\n"

+ "CLR_StartDomain(ByRef AppDomain, BaseDirectory=\"\") {\n"
+ "	static _null := ComObject(13,0)\n"
+ "	args := ComObjArray(0xC, 5), args[0] := \"\", args[2] := BaseDirectory, args[4] := ComObject(0xB,false)\n"
+ "	AppDomain := CLR_GetDefaultDomain().GetType().InvokeMember_3(\"CreateDomain\", 0x158, _null, _null, args)\n"
+ "	return A_LastError >= 0\n"
+ "}\n"

+ "CLR_StopDomain(ByRef AppDomain) {\n"
+ "	; ICorRuntimeHost::UnloadDomain\n"
+ "	DllCall(\"SetLastError\", \"uint\", hr := DllCall(NumGet(NumGet(0+RtHst:=CLR_Start())+20*A_PtrSize), \"ptr\", RtHst, \"ptr\", ComObjValue(AppDomain))), AppDomain := \"\"\n"
+ "	return hr >= 0\n"
+ "}\n"

+ "; NOTE: IT IS NOT NECESSARY TO CALL THIS FUNCTION unless you need to load a specific version.\n"
+ "CLR_Start(Version=\"\") {\n"
+ "	; returns ICorRuntimeHost*\n"
+ "	static RtHst := 0\n"
+ "	; The simple method gives no control over versioning, and seems to load .NET v2 even when v4 is present:\n"
+ "	; return RtHst ? RtHst : (RtHst:=COM_CreateObject(\"CLRMetaData.CorRuntimeHost\",\"{CB2F6722-AB3A-11D2-9C40-00C04FA30A3E}\"), DllCall(NumGet(NumGet(RtHst+0)+40),\"uint\",RtHst))\n"
+ "	if RtHst\n"
+ "		return RtHst\n"
+ "	EnvGet SystemRoot, SystemRoot\n"
+ "	if Version =\n"
+ "		Loop % SystemRoot \"\\Microsoft.NET\\Framework\" (A_PtrSize=8?\"64\":\"\") \"\\*\", 2\n"
+ "			if (FileExist(A_LoopFileFullPath \"\\mscorlib.dll\") && A_LoopFileName > Version)\n"
+ "				Version := A_LoopFileName\n"
+ "	if DllCall(\"mscoree\\CorBindToRuntimeEx\", \"wstr\", Version, \"ptr\", 0, \"uint\", 0\n"
+ "	, \"ptr\", CLR_GUID(CLSID_CorRuntimeHost, \"{CB2F6723-AB3A-11D2-9C40-00C04FA30A3E}\")\n"
+ "	, \"ptr\", CLR_GUID(IID_ICorRuntimeHost,  \"{CB2F6722-AB3A-11D2-9C40-00C04FA30A3E}\")\n"
+ "	, \"ptr*\", RtHst) >= 0\n"
+ "		DllCall(NumGet(NumGet(RtHst+0)+10*A_PtrSize), \"ptr\", RtHst) ; Start\n"
+ "	return RtHst\n"
+ "}\n"

+ ";\n"
+ "; INTERNAL FUNCTIONS\n"
+ ";\n"

+ "CLR_GetDefaultDomain() {\n"
+ "	static defaultDomain := 0\n"
+ "	if !defaultDomain {\n"
+ "		; ICorRuntimeHost::GetDefaultDomain\n"
+ "		if DllCall(NumGet(NumGet(0+RtHst:=CLR_Start())+13*A_PtrSize), \"ptr\", RtHst, \"ptr*\", p:=0) >= 0\n"
+ "			defaultDomain := ComObject(p), ObjRelease(p)\n"
+ "	}\n"
+ "	return defaultDomain\n"
+ "}\n"

+ "CLR_CompileAssembly(Code, References, ProviderAssembly, ProviderType, AppDomain=0, FileName=\"\", CompilerOptions=\"\") {\n"
+ "	if !AppDomain\n"
+ "		AppDomain := CLR_GetDefaultDomain()\n"

+ "	if !(asmProvider := CLR_LoadLibrary(ProviderAssembly, AppDomain))\n"
+ "	|| !(codeProvider := asmProvider.CreateInstance(ProviderType))\n"
+ "	|| !(codeCompiler := codeProvider.CreateCompiler())\n"
+ "		return 0\n"

+ "	if !(asmSystem := (ProviderAssembly=\"System\") ? asmProvider : CLR_LoadLibrary(\"System\", AppDomain))\n"
+ "		return 0\n"

+ "	; Convert | delimited list of references into an array.\n"
+ "	StringSplit, Refs, References, |, %A_Space%%A_Tab%\n"
+ "	aRefs := ComObjArray(8, Refs0)\n"
+ "	Loop % Refs0\n"
+ "		aRefs[A_Index-1] := Refs%A_Index%\n"

+ "	; Set parameters for compiler.\n"
+ "	prms := CLR_CreateObject(asmSystem, \"System.CodeDom.Compiler.CompilerParameters\", aRefs)\n"
+ "	, prms.OutputAssembly          := FileName\n"
+ "	, prms.GenerateInMemory        := FileName=\"\"\n"
+ "	, prms.GenerateExecutable      := SubStr(FileName,-3)=\".exe\"\n"
+ "	, prms.CompilerOptions         := CompilerOptions\n"
+ "	, prms.IncludeDebugInformation := true\n"

+ "	; Compile!\n"
+ "	compilerRes := codeCompiler.CompileAssemblyFromSource(prms, Code)\n"

+ "	if error_count := (errors := compilerRes.Errors).Count {\n"
+ "		error_text := \"\"\n"
+ "		Loop % error_count\n"
+ "			error_text .= ((e := errors.Item[A_Index-1]).IsWarning ? \"Warning \" : \"Error \") . e.ErrorNumber \" on line \" e.Line \": \" e.ErrorText \"`n`n\"\n"
+ "		MsgBox, 16, Compilation Failed, %error_text%\n"
+ "		return 0\n"
+ "	}\n"
+ "	; Success. Return Assembly object or path.\n"
+ "	return compilerRes[FileName=\"\" ? \"CompiledAssembly\" : \"PathToAssembly\"]\n"
+ "}\n"

+ "CLR_GUID(ByRef GUID, sGUID) {\n"
+ "	VarSetCapacity(GUID, 16, 0)\n"
+ "	return DllCall(\"ole32\\CLSIDFromString\", \"wstr\", sGUID, \"ptr\", &GUID) >= 0 ? &GUID : \"\"\n"
+ "}\n"

+ "class AutoHotInterception {\n"
+ "	_contextManagers := {}\n"

+ "	__New() {\n"
+ "		bitness := A_PtrSize == 8 ? \"x64\" : \"x86\"\n"
+ "		dllName := \"interception.dll\"\n"
+ "		if (A_IsCompiled){\n"
+ "			dllFile := dllName\n"
+ "			FileInstall, AutoHotInterception.dll, AutoHotInterception.dll\n"
+ "			if (bitness == \"x86\"){\n"
+ "				FileInstall, interception.dll, interception.dll\n"
+ "			} else {\n"
+ "				FileInstall, interception.dll, interception.dll\n"
+ "			}\n"
+ "		} else {\n"
+ "			dllFile := dllName\n"
+ "		}\n"
+ "		if (!FileExist(dllFile)) {\n"
+ "			MsgBox % \"Unable to find \" dllFile \", exiting...`nYou should extract both x86 and x64 folders from the library folder in interception.zip into AHI's lib folder.\"\n"
+ "			ExitApp\n"
+ "		}\n"

+ "		hModule := DllCall(\"LoadLibrary\", \"Str\", dllFile, \"Ptr\")\n"
+ "		if (hModule == 0) {\n"
+ "			this_bitness := A_PtrSize == 8 ? \"64-bit\" : \"32-bit\"\n"
+ "			other_bitness := A_PtrSize == 4 ? \"64-bit\" : \"32-bit\"\n"
+ "			MsgBox % \"Bitness of \" dllName \" does not match bitness of AHK.`nAHK is \" this_bitness \", but \" dllName \" is \" other_bitness \".\"\n"
+ "			ExitApp\n"
+ "		}\n"
+ "		DllCall(\"FreeLibrary\", \"Ptr\", hModule)\n"

+ "		dllName := \"AutoHotInterception.dll\"\n"
+ "		if (A_IsCompiled){\n"
+ "			dllFile := dllName\n"
+ "		} else {\n"
+ "			dllFile := dllName\n"
+ "		}\n"
+ "		hintMessage := \"Try right-clicking \" dllFile \", select Properties, and if there is an 'Unblock' checkbox, tick it`nAlternatively, running Unblocker.ps1 in the lib folder (ideally as admin) can do this for you.\"\n"
+ "		if (!FileExist(dllFile)) {\n"
+ "			MsgBox % \"Unable to find \" dllFile \", exiting...\"\n"
+ "			ExitApp\n"
+ "		}\n"

+ "		asm := CLR_LoadLibrary(dllFile)\n"
+ "		try {\n"
+ "			this.Instance := asm.CreateInstance(\"AutoHotInterception.Manager\")\n"
+ "		}\n"
+ "		catch {\n"
+ "			MsgBox % dllName \" failed to load`n`n\" hintMessage\n"
+ "			ExitApp\n"
+ "		}\n"
+ "		if (this.Instance.OkCheck() != \"OK\") {\n"
+ "			MsgBox % dllName \" loaded but check failed!`n`n\" hintMessage\n"
+ "			ExitApp\n"
+ "		}\n"
+ "	}\n"

+ "	GetInstance() {\n"
+ "		return this.Instance\n"
+ "	}\n"

+ "	; --------------- Input Synthesis ----------------\n"
+ "	SendKeyEvent(id, code, state) {\n"
+ "		this.Instance.SendKeyEvent(id, code, state)\n"
+ "	}\n"

+ "	SendMouseButtonEvent(id, btn, state) {\n"
+ "		this.Instance.SendMouseButtonEvent(id, btn, state)\n"
+ "	}\n"

+ "	SendMouseButtonEventAbsolute(id, btn, state, x, y) {\n"
+ "		this.Instance.SendMouseButtonEventAbsolute(id, btn, state, x, y)\n"
+ "	}\n"

+ "	SendMouseMove(id, x, y) {\n"
+ "		this.Instance.SendMouseMove(id, x, y)\n"
+ "	}\n"

+ "	SendMouseMoveRelative(id, x, y) {\n"
+ "		this.Instance.SendMouseMoveRelative(id, x, y)\n"
+ "	}\n"

+ "	SendMouseMoveAbsolute(id, x, y) {\n"
+ "		this.Instance.SendMouseMoveAbsolute(id, x, y)\n"
+ "	}\n"

+ "	SetState(state){\n"
+ "		this.Instance.SetState(state)\n"
+ "	}\n"
+ "	\n"
+ "	MoveCursor(x, y, cm := \"Screen\", mouseId := -1){\n"
+ "		if (mouseId == -1)\n"
+ "			mouseId := 11 ; Use 1st found mouse\n"
+ "		oldMode := A_CoordModeMouse\n"
+ "		CoordMode, Mouse, % cm\n"
+ "		Loop {\n"
+ "			MouseGetPos, cx, cy\n"
+ "			dx := this.GetDirection(cx, x)\n"
+ "			dy := this.GetDirection(cy, y)\n"
+ "			if (dx == 0 && dy == 0)\n"
+ "				break\n"
+ "			this.SendMouseMove(mouseId, dx, dy)\n"
+ "		}\n"
+ "		CoordMode, Mouse, % oldMode\n"
+ "	}\n"
+ "	\n"
+ "	GetDirection(cp, dp){\n"
+ "		d := dp - cp\n"
+ "		if (d > 0)\n"
+ "			return 1\n"
+ "		if (d < 0)\n"
+ "			return -1\n"
+ "		return 0\n"
+ "	}\n"

+ "	; --------------- Querying ------------------------\n"
+ "	GetDeviceId(IsMouse, VID, PID, instance := 1) {\n"
+ "		static devType := {0: \"Keyboard\", 1: \"Mouse\"}\n"
+ "		dev := this.Instance.GetDeviceId(IsMouse, VID, PID, instance)\n"
+ "		if (dev == 0) {\n"
+ "			MsgBox % \"Could not get \" devType[isMouse] \" with VID \" VID \", PID \" PID \", Instance \" instance\n"
+ "			ExitApp\n"
+ "		}\n"
+ "		return dev\n"
+ "	}\n"

+ "	GetDeviceIdFromHandle(isMouse, handle, instance := 1) {\n"
+ "		static devType := {0: \"Keyboard\", 1: \"Mouse\"}\n"
+ "		dev := this.Instance.GetDeviceIdFromHandle(IsMouse, handle, instance)\n"
+ "		if (dev == 0) {\n"
+ "			MsgBox % \"Could not get \" devType[isMouse] \" with Handle \" handle \", Instance \" instance\n"
+ "			ExitApp\n"
+ "		}\n"
+ "		return dev\n"
+ "	}\n"

+ "	GetKeyboardId(VID, PID, instance := 1) {\n"
+ "		return this.GetDeviceId(false, VID, PID, instance)\n"
+ "	}\n"

+ "	GetMouseId(VID, PID, instance := 1) {\n"
+ "		return this.GetDeviceId(true, VID, PID, instance)\n"
+ "	}\n"

+ "	GetKeyboardIdFromHandle(handle, instance := 1) {\n"
+ "		return this.GetDeviceIdFromHandle(false, handle, instance)\n"
+ "	}\n"

+ "	GetMouseIdFromHandle(handle, instance := 1) {\n"
+ "		return this.GetDeviceIdFromHandle(true, handle, instance)\n"
+ "	}\n"

+ "	GetDeviceList() {\n"
+ "		DeviceList := {}\n"
+ "		arr := this.Instance.GetDeviceList()\n"
+ "		for v in arr {\n"
+ "			DeviceList[v.id] := { ID: v.id, VID: v.vid, PID: v.pid, IsMouse: v.IsMouse, Handle: v.Handle }\n"
+ "		}\n"
+ "		return DeviceList\n"
+ "	}\n"

+ "	; ---------------------- Subscription Mode ----------------------\n"
+ "	SubscribeKey(id, code, block, callback, concurrent := false) {\n"
+ "		this.Instance.SubscribeKey(id, code, block, callback, concurrent)\n"
+ "	}\n"

+ "	UnsubscribeKey(id, code){\n"
+ "		this.Instance.UnsubscribeKey(id, code)\n"
+ "	}\n"

+ "	SubscribeKeyboard(id, block, callback, concurrent := false) {\n"
+ "		this.Instance.SubscribeKeyboard(id, block, callback, concurrent)\n"
+ "	}\n"
+ "	\n"
+ "	UnsubscribeKeyboard(id){\n"
+ "		this.Instance.UnsubscribeKeyboard(id)\n"
+ "	}\n"

+ "	SubscribeMouseButton(id, btn, block, callback, concurrent := false) {\n"
+ "		this.Instance.SubscribeMouseButton(id, btn, block, callback, concurrent)\n"
+ "	}\n"

+ "	UnsubscribeMouseButton(id, btn){\n"
+ "		this.Instance.UnsubscribeMouseButton(id, btn)\n"
+ "	}\n"

+ "	SubscribeMouseButtons(id, block, callback, concurrent := false) {\n"
+ "		this.Instance.SubscribeMouseButtons(id, block, callback, concurrent)\n"
+ "	}\n"
+ "	\n"
+ "	UnsubscribeMouseButtons(id){\n"
+ "		this.Instance.UnsubscribeMouseButtons(id)\n"
+ "	}\n"

+ "	SubscribeMouseMove(id, block, callback, concurrent := false) {\n"
+ "		this.Instance.SubscribeMouseMove(id, block, callback, concurrent)\n"
+ "	}\n"

+ "	UnsubscribeMouseMove(id){\n"
+ "		this.Instance.UnsubscribeMouseMove(id)\n"
+ "	}\n"

+ "	SubscribeMouseMoveRelative(id, block, callback, concurrent := false) {\n"
+ "		this.Instance.SubscribeMouseMoveRelative(id, block, callback, concurrent)\n"
+ "	}\n"

+ "	UnsubscribeMouseMoveRelative(id){\n"
+ "		this.Instance.UnsubscribeMouseMoveRelative(id)\n"
+ "	}\n"

+ "	SubscribeMouseMoveAbsolute(id, block, callback, concurrent := false) {\n"
+ "		this.Instance.SubscribeMouseMoveAbsolute(id, block, callback, concurrent)\n"
+ "	}\n"

+ "	UnsubscribeMouseMoveAbsolute(id){\n"
+ "		this.Instance.UnsubscribeMouseMoveAbsolute(id)\n"
+ "	}\n"

+ "	; ------------- Context Mode ----------------\n"
+ "	; Creates a context class to make it easy to turn on/off the hotkeys\n"
+ "	CreateContextManager(id) {\n"
+ "		if (this._contextManagers.HasKey(id)) {\n"
+ "			Msgbox % \"ID \" id \" already has a Context Manager\"\n"
+ "			ExitApp\n"
+ "		}\n"
+ "		cm := new this.ContextManager(this, id)\n"
+ "		this._contextManagers[id] := cm\n"
+ "		return cm\n"
+ "	}\n"

+ "	RemoveContextManager(id) {\n"
+ "		if (!this._contextManagers.HasKey(id)) {\n"
+ "			Msgbox % \"ID \" id \" does not have a Context Manager\"\n"
+ "			ExitApp\n"
+ "		}\n"
+ "		this._contextManagers[id].Remove()\n"
+ "		this._contextManagers.Delete(id)\n"
+ "		return cm\n"
+ "	}\n"

+ "	; Helper class for dealing with context mode\n"
+ "	class ContextManager {\n"
+ "		IsActive := 0\n"
+ "		__New(parent, id) {\n"
+ "			this.parent := parent\n"
+ "			this.id := id\n"
+ "			result := this.parent.Instance.SetContextCallback(id, this.OnContextCallback.Bind(this))\n"
+ "		}\n"
+ "		\n"
+ "		OnContextCallback(state) {\n"
+ "			Sleep 0\n"
+ "			this.IsActive := state\n"
+ "		}\n"
+ "		\n"
+ "		Remove(){\n"
+ "			this.parent.Instance.RemoveContextCallback(this.id)\n"
+ "		}\n"
+ "	}\n"
+ "}\n"
+ "AHI := new AutoHotInterception()\n"
            + "keyboardId:= AHI.GetKeyboardId(" + keyBoardId + ")\n"
            + "global AHI, keyboardId\n" +
            "CoordMode Pixel\n";
        }

        public static void GetkId()
        {
            string getId = "";
            try
            {
                SelectQuery Sq = new SelectQuery("Win32_Keyboard");
                ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
                ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
                listV.Clear();
                listP.Clear();

                foreach (ManagementObject mo in osDetailsCollection)
                {
                    string deviceId = (string)mo["DeviceID"];
                    if (deviceId.Contains("\\VID_"))
                    {
                        int vidIndex = deviceId.IndexOf("VID_");
                        string startingAtVid = deviceId.Substring(vidIndex + 4); // + 4 to remove "VID_"                    
                        string vid = "0x" + startingAtVid.Substring(0, 4); // vid is four characters long

                        listV.Add(vid);

                        int pidIndex = deviceId.IndexOf("PID_");
                        string startingAtPid = deviceId.Substring(pidIndex + 4); // + 4 to remove "PID_"                    
                        string pid = "0x" + startingAtPid.Substring(0, 4); // pid is four characters long

                        listP.Add(pid);

                    }
                    getId += deviceId + "\n";
                }
                keyBoardId = String.Format("{0}, {1}", listV[0], listP[0]);
            }
            catch
            {
                File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " Error: GetKeyID \n" + getId + Environment.NewLine);
            }
        }

        static List<string> listV = new List<string>();
        static List<string> listP = new List<string>();
        public static string keyBoardId;
        static bool readInfo = true;
        /// <summary>
        /// Sleep add delay
        /// </summary>
        public async void Run()
        {
            CreateAK();
            Read();

            if (readInfo)
                try
                {
                    var ak = AutoHotkeyEngine.Instance;
                    string scriptRAK = ""
                        + scriptAHK

                        + "Loop\n{\n"
                        + converter + "\n}\n"
                        + "return\n" +
                        "F4::\n" +
                        "Suspend On\n" +
                        "WinSetTitle, PLAYING, , PAUSED\n" +
                        "Pause On\n" +
                        "return\n" +
                        "#If (A_IsPaused)\n" +
                        "F4::\n" +
                        "Suspend Off\n" +
                        "Pause Off\n" +
                        "WinSetTitle, PAUSED, , PLAYING\n" +
                        "return\n" +
                        "}\ncatch {\n MsgBox, 0x10, AK, Error!\nExitApp\n}";

                    ak.LoadScript(scriptRAK);
                }
                catch (Exception e)
                {
                    File.AppendAllText("errorlog.txt", DateTime.Now.ToString() + " " + e.Message.ToString() + Environment.NewLine);
                    Process.GetCurrentProcess().Kill();
                }

            await Task.Run(() => AUTH.FirstAuthen());
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        string converter = "";
        int tab = 0;

        public string Tab()
        {
            string tabLine = "";
            for (int i = 0; i < tab; i++)
            {
                tabLine += "\t";
            }
            return tabLine;
        }

        private void MessageError(string line, int currentLineNumber)
        {
            MessageBox.Show("Error!\n" + line + ", line: " + currentLineNumber);
            readInfo = false;
            Process.GetCurrentProcess().Kill();
        }

        public void Read()
        {
            StreamReader reader = new StreamReader(@".AK\.script.mcr");

            string line = "";
            int currentLine = 0;
            string nameImage = "";
            while ((line = reader.ReadLine()) != null)
            {
                currentLine++;
                if (line.StartsWith("MOVE WINDOW"))
                {
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    Process[] processlist = Process.GetProcesses(); foreach (Process proc in processlist)
                    {
                        const short SWP_NOSIZE = 1;
                        const short SWP_NOZORDER = 0X4;
                        const int SWP_SHOWWINDOW = 0x0040;
                        if (!String.IsNullOrEmpty(proc.MainWindowTitle) && proc.MainWindowTitle == strlist[1])
                        {
                            SetWindowPos(proc.MainWindowHandle, 0, Int32.Parse(strlist[4]), Int32.Parse(strlist[5]), Int32.Parse(strlist[6]), Int32.Parse(strlist[7]), SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                            break;
                        }
                    }
                    continue;
                }

                else if (line.StartsWith("MESSAGE BOX"))
                {
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    string newline = Tab() + "MsgBox, 0x40, " + "Message, " + strlist[1] + "\n";
                    converter = converter + newline;
                    continue;
                }

                else if (line.StartsWith("COMMENT"))
                {
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    string newline = Tab() + "; " + strlist[1] + "\n";
                    converter = converter + newline;
                    nameImage = strlist[1];
                    continue;
                }

                else if (line.StartsWith("DELAY"))
                {
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    string newline = Tab() + "Sleep " + strlist[1] + "\n";
                    converter = converter + newline;
                    continue;
                }

                else if (line.StartsWith("REPEAT"))
                {
                    string newline = "";
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    if (strlist[1] != "0")
                    {
                        newline = Tab() + "Loop, " + strlist[1] + "\n" + Tab() + "{\n";
                    }
                    else if (strlist[5] != "0")
                    {
                        newline = Tab() + String.Format("Random, rand, {0}, {1}\n", strlist[5], strlist[6]);
                        newline += Tab() + "Loop, %rand%\n{\n";
                    }
                    else
                        MessageError(line, currentLine);
                    tab++;
                    converter = converter + newline;
                    continue;
                }

                else if (line.StartsWith("IF ITERATION EQUALS"))
                {
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    string newline = Tab() + String.Format("if (A_Index >= {0})\n", strlist[1]);
                    newline += Tab() + "{\n";
                    tab++;
                    converter = converter + newline;
                    continue;
                }

                else if (line.StartsWith("EXIT LOOP"))
                {
                    string newline = Tab() + "break\n";
                    converter = converter + newline;
                    continue;
                }

                else if (line.StartsWith("ENDREPEAT") || line.StartsWith("ENDIF"))
                {
                    tab--;
                    converter = converter + Tab() + "}\n";
                    continue;
                }

                else if (line.StartsWith("IF IMAGE"))
                {
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] posSplit = strlist[5].Split(new[] { ';' });
                    int posXX = Int32.Parse(posSplit[0]) + Int32.Parse(posSplit[2]);
                    int posYY = Int32.Parse(posSplit[1]) + Int32.Parse(posSplit[3]);
                    string newline = Tab() + String.Format("ImageSearch, , , {0}, {1}, {2}, {3}, %A_WorkingDir%\\.AK\\{4}.bmp\n", posSplit[0], posSplit[1], posXX.ToString(), posYY.ToString(), nameImage);
                    newline +=
                        Tab() + "If !ErrorLevel\n" +
                        Tab() + "{\n";
                    tab++;
                    converter = converter + newline;
                    nameImage = "";
                    continue;
                }

                else if (line.StartsWith("ELSE"))
                {
                    tab--;
                    string newline = Tab() + "} else {\n";
                    tab++;
                    converter = converter + newline;
                    continue;
                }

                else if (line.StartsWith("LABEL"))
                {
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    string newline = strlist[1] = ":\n ";
                    converter = converter + newline;
                    continue;
                }

                else if (line.StartsWith("GOTO"))
                {
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    string newline = "Goto, " + strlist[1] + "\n";
                    converter = converter + newline;
                    continue;
                }

                else if (line.StartsWith("Mouse"))
                {
                    string newline = "";
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    if (strlist[3] == "Click" || strlist[3] == "LeftButtonDown")
                    {
                        newline += Tab() + String.Format("MouseClick, left, {0}, {1}\n", strlist[1], strlist[2]);
                    }
                    else if (strlist[3] == "Move")
                    {
                        newline += Tab() + String.Format("MouseMove, {0}, {1}\n", strlist[1], strlist[2]);
                    }
                    else if (strlist[3] == "RightButtonDown" || strlist[3] == "RightClick")
                    {
                        newline += Tab() + String.Format("MouseClick, right, {0}, {1}\n", strlist[1], strlist[2]);
                    }
                    else
                        MessageError(line, currentLine);
                    converter = converter + newline;
                    continue;
                }

                else if (line.StartsWith("Keyboard"))
                {
                    string[] strlist = line.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    string newline = "";

                    string keyNameFix = strlist[1];

                    if (keyNameFix == "Back")
                    {
                        keyNameFix = "Backspace";
                    }
                    else if (keyNameFix == "PageUP")
                    {
                        keyNameFix = "PgUp";
                    }
                    else if (keyNameFix == "PageDOWN")
                    {
                        keyNameFix = "PgDn";
                    }
                    else if (keyNameFix == "D0")
                    {
                        keyNameFix = "0";
                    }
                    else if (keyNameFix == "D1")
                    {
                        keyNameFix = "1";
                    }
                    else if (keyNameFix == "D2")
                    {
                        keyNameFix = "2";
                    }
                    else if (keyNameFix == "D3")
                    {
                        keyNameFix = "PgUp";
                    }
                    else if (keyNameFix == "D4")
                    {
                        keyNameFix = "4";
                    }
                    else if (keyNameFix == "D5")
                    {
                        keyNameFix = "5";
                    }
                    else if (keyNameFix == "D6")
                    {
                        keyNameFix = "6";
                    }
                    else if (keyNameFix == "D7")
                    {
                        keyNameFix = "7";
                    }
                    else if (keyNameFix == "D8")
                    {
                        keyNameFix = "8";
                    }
                    else if (keyNameFix == "D9")
                    {
                        keyNameFix = "9";
                    }
                    else if (keyNameFix == "ShiftLeft" || keyNameFix == "ShiftRight")
                    {
                        keyNameFix = "LShift";
                    }
                    else if (keyNameFix == "ControlLeft" || keyNameFix == "ControlRight")
                    {
                        keyNameFix = "LControl";
                    }
                    else if (keyNameFix == "AltLeft" || keyNameFix == "AltRight")
                    {
                        keyNameFix = "LAlt";
                    }

                    keyNameFix = keyNameFix.ToLower();

                    if (strlist[2] == "KeyPress")
                    {
                        newline =
                         Tab() + "AHI.SendKeyEvent(keyboardId, GetKeySC(\"" + keyNameFix + "\"), 1)\n"
                        + Tab() + "Sleep 33\n"
                        + Tab() + "AHI.SendKeyEvent(keyboardId, GetKeySC(\"" + keyNameFix + "\"), 0)\n";
                    }
                    else if (strlist[2] == "KeyDown")
                    {
                        newline = Tab() + "AHI.SendKeyEvent(keyboardId, GetKeySC(\"" + keyNameFix + "\"), 1)\n";
                    }
                    else if (strlist[2] == "KeyUp")
                    {
                        newline = Tab() + "AHI.SendKeyEvent(keyboardId, GetKeySC(\"" + keyNameFix + "\"), 0)\n";
                    }
                    else
                        MessageError(line, currentLine);
                    converter = converter + newline;
                    continue;
                }

                else
                {
                    MessageError(line, currentLine);
                }

            }
            reader.Close();
            File.WriteAllText("C:\\Windows\\conv", converter);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Run();
        }
    }
}
