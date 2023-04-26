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
        public DisciplinePair(string modifiedName, string originalName)
        {
            Modified = modifiedName;
            Original = originalName;
        }
        public override string ToString() => Modified;
    }
}
