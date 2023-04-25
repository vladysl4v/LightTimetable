using System;


namespace LightTimetable.Tools.UtilityWindows
{
    public partial class InputBox
    {
        private bool _isOkClicked;
        private InputBox(string title, string bodyText, string defaultText)
        {
            InitializeComponent();
            Title = title;
            BodyLabel.Text = bodyText;
            InputField.Text = defaultText;
        }

        public static string Show(string title, string bodyText, string defaultText = "")
        {
            var inputWindow = new InputBox(title, bodyText, defaultText);
            inputWindow.ShowDialog();
            return inputWindow.InputField.Text;
        }

        private void OnWindowClosing(object? s, EventArgs e)
        {
            if (!_isOkClicked)
                InputField.Text = string.Empty;
        }
        private void OKButton_Clicked(object s, EventArgs e)
        {
            _isOkClicked = true;
            Close();
        }
        private void CloseButton_Clicked(object s, EventArgs e)
        {
            Close();
        }
    }
}
