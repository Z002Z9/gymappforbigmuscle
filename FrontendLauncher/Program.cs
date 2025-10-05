using System.Diagnostics;

//külön program.cs kell hogy elindítsa a frontendet is startup projektből
string frontendPath = @"E:\egyetem\5felev\projektlab\biproject\GymApp\"; //teljes elérési út

ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c start-frontend.bat")
{
    WorkingDirectory = frontendPath,
    UseShellExecute = true
};

Process.Start(processInfo);