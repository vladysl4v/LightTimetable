using System;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Services.Enums;



namespace LightTimetable.Services.Models
{
    public class OutageFactory : FactoryConverter<List<SpecificOutage>, Dictionary<string, string>>
    {
        public override List<SpecificOutage> CreateAndPopulate(Type objectType, Dictionary<string, string> arguments)
        {
            var output = new List<SpecificOutage>();
            foreach (var pair in arguments)
            {
                OutageType outage = pair.Value switch
                {
                    "yes" => OutageType.Definite,
                    "maybe" => OutageType.Possible,
                    _ => OutageType.Not
                };
                int time = int.Parse(pair.Key);

                if (!TimeOnly.TryParseExact(time.ToString(), "%H", out var outageEnd))
                {
                    outageEnd = new TimeOnly(23, 59);
                }
                output.Add(new SpecificOutage
                {
                    Type = outage,
                    Time = new TimeInterval(TimeOnly.ParseExact((time - 1).ToString(), "%H"), outageEnd)
                });
            }

            return output;
        }
    }
}
