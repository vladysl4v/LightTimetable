using Timetable.Main;

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Timetable.Settings;


namespace Timetable
{
    /// <summary>
    /// Interactions with tray
    /// </summary>

    public class TrayMenu
    {
        private NotifyIcon Tray { get; } = new NotifyIcon();
        private MainWindow mainWindow { get; } = new MainWindow();
        private SettingsWindow settingsWindow { get; set; }

        public TrayMenu()
        {
            Tray.Icon = new Icon("Resources/MainWindowIcon.ico");
            Tray.Text = "Розклад";
            Tray.Visible = true;
            Tray.ContextMenuStrip = InitializeTrayMenu();
            Tray.DoubleClick += DeiconifyWindow;
        }

        private ContextMenuStrip InitializeTrayMenu()
        {
            ContextMenuStrip TrayContextMenu = new ContextMenuStrip();

            var MenuItem_Show = new ToolStripMenuItem("Показати");
            MenuItem_Show.Click += DeiconifyWindow;
            
            var MenuItem_Refresh = new ToolStripMenuItem("Обновити");
            MenuItem_Refresh.Click += RefreshData;

            var MenuItem_Settings = new ToolStripMenuItem("Налаштування");
            MenuItem_Settings.Click += OpenSettingsWindow;

            var MenuItem_Close = new ToolStripMenuItem("Закрити");
            MenuItem_Close.Click += CloseApp;

            TrayContextMenu.Items.Add(MenuItem_Show);
            TrayContextMenu.Items.Add(MenuItem_Refresh);
            TrayContextMenu.Items.Add("-");
            TrayContextMenu.Items.Add(MenuItem_Settings);
            TrayContextMenu.Items.Add("-");
            TrayContextMenu.Items.Add(MenuItem_Close);

            return TrayContextMenu;
        }

        // Actions when selecting a menu item
        private async void RefreshData(object? sender, EventArgs? args)
        {
            await UserData.Initialize();
            mainWindow.FillTimetable();
            mainWindow.RefreshDatePicker();
            mainWindow.RenderWidgets();
        }

        private void OpenSettingsWindow(object? sender, EventArgs args)
        {
            settingsWindow = new SettingsWindow(RefreshData);
            settingsWindow.Show();
        }

        private void DeiconifyWindow(object? sender, EventArgs args)
        {
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
        }

        private void CloseApp(object? sender, EventArgs args)
        {
            Tray.Visible = false;
            mainWindow.Close();
        }
    }
}