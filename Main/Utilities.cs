using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;


namespace Timetable.Utilities

{    /// <summary>
     /// Enumeration that makes it easier to understand the kind of lights shutdown
     /// </summary>
    public enum LIGHT_TYPE
    {
        PRESENT,
        MAYBE,
        ABSENT
    };

    /// <summary>
    /// Mutable kind of KeyValuePair that is used for the renaming table in settings
    /// </summary>
    public class MutablePair<KeyType, ValueType>
    {
        public KeyType Key { get; set; }
        public ValueType Value { get; set; }
        public MutablePair(KeyValuePair<KeyType, ValueType> keyValuePair)
        {
            this.Key = keyValuePair.Key;
            this.Value = keyValuePair.Value;
        }
    }

    /// <summary>
    /// Class that constructs text and an icon for lights shutdown information
    /// </summary>
    public class ElectricityStatus
    {
        public string Icon { get; private set; } = string.Empty;
        public string Text { get; private set; } = string.Empty;

        private LIGHT_TYPE _type = LIGHT_TYPE.PRESENT;
        private List<string> _maybeTimes = new List<string>();
        private List<string> _definitelyTimes = new List<string>();
        private readonly bool _showMaybe = Properties.Settings.Default.ShowPossibleBlackouts;

        public void Add(string hour, LIGHT_TYPE addType)
        {
            if (addType > _type) _type = addType;

            if (addType == LIGHT_TYPE.MAYBE && _showMaybe)
                _maybeTimes.Add(hour);

            if (addType == LIGHT_TYPE.ABSENT)
                _definitelyTimes.Add(hour);
        }
        public void Finish()
        {
            if (_type == LIGHT_TYPE.MAYBE && _showMaybe)
                Icon = "../Resources/MaybeElectricityIcon.png";
            else if (_type == LIGHT_TYPE.ABSENT)
                Icon = "../Resources/NoElectricityIcon.png";

            Text = GetText();
        }
        private string GetText()
        {
            _maybeTimes = _maybeTimes.OrderBy(x => int.Parse(x)).Select(x => Convert.ToString(x)).ToList();
            _definitelyTimes = _definitelyTimes.OrderBy(x => int.Parse(x)).Select(x => Convert.ToString(x)).ToList();
            StringBuilder result = new StringBuilder();

            result.Append("Ймовірнi відключення:");

            if (_maybeTimes.Count > 0)
            {
                int startHour = int.Parse(_maybeTimes.First()) - 1;
                int endHour = int.Parse(_maybeTimes.Last());
                result.Append($"\n{startHour}:00-{endHour}:00 - можливе відключення");
            }
            if (_definitelyTimes.Count > 0)
            {
                int startHour = int.Parse(_definitelyTimes.First()) - 1;
                int endHour = int.Parse(_definitelyTimes.Last());
                result.Append($"\n{startHour}:00-{endHour}:00 - електроенергії не буде");
            }

            return result.ToString();
        }
    }
}
