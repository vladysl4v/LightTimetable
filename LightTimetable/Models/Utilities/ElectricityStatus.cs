using System.Text;
using System.Collections.Generic;

using LightTimetable.Tools;


namespace LightTimetable.Models.Utilities
{
    public readonly struct ElectricityStatus
    {
        public string ImagePath { get; }
        public string ToolTipText { get; }
        
        public ElectricityStatus(List<(TimeInterval Time, string Type)> currentOutage)
        {
            if (currentOutage == null)
                return;
            var textBuilder = new StringBuilder();
            
            textBuilder.Append("Відключення світла:");
            ImagePath = "/Assets/DataGrid/MaybeElectricity.png";

            foreach (var outage in currentOutage)
            {
                if (outage.Type == "DEFINITE_OUTAGE")
                {
                    ImagePath = "/Assets/DataGrid/NoElectricity.png";
                    textBuilder.Append($"\n{outage.Time.ToString()} - електроенергії не буде");
                }
                else
                {
                    textBuilder.Append($"\n{outage.Time.ToString()} - можливе відключення");
                }   
            }

            ToolTipText = textBuilder.ToString();
        }
    }
}
