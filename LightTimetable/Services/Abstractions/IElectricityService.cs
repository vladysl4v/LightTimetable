using System.Threading.Tasks;
using System.Collections.Generic;

using LightTimetable.Models;
using LightTimetable.Models.Enums;
using LightTimetable.Services.Models;


namespace LightTimetable.Services.Abstractions
{
    public interface IElectricityService
    {
        public List<SpecificOutage> GetOutagesInformation(TimeInterval time, NormalDayOfWeek dayOfWeek);
        public Task InitializeAsync();
    }
}