using Newtonsoft.Json;

using System.Collections.Generic;


namespace Timetable.Properties
{
    internal sealed partial class Settings
    {
        public Dictionary<string, string> RenameList
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(Default._renameList);
            }
            set
            {
                Default._renameList = JsonConvert.SerializeObject(value);
                Default.Save();
            }
        }
        public void AppendToRenameList(string key, string value)
        {
            var tempList = RenameList;
            if (tempList.ContainsKey(key))
            {
                tempList[key] = value;
            }
            else
            {
                tempList.Add(key, value);
            }
            RenameList = tempList;
        }

    }
}
