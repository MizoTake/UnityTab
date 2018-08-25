using System.Windows.Forms;
using System;
using System.Diagnostics;
using static UnityTab.TabWindow;

namespace UnityTab
{
    public class TabMain
    {
        public static TabWindow tabWin;
        public static string[] GetFilePath => tabWin.LayoutFiles;

        public static void Launch(SelectedTabItem delegateMethod)
        {
            if (tabWin != null) return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            tabWin = new TabWindow()
            {
                selectedDelegate = delegateMethod
            };
            tabWin.Show();
        }

        public static void Stop()
        {
            tabWin?.Close();
            tabWin = null;
        }

    }
}
