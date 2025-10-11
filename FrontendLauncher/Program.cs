using System.Diagnostics;

//külön program.cs kell hogy elindítsa a frontendet is startup projektből
string frontendPath = @"C:\Users\tenye\source\repos\gymappforbigmuscle\"; //teljes elérési út

ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c start-frontend.bat")
{
    WorkingDirectory = frontendPath,
    UseShellExecute = true
};

Process.Start(processInfo);