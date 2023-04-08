using LightTimetable.Tools;


namespace LightTimetable.Models
{
    public class TimetableStatusControl
    {
        public string? IconPath { get; private set; }
        public string ToolTip { get; private set; }

        private TimetableStatus _type;

        public TimetableStatusControl(TimetableStatus value)
        {
            Type = value;
        }

        public TimetableStatus Type
        {
            get => _type;
            set
            {
                _type = value;
                SetupProperties(value);
            }
        }

        private void SetupProperties(TimetableStatus status)
        {
            switch (status)
            {
                case TimetableStatus.Default:
                {
                    IconPath = null;
                    ToolTip = "";
                    break;
                }
                case TimetableStatus.LoadingData:
                {
                    IconPath = "../Assets/Status/Loading.png";
                    ToolTip = "Дані розкладу завантажуються.";
                    break;
                }
                case TimetableStatus.RiggedScheduleShown:
                {
                    IconPath = "../Assets/Status/Warning.png";
                    ToolTip = "Відображається не справжній розклад, а згенерований на основі минулих тижнів.";
                    break;
                }
                case TimetableStatus.DataLoadingError:
                {
                    IconPath = "../Assets/Status/Warning.png";
                    ToolTip = "Помилка завантаження розкладу.";
                    break;
                }
            }
        }

    }
}
