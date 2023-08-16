using System;

namespace LightTimetable.Models
{
    public class UpdatesMediator
    {
        public event Action? OnReloadRequired;
        public event Action? OnUpdateRequired;
        public event Action? OnRepositionRequired;

        public void ReloadRequired() => OnReloadRequired?.Invoke();
        public void UpdateRequired() => OnUpdateRequired?.Invoke();
        public void RepositionRequired() => OnRepositionRequired?.Invoke();
    }
}