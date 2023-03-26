using System.Collections.Generic;


namespace LightTimetable.Tools
{
    public class DisciplinePair
    {
        public string Modified { get; set; }
        public string Original { get; set; }

        public DisciplinePair(KeyValuePair<string, string> keyValuePair)
        {
            Modified = keyValuePair.Key;
            Original = keyValuePair.Value;
        }
        public DisciplinePair(string key, string value)
        {
            Modified = key;
            Original = value;
        }

        public override string ToString()
        {
            return Modified;
        }
    }
}
