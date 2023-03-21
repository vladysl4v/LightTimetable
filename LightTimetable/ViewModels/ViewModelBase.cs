using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace LightTimetable.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        // Multicast event for property change notifications.
        public event PropertyChangedEventHandler? PropertyChanged;

        // Checks if a property already matches a desired value.  Sets the property and
        // notifies listeners only when necessary.
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // Notifies listeners that a property value has changed.
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
