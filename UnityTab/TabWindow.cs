using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UnityTab
{
    public partial class TabWindow : Form
    {

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        private int initHeight = 0;
        private IntPtr unityHWND = IntPtr.Zero;
        private Timer reloadWindowTimer = new Timer();
        private string[] nowTabNames = new string[0];
        public delegate void SelectedTabItem(int index);

        public SelectedTabItem selectedDelegate;
        public string[] LayoutFiles => Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Unity\Editor-5.x\Preferences\Layouts", "*", SearchOption.AllDirectories);

        public TabWindow()
        {
            InitializeComponent();

            initHeight = this.Height;

            UpdateUnityHWND();
            UpdateTab();

            //reloadWindowTimer.Interval = 2000;
            //reloadWindowTimer.Tick += DispatcherTimer_Tick;
            //reloadWindowTimer.Start();
        }
        
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var hwnd = GetForegroundWindow();
            if (hwnd == Handle)
                return;

            UpdateTab();

            var rect = new RECT();
            GetWindowRect(hwnd, ref rect);
            this.Left = rect.left;
            this.Width = rect.right - rect.left;
            this.Top = rect.top - this.Height + (int)(initHeight / 4.0);
            this.Height = initHeight;
        }

        private void UpdateUnityHWND()
        {
            Process unityProcess = null;
            foreach (var p in Process.GetProcesses())
            {
                if (p.ProcessName == "Unity")
                    unityProcess = p;
            }
            if (unityProcess == null) return;
            unityHWND = unityProcess.MainWindowHandle;
        }

        private void UpdateTab()
        {
            if (nowTabNames.Length == 0 || nowTabNames.Last() != LayoutFiles.Select(_ => Path.GetFileNameWithoutExtension(@_)).ToArray().Last())
            {
                tabControl1.Controls.Clear();
                nowTabNames = LayoutFiles.Select(_ => Path.GetFileNameWithoutExtension(@_)).ToArray();
                foreach (var fileName in nowTabNames)
                {
                    tabControl1.Controls.Add(new TabPage(fileName));
                }
            }
        }

        // Method Callback

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            selectedDelegate(e.TabPageIndex);
        }

        private void TabWindow_Activated(object sender, ControlEventArgs e)
        {
            ActivateUnityWindow();
        }

        private void TabWindow_Deactivate(object sender, EventArgs e)
        {
            DeactivateUnityWindow();
        }

        private void ActivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private void TabWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            TabMain.tabWin = null;
            reloadWindowTimer.Stop();
            using (reloadWindowTimer) { }
        }
    }
}
