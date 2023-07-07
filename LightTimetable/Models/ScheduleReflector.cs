using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using LightTimetable.Tools;
using LightTimetable.DataTypes.Interfaces;


namespace LightTimetable.Models
{
    public static class ScheduleReflector
    {
        public static (bool faculties, bool educType, bool course) ConfigureFiltersVisibility(string universityName)
        {
            var scheduleType = GetScheduleSourceType(universityName);
            if (scheduleType == null)
            {
                return (true, true, true);
            }
            var settingsType = scheduleType.GetCustomAttribute<ScheduleSourceAttribute>()!.Settings;
            
            var educTypeVisibility = settingsType.GetProperty("EducationTypes")!
                .GetCustomAttribute<HideFilterAttribute>() == null;
            var facultiesVisibility = settingsType.GetProperty("Faculties")!
                .GetCustomAttribute<HideFilterAttribute>() == null;
            var courseVisibility = settingsType.GetProperty("Courses")!
                .GetCustomAttribute<HideFilterAttribute>() == null;
            
            return (facultiesVisibility, educTypeVisibility, courseVisibility);
        }

        public static IScheduleSource? GetScheduleSource(string universityName, params object?[] args)
        {
            var scheduleType = GetScheduleSourceType(universityName);
            if (scheduleType == null)
            {
                return null;
            }
            
            return (IScheduleSource)Activator.CreateInstance(scheduleType, args)!;
        }

        public static IScheduleSettings? GetScheduleSettings(string universityName)
        {
            var scheduleType = GetScheduleSourceType(universityName);
            if (scheduleType == null)
            {
                return null;
            }
            var settingsType = scheduleType.GetCustomAttribute<ScheduleSourceAttribute>()!.Settings;
            
            return (IScheduleSettings)Activator.CreateInstance(settingsType)!;
        }

        public static List<string> GetScheduleNames()
        {
            var sourceNames = new HashSet<string>();
            foreach (var sourceType in GetAllScheduleSources())
            {
                var attribute = sourceType.GetCustomAttribute<ScheduleSourceAttribute>();
                if (attribute != null)
                {
                    sourceNames.Add(attribute.Name);
                }
            }
            return sourceNames.ToList();
        }

        private static Type? GetScheduleSourceType(string universityName)
        {
            return GetAllScheduleSources()
                .Where(t => t.GetCustomAttribute<ScheduleSourceAttribute>()?.Name == universityName)
                    .SingleOrDefault();
        }

        private static List<Type> GetAllScheduleSources()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "LightTimetable.Models.ScheduleSources")
                   .Where(t => t.GetCustomAttribute<ScheduleSourceAttribute>() != null)
                       .ToList();
        }
    }
}