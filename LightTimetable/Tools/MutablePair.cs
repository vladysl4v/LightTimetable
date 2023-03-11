using System.Collections.Generic;


namespace LightTimetable.Tools
{
    /// <summary>
    /// Mutable kind of KeyValuePair
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
        public MutablePair(KeyType key, ValueType value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString()
        {
            return (string)(object)Key;
        }
    }
}
