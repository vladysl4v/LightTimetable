using System.Text;
using System.Collections.Generic;


namespace LightTimetable.Services.Models
{
    public class EventsContainer : List<SpecificEvent>
    {
        public string Information { get; set; }

        public EventsContainer(IEnumerable<SpecificEvent> list)
        {
            AddRange(list);
            Information = CreateString();
        }

        public string CreateString()
        {
            var sbuilder = new StringBuilder();
            sbuilder.Append("Запланованi наради:");
            foreach (var specEvent in this)
            {
                sbuilder.Append("\n" + specEvent);
            }
            return sbuilder.ToString();
        }
    }
}