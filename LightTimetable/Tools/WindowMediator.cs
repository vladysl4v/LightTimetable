using System;

namespace LightTimetable.Tools
{
    /// <summary>
    /// Serves as a mediator between user settings and the main viewmodel
    /// </summary>
    public static class WindowMediator
    {
        public static event Action? OnReloadRequired;
        public static event Action? OnUpdateRequired;
        public static event Action? OnRepositionRequired;
        
        public static void ReloadRequired() => OnReloadRequired?.Invoke();
        public static void UpdateRequired() => OnUpdateRequired?.Invoke();
        public static void RepositionRequired() => OnRepositionRequired?.Invoke();
    }
}