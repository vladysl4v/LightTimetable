using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Timetable.Utilities

{    /// <summary>
     /// Enumeration that makes it easier to understand the kind of lights shutdown
     /// </summary>
    public enum LIGHT_TYPE
    {
        PRESENT,
        MAYBE,
        ABSENT
    };

    /// <summary>
    /// Mutable kind of KeyValuePair that is used for the renaming table in settings
    /// </summary>
    public class MutablePair<KeyType, ValueType>
    {
        public KeyType Key { get; set; }
        public ValueType Value { get; set; }
        public MutablePair(KeyValuePair<KeyType, ValueType> keyValuePair)
        {
            this.Key = keyValuePair.Key;
            this.Value = keyValuePair.Value;
        }
    }

    /// <summary>
    /// Class that constructs text and an icon for lights shutdown information
    /// </summary>
    public class ElectricityStatus
    {
        public string Icon { get; private set; } = string.Empty;
        public string Text { get; private set; } = string.Empty;

        private LIGHT_TYPE _type = LIGHT_TYPE.PRESENT;
        private List<string> _maybeTimes = new List<string>();
        private List<string> _definitelyTimes = new List<string>();
        private readonly bool _showMaybe = Properties.Settings.Default.ShowPossibleBlackouts;

        public void Add(string hour, LIGHT_TYPE addType)
        {
            if (addType > _type) _type = addType;

            if (addType == LIGHT_TYPE.MAYBE && _showMaybe)
                _maybeTimes.Add(hour);

            if (addType == LIGHT_TYPE.ABSENT)
                _definitelyTimes.Add(hour);
        }
        public void Finish()
        {
            if (_type == LIGHT_TYPE.MAYBE && _showMaybe)
                Icon = "../Resources/MaybeElectricityIcon.png";
            else if (_type == LIGHT_TYPE.ABSENT)
                Icon = "../Resources/NoElectricityIcon.png";

            Text = GetText();
        }
        private string GetText()
        {
            _maybeTimes = _maybeTimes.OrderBy(x => int.Parse(x)).Select(x => Convert.ToString(x)).ToList();
            _definitelyTimes = _definitelyTimes.OrderBy(x => int.Parse(x)).Select(x => Convert.ToString(x)).ToList();
            StringBuilder result = new StringBuilder();

            result.Append("Ймовірнi відключення:");

            if (_maybeTimes.Count > 0)
            {
                int startHour = int.Parse(_maybeTimes.First()) - 1;
                int endHour = int.Parse(_maybeTimes.Last());
                result.Append($"\n{startHour}:00-{endHour}:00 - можливе відключення");
            }
            if (_definitelyTimes.Count > 0)
            {
                int startHour = int.Parse(_definitelyTimes.First()) - 1;
                int endHour = int.Parse(_definitelyTimes.Last());
                result.Append($"\n{startHour}:00-{endHour}:00 - електроенергії не буде");
            }

            return result.ToString();
        }
    }

    /// <summary>
    /// User input window
    /// </summary>
    public class InputBox : Window
    {
        private bool _isOKClicked = false;
        private readonly StackPanel _panel = new StackPanel();
        private readonly TextBox _textBox = new TextBox();
        private readonly Label _label = new Label();
        private readonly Button _button = new Button();


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
            if (!_isOKClicked)
                _textBox.Text = string.Empty;
        }
        private void OKButton_Clicked(object s, EventArgs e)
        {
            _isOKClicked = true;
            Close();
        }
        private void ChangeView()
        {
            Topmost = true;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            Content = _panel;
            Height = 150;
            Width = 300;

            _label.Width = 260;
            _label.HorizontalAlignment = HorizontalAlignment.Center;
            _label.VerticalAlignment = VerticalAlignment.Top;
            _textBox.Height = 40;
            _textBox.Width = 250;
            _textBox.MaxLines = 2;
            _textBox.HorizontalAlignment = HorizontalAlignment.Right;
            _textBox.Margin = new Thickness { Right = 15 };
            _textBox.VerticalAlignment = VerticalAlignment.Center;
            _button.Content = "ОК";
            _button.Height = 20;
            _button.Width = 90;
            _button.HorizontalAlignment = HorizontalAlignment.Right;
            _button.VerticalAlignment = VerticalAlignment.Bottom;
            _button.Margin = new Thickness { Top = 10, Right = 15 };
            _button.IsDefault = true;

            _panel.Children.Add(_label);
            _panel.Children.Add(_textBox);
            _panel.Children.Add(_button);
        }
    }


}
