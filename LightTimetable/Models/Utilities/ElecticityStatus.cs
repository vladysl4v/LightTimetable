namespace LightTimetable.Models.Utilities
{
    public struct ElecticityStatus
    {
        public string ImagePath { get; }
        public string ToolTipText { get; }

        public ElecticityStatus(string text, bool definitelyBlackout)
        {
            ToolTipText = text;
            ImagePath = definitelyBlackout ? "/Assets/DataGrid/NoElectricity.png" : "/Assets/DataGrid/MaybeElectricity.png";
        }
    }
}
