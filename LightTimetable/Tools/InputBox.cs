using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace LightTimetable.Tools
{
    public class InputBox : Window
    {
        private readonly StackPanel _panel = new();
        private readonly TextBox _textBox = new();
        private readonly Label _label = new();
        private readonly Button _button = new();

        private bool _isOkClicked;

        public InputBox(string title, string text, string defaultText = "")
        {
            Title = title;
            _label.Content = text;
            _textBox.Text = defaultText;

            _button.Click += OKButton_Clicked;
            Closing += OnWindowClosing;

            ChangeView();
        }

        public string GetText()
        {
            ShowDialog();
            return _textBox.Text;
        }

        private void OnWindowClosing(object? s, EventArgs e)
        {
            if (!_isOkClicked)
                _textBox.Text = string.Empty;
        }
        private void OKButton_Clicked(object s, EventArgs e)
        {
            _isOkClicked = true;
            Close();
        }
        private void ChangeView()
        {
            Topmost = true;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            Content = _panel;
            SizeToContent = SizeToContent.WidthAndHeight;
            Icon = new BitmapImage(new Uri("pack://application:,,,/LightTimetable;component/Assets/LightTimetable-Logo.ico"));

            _label.HorizontalAlignment = HorizontalAlignment.Stretch;
            _label.MinWidth = 250;
            _label.Margin = new Thickness(10, 0, 10, 0);
            _label.VerticalAlignment = VerticalAlignment.Top;

            _textBox.MinHeight = 40;
            _textBox.Margin = new Thickness(10, 0, 10, 0);
            _textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            _textBox.VerticalAlignment = VerticalAlignment.Center;
            _textBox.AcceptsReturn = true;

            _button.Content = "ОК";
            _button.Height = 20;
            _button.Width = 90;
            _button.HorizontalAlignment = HorizontalAlignment.Right;
            _button.VerticalAlignment = VerticalAlignment.Bottom;
            _button.Margin = new Thickness(0, 10, 10, 10);

            _panel.Children.Add(_label);
            _panel.Children.Add(_textBox);
            _panel.Children.Add(_button);
        }
    }
}
