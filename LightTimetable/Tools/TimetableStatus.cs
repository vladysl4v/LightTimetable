namespace LightTimetable.Tools
{
    public readonly struct TimetableStatus
    {
        public readonly string? IconPath { get; }
        public readonly string? ToolTip { get; }
        
        private TimetableStatus(string? iconPath, string? toolTip)
        {
            IconPath = iconPath;
            ToolTip = toolTip;
        }
        
        public static TimetableStatus Default => new TimetableStatus(null, null);
        public static TimetableStatus RiggedScheduleShown => new TimetableStatus("../Assets/Status/Warning.png", "Відображається не справжній розклад, а згенерований на основі минулих тижнів.");
        public static TimetableStatus LoadingData => new TimetableStatus("../Assets/Status/Loading.png", "Дані розкладу завантажуються.");
        public static TimetableStatus DataLoadingError => new TimetableStatus("../Assets/Status/Warning.png", "Помилка завантаження розкладу.");
    }
}
