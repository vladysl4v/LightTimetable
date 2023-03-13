namespace LightTimetable.Models.Electricity
{
    public struct ElecticityStatus
    {
        public string ImagePath { get; }
        public string ToolTipText { get; }

        public ElecticityStatus(string text, bool definitelyBlackout)
        {
            ToolTipText = text;
            ImagePath = definitelyBlackout ? "/Assets/NoElectricity.png" : "/Assets/MaybeElectricity.png";
        }
    }
}
