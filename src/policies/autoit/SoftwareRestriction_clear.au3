#RequireAdmin
#include "GPupdate.au3"

Send("{LWIN}")
Send("secpol.msc")
Sleep(400)
Send("{ENTER}")
WinWait("Lokaal beveiligingsbeleid")
WinActive("Lokaal beveiligingsbeleid")
Send("{ENTER}")
MouseClick("primary", 45, 206, 1, 0)
MouseClick("right", 45, 207, 1, 0)
Sleep(300)
MouseClick("primary", 48, 212, 1, 0)
Sleep(300)
Send("{ENTER}")
;closing the script...
MouseClick("primary", 785, 3, 1, 0)
updatePolicy()