using System;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;
using System.Drawing;

public class AmazeAim : Form
{
    private static string AppName = "AmazeAim";
    private static string GithubUrl = "https://github.com/Adiru3";
    private static string InstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "AmazeAim");
    private static string ExeName = "AmazeAim.exe";

    private NotifyIcon trayIcon;
    private ContextMenu trayMenu;
    private Timer optimizationTimer;

    [STAThread]
    static void Main()
    {
        if (!IsAdmin())
        {
            MessageBox.Show("Please run as Administrator!", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        SetupInstallation();
        SetupTaskScheduler();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new AmazeAim());
    }

    public AmazeAim()
    {
        trayMenu = new ContextMenu();
        trayMenu.MenuItems.Add("Open GitHub", new EventHandler(delegate { Process.Start(GithubUrl); }));
        trayMenu.MenuItems.Add("-");
        trayMenu.MenuItems.Add("Exit", new EventHandler(OnExit));

        trayIcon = new NotifyIcon();
        trayIcon.Text = AppName;
        trayIcon.Icon = SystemIcons.Shield; 
        trayIcon.ContextMenu = trayMenu;
        trayIcon.Visible = true;

        optimizationTimer = new Timer();
        optimizationTimer.Interval = 30000; 
        optimizationTimer.Tick += new EventHandler(delegate { ExecuteFullOptimization(); });
        optimizationTimer.Start();

        ExecuteFullOptimization();

        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
        this.Visible = false;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        this.Hide();
    }

    private void OnExit(object sender, EventArgs e)
    {
        trayIcon.Visible = false;
        Application.Exit();
    }

    public void ExecuteFullOptimization()
    {
        try 
        {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 0, RegistryValueKind.DWord);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\PriorityControl", "Win32PrioritySeparation", 26, RegistryValueKind.DWord);

            RunCmd("powercfg /SETACVALUEINDEX SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bebba308a3 48eaba3a-50f6-42d1-bc73-0fd23c96d354 0");
            RunCmd("powercfg /SETACVALUEINDEX SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bebba308a3 d633f0c7-9730-4618-abba-78bb35104a02 0");
            RunCmd("powercfg /SETACTIVE SCHEME_CURRENT");

            RunCmd("bcdedit /set disabledynamictick yes");
            RunCmd("bcdedit /set useplatformtick yes");

            BoostDrivers();
        }
        catch { }
    }

    private void BoostDrivers()
    {
        string[] drivers = { "LGHUB", "RazerCentral", "SteelSeriesEngine", "mouclass" };
        foreach (string name in drivers)
        {
            foreach (Process proc in Process.GetProcessesByName(name))
            {
                try {
                    proc.PriorityClass = ProcessPriorityClass.High;
                    proc.ProcessorAffinity = (IntPtr)0x3E; 
                } catch { }
            }
        }
    }

    private static bool IsAdmin()
    {
        return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static void SetupInstallation()
    {
        string currentExe = Process.GetCurrentProcess().MainModule.FileName;
        string targetPath = Path.Combine(InstallPath, ExeName);

        if (!Directory.Exists(InstallPath)) Directory.CreateDirectory(InstallPath);

        if (currentExe != targetPath)
        {
            try { File.Copy(currentExe, targetPath, true); } catch { }
        }
    }

    private static void SetupTaskScheduler()
    {
        string targetPath = Path.Combine(InstallPath, ExeName);
        // Заменяем $ на string.Format для совместимости со старым компилятором
        string taskCmd = string.Format("/Create /F /TN \"{0}\" /TR \"'{1}'\" /SC ONLOGON /RL HIGHEST", AppName, targetPath);
        
        ProcessStartInfo psi = new ProcessStartInfo("schtasks.exe", taskCmd);
        psi.CreateNoWindow = true;
        psi.UseShellExecute = false;
        Process.Start(psi).WaitForExit();
    }

    private void RunCmd(string args)
    {
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + args);
        psi.CreateNoWindow = true;
        psi.UseShellExecute = false;
        Process.Start(psi).WaitForExit();
    }
}