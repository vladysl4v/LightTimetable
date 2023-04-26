namespace LightTimetable.Models.Utilities
{
    public struct ElectricityStatus
    {
        public string ImagePath { get; }
        public string ToolTipText { get; }

        public ElectricityStatus(string text, bool definitelyBlackout)
        {
            ToolTipText = text;
            ImagePath = definitelyBlackout ? "/Assets/DataGrid/NoElectricity.png" : "/Assets/DataGrid/MaybeElectricity.png";
        }
    }
}
