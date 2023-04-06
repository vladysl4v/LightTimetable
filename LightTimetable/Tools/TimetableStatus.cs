
namespace LightTimetable.Tools
{

    public enum TimetableStatus
    {
        Default,
        Loading,
        RiggedScheduleShown,
        ScheduleLoadingError
    }

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

                switch (value)
                {
                    case TimetableStatus.Default:
                    {
                        IconPath = null;
                        ToolTip = "";
                        break;
                    }
                    case TimetableStatus.Loading:
                    {
                        IconPath = "../Assets/Loading.png";
                        ToolTip = "Дані розкладу завантажуються.";
                        break;
                    }
                    case TimetableStatus.RiggedScheduleShown:
                    {
                        IconPath = "../Assets/Warning.png";
                        ToolTip = "Відображається не справжній розклад, а згенерований на основі минулих тижнів.";
                        break;
                    }
                    case TimetableStatus.ScheduleLoadingError:
                    {
                        IconPath = "../Assets/Warning.png";
                        ToolTip = "Помилка завантаження розкладу.";
                        break;
                    }
                }
            }
        }

    }
}
