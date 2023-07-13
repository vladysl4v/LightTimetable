using Newtonsoft.Json;

using System;


namespace LightTimetable.Common
{
    public class DataItem : IComparable
    {
        public uint Id { get; set; }
        public DateTime Date { get; set; }
        public TimeInterval StudyTime { get; set; }
        public DisciplineName Discipline { get; set; }
        public string StudyType { get; set; }
        public string Cabinet { get; set; } 
        public string Employee { get; set; }
        public string Promt { get; set; }
        public string Note { get; set; }
        public OutagesContainer? Outages { get; set; }
        [JsonIgnore]
        public EventsContainer? Events { get; set; }

        public int CompareTo(object? item)
        {
            var dataItem = (DataItem)item;
            return StudyTime.Start == dataItem.StudyTime.Start ? 0 
                   : StudyTime.Start > dataItem.StudyTime.Start ? 1
                   : -1 ;
        }
    }
}