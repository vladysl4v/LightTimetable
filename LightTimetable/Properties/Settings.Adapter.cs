using Newtonsoft.Json;

using System.Collections.Generic;


namespace LightTimetable.Properties
{
    internal sealed partial class Settings
    {
        #region Renames

        public Dictionary<string, string> Renames
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(Default._renames) ?? new();
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
            tempList[key] = value;
            Renames = tempList;
        }

        #endregion

        #region Notes

        public Dictionary<uint, string> Notes
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<uint, string>>(Default._notes);
            }
            set
            {
                Default._notes = JsonConvert.SerializeObject(value);
                Default.Save();
            }
        }
        public void AppendToNotes(uint key, string value)
        {
            var tempList = Notes;
            tempList[key] = value;
            Notes = tempList;

        }
        public void RemoveFromNotes(uint key)
        {
            var tempList = Notes;
            tempList.Remove(key);
            Notes = tempList;
        }

        #endregion
    }
}