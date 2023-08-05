using System.Collections.Generic;


namespace LightTimetable.Properties
{
    public interface IUserSettings
    {
        public string ScheduleSource { get; set; }
        public string Faculty { get; set; }
        public string EducationForm { get; set; }
        public string Course { get; set; }
        public string StudyGroup { get; set; }
        public bool ShowOutages { get; set; }
        public bool ShowPossibleOutages { get; set; }
        public int OutagesGroup { get; set; }
        public string OutagesCity { get; set; }
        public bool ShowTeamsEvents { get; set; }
        public bool ShowOldEvents { get; set; }
        public int WindowPosition { get; set; }
        public int Autostart { get; set; }
        public int OpenWindowMode { get; set; }
        public int MiddleMouseClick { get; set; }
        public bool ShowRiggedSchedule { get; set; }
        public Dictionary<string, string> Renames { get; set; }
        public Dictionary<uint, string> Notes { get; set; }
        public void Save();
        public void Upgrade();

    }
}
