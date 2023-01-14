using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Main
{
    /// <summary>
    /// Interactions with tray
    /// </summary>

    public partial class MainWindow : Window
    {
        private void InitializeTray()
        {
            NotifyIcon Tray = new NotifyIcon();
            Tray.Icon = new Icon(this.GetType(), "Resources.MainWindowIcon.ico");
            Tray.Text = "Розклад";
            Tray.Visible = true;
            Tray.ContextMenuStrip = InitializeTrayMenu(Tray);
            Tray.DoubleClick += DeiconifyWindow;
        }

        private ContextMenuStrip InitializeTrayMenu(NotifyIcon Tray)
        {
            ContextMenuStrip TrayContextMenu = new ContextMenuStrip();

            var MenuItem_Show = new ToolStripMenuItem("Показати");
            MenuItem_Show.Click += DeiconifyWindow;
            TrayContextMenu.Items.Add(MenuItem_Show);

            TrayContextMenu.Items.Add("-");

            var MenuItem_Close = new ToolStripMenuItem("Закрити");
            MenuItem_Close.Click += delegate (object? sender, EventArgs args) 
            {
                Tray.Visible = false; 
                Close(); 
            };
            TrayContextMenu.Items.Add(MenuItem_Close);

            return TrayContextMenu;
        }

        private void DeiconifyWindow(object? sender, EventArgs args)
        {
            this.Show(); 
            this.WindowState = WindowState.Normal;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
            base.OnStateChanged(e);
        }

    }


}