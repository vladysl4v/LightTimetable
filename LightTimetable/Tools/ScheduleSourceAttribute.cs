using System;

using LightTimetable.Common;


namespace LightTimetable.Tools
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScheduleSourceAttribute : Attribute
    { 
        public string Name { get; set; }
        public Type Settings { get; set; }
        public ScheduleSourceAttribute(string name, Type settingsClass)
        {
            if (!settingsClass.IsAssignableTo(typeof(IScheduleSettings)))
            {
                throw new TypeLoadException("Type does not implement IScheduleSettings interface.");
            }
            Name = name;
            Settings = settingsClass;
        }
    }
}