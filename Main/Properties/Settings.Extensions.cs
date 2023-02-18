using Newtonsoft.Json;

using System.Collections.Generic;


namespace Timetable.Properties
{
    internal sealed partial class Settings
    {
        public Dictionary<string, string> Renames
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(Default._renames);
            }
            set
            {
                Default._renames = JsonConvert.SerializeObject(value);
                Default.Save();
            }
        }
        public void AppendToRenames(string key, string value)
        {
            var tempList = Renames;
            if (tempList.ContainsKey(key))
            {
                tempList[key] = value;
            }
            else
            {
                tempList.Add(key, value);
            }
            Renames = tempList;
        }

    }
}
