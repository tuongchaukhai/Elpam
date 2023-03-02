#include AutoHotInterception.ahk

AHI := new AutoHotInterception()
if keyboardId = "default"
DeviceList := AHI.GetDeviceList(false)

^Esc::ExitApp
