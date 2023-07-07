using System;

using LightTimetable.DataTypes.Interfaces;


namespace LightTimetable.Tools
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScheduleSourceAttribute : Attribute
    { 
        public string Name { get; set; }
        public Type Settings { get; set; }
        public ScheduleSourceAttribute(string name, Type settingsClass)
        {
            Name = name;
            if (!settingsClass.IsAssignableTo(typeof(IScheduleSettings)))
            {
                throw new TypeLoadException("Type is not implemented IScheduleSettings interface.");
            }
            Settings = settingsClass;
        }
    }
}