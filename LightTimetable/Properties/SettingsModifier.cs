using Newtonsoft.Json;

using System.Collections.Generic;


namespace LightTimetable.Properties
{
    internal sealed partial class Settings : IUserSettings
    {
        public override void Save()
        {
            Default._renames = JsonConvert.SerializeObject(Renames);
            Default._notes = JsonConvert.SerializeObject(Notes);
            base.Save();
        }

        private Dictionary<string, string>? _sRenames;
        private Dictionary<uint, string>? _sNotes;

        public Dictionary<string, string> Renames
        {
            get => _sRenames ??= JsonConvert.DeserializeObject<Dictionary<string, string>>(Default._renames) ?? new();
            set => _sRenames = value;
        }

        public Dictionary<uint, string> Notes
        {
            get => _sNotes ??= JsonConvert.DeserializeObject<Dictionary<uint, string>>(Default._notes) ?? new();
            set => _sNotes = value;
        }
    }
}