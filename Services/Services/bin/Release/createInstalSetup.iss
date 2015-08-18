; -- Example1.iss --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Services
AppVersion=0.1
DefaultDirName={pf}\Services
DefaultGroupName=Services
UninstallDisplayIcon={app}\uninstall.exe
Compression=lzma2
SolidCompression=yes
OutputDir=c:\nmbackup\Install
;LicenseFile=license.txt

[Files]
Source: "Services.exe"; DestDir: "{app}"   
Source: "MySql.Data.dll"; DestDir: "{app}"
Source: "mysqldump.exe"; DestDir: "{app}"


;Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

[Icons]
;Name: "{group}\My Program"; Filename: "{app}\MyProg.exe"  [Icons]
Name: "{group}\Services"; Filename: "{app}\Noiva Modas 2.0.exe"; WorkingDir: "{app}"
Name: "{group}\Desistalar Services"; Filename: "{uninstallexe}"
Name: "{commondesktop}\Services"; Filename: "{app}\Services.exe"; WorkingDir: "{app}"
Name: "{commonprograms}\Services"; Filename: "{app}\Services.exe"; WorkingDir: "{app}"
Name: "{commonstartup}\Services"; Filename: "{app}\Services.exe"; WorkingDir: "{app}"

[Registry]
Root: "HKLM"; Subkey: "SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers\"; ValueType: String; ValueName: "{app}\Services.exe"; ValueData: "RUNASADMIN"; Flags: uninsdeletekeyifempty uninsdeletevalue; MinVersion: 0,10.0