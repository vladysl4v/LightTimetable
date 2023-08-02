using LightTimetable.Models;
using LightTimetable.Models.Enums;
using LightTimetable.Services.Enums;


namespace LightTimetable.Services.Models
{
    public record SpecificOutage
    {
        public TimeInterval Time { get; init; }
        public OutageType Type { get; init; }
        public NormalDayOfWeek DayOfWeek { get; init; }

        public SpecificOutage(TimeInterval time, OutageType type, NormalDayOfWeek dayOfWeek)
        {
            Time = time;
            Type = type;
            DayOfWeek = dayOfWeek;
        }

        public override string ToString()
        {
            var outageText = Type == OutageType.Possible ?
                "можливе вiдключення" : "електроенергії не буде";
            return Time + " - " + outageText;
        }
    }
}