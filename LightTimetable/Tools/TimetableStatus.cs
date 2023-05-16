namespace LightTimetable.Models
{
    public readonly struct TimetableStatus
    {
        public readonly string? IconPath { get; }
        public readonly string? ToolTip { get; }
        public readonly int Priority;
        
        private TimetableStatus(string? iconPath, string? toolTip, int priority)
        {
            IconPath = iconPath;
            ToolTip = toolTip;
            Priority = priority;
        }
        public static bool operator ==(TimetableStatus left, TimetableStatus right)
        {
            return left.Priority == right.Priority;
        }  
        public static bool operator !=(TimetableStatus left, TimetableStatus right)
        {
            return left.Priority != right.Priority;
        }  

        public static TimetableStatus Default => new TimetableStatus(null, null, 0);
        public static TimetableStatus RiggedScheduleShown => new TimetableStatus("../Assets/Status/Warning.png", "Відображається не справжній розклад, а згенерований на основі минулих тижнів.", 1);
        public static TimetableStatus LoadingData => new TimetableStatus("../Assets/Status/Loading.png", "Дані розкладу завантажуються.", 2);
        public static TimetableStatus DataLoadingError => new TimetableStatus("../Assets/Status/Warning.png", "Помилка завантаження розкладу.", 3);
    }
}
