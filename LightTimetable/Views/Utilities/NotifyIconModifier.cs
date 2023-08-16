using System.Windows;
using System.Windows.Input;

using Hardcodet.Wpf.TaskbarNotification;


namespace LightTimetable.Views.Utilities
{
    public class NotifyIconModifier : DependencyObject
    {
        public static ICommand GetMiddleMouseCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MiddleMouseProperty);
        }

        public static void SetMiddleMouseCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MiddleMouseProperty, value);
        }

        public static readonly DependencyProperty MiddleMouseProperty =
            DependencyProperty.RegisterAttached("MiddleMouseCommand", typeof(ICommand), typeof(NotifyIconModifier), new PropertyMetadata(null, OnMiddleMouseClick));

        private static void OnMiddleMouseClick(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (TaskbarIcon)sender;
            var command = (ICommand)e.NewValue;
            control.TrayMiddleMouseDown += (_, _) => command.Execute(null);
        }
    }
}
