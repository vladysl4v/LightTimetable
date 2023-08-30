using LightTimetable.Models;
using LightTimetable.Models.Enums;
using LightTimetable.Services.Enums;


namespace LightTimetable.Services.Models
{
    public record SpecificOutage
    {
        public TimeInterval Time { get; init; }
        public OutageType Type { get; init; }

        public override string ToString()
        {
            var outageText = Type == OutageType.Possible ?
                "можливе вiдключення" : "електроенергії не буде";
            return Time + " - " + outageText;
        }
    }
}