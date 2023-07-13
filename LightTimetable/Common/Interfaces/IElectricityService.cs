using System.Threading.Tasks;
using System.Collections.Generic;


namespace LightTimetable.Common
{
    public interface IElectricityService
    {
        public List<SpecificOutage> GetOutagesInformation(TimeInterval time, NormalDayOfWeek dayOfWeek);
        public Task InitializeAsync();
    }
}