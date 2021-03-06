using System;
using System.Windows.Forms;
using PSTaskDialog;
using System.Diagnostics;

namespace Fire
{
    public partial class HotkeyHandler : Form
    {

        public HotkeyHandler()
        {
            InitializeComponent();
        }

        public void ShowAboutDialog()
        {
            CenterToScreen();
            cTaskDialog.ShowTaskDialogBox(this,
                "About Fire",
                $"Fire {Fire.Version}",
                "A tool for killing process trees.\nCreated by LewisTehMinerz / Lewis Crichton\nLicensed under the MIT license." +
                (Fire.Beta ? "\n\nYou are running a beta version of Fire. Things may not work correctly. Remember, always report bugs on the GitHub repository." : ""),
                "",
                "",
                "",
                "",
                "GitHub Repository",
                eTaskDialogButtons.OK,
                eSysIcons.Information,
                eSysIcons.Question);
            if (cTaskDialog.CommandButtonResult == 0)
            {
                cTaskDialog.CommandButtonResult = 1;
                Process.Start("https://github.com/LewisTehMinerz/Fire");
            }
        }

        private void HotkeyHandler_Load(object sender, EventArgs e)
        {
            //                                                    SHIFT  +  F4
            var success = NativeMethods.RegisterHotKey(Handle, 0, 0x0004, 0x73);
            if (!success)
            {
                MessageBox.Show("Error while registering hotkey.");
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                var activeWindow = NativeMethods.GetForegroundWindow();
                NativeMethods.GetWindowThreadProcessId(activeWindow, out var pid);
                if (Process.GetCurrentProcess().Id != (int)pid)
                {
                    KillItWithFire((int)pid);
                }
            }
            base.WndProc(ref m);
        }

        private void KillItWithFire(int pid)
        {
            CenterToScreen();
            var res = cTaskDialog.MessageBox(this,
                "Kill it with Fire!",
                "Are you sure?",
                $"Are you sure you want to kill process '{Process.GetProcessById(pid).ProcessName}' (PID {pid}) with Fire, killing all subprocesses that were spawned by it?" +
                "\n\nYou could lose unsaved work if any subprocesses containing edited documents were spawned by this process.",
                eTaskDialogButtons.YesNo,
                eSysIcons.Warning);

            if (res == DialogResult.Yes)
            {
                try
                {
                    Fire.KillProcessAndChildren(pid);
                }
                catch (Exception e)
                {
                    cTaskDialog.MessageBox(this,
                        "Error",
                        "Failed to kill process tree.",
                        $"An error occured while killing the process tree of '{Process.GetProcessById(pid).ProcessName}' (PID {pid})."
                        + "\n\n" + e.Message,
                        eTaskDialogButtons.OK,
                        eSysIcons.Error);
                }
            }
        }

        private void HotkeyHandler_Shown(object sender, EventArgs e)
        {
            Hide();
        }
    }
}





