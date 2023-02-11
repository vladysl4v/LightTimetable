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
        private readonly NotifyIcon _tray = new NotifyIcon();
        private readonly MainWindow _mainWindow = new MainWindow();
        private SettingsWindow _settingsWindow;

        public TrayMenu()
        {
            _tray.Icon = new Icon("Resources/MainWindowIcon.ico");
            _tray.Text = "Розклад";
            _tray.Visible = true;
            _tray.ContextMenuStrip = InitializeTrayMenu();
            _tray.DoubleClick += DeiconifyWindow;
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


        private async void RefreshData(object? sender, EventArgs? args)
        {
            await UserData.Initialize();
            _mainWindow.FillTimetable();
            _mainWindow.RefreshDatePicker();
            _mainWindow.RenderWidgets();
        }

        // Actions when selecting a menu item

        private void OpenSettingsWindow(object? sender, EventArgs args)
        {
            _settingsWindow = new SettingsWindow(RefreshData);
            _settingsWindow.Show();
        }

        private void DeiconifyWindow(object? sender, EventArgs args)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }

        private void CloseApp(object? sender, EventArgs args)
        {
            _tray.Visible = false;
            _settingsWindow?.Close();
            _mainWindow.Close();
        }
    }
}