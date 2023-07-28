using System.Linq;
using System.Text;
using System.Collections.Generic;



namespace LightTimetable.Common
{
    public class OutagesContainer : List<SpecificOutage>
    {
        public string Information { get; set; }
        public string Image { get; set; }

        public OutagesContainer(IEnumerable<SpecificOutage> list)
        {
            this.AddRange(list);
            Image = ConfigureImage();
            Information = CreateString();
        }

        private string ConfigureImage()
        {
            if (this.Any(x => x.Type == OutageType.Definite))
            {
                return "/Assets/DataGrid/NoElectricity.png";
            }
            return "/Assets/DataGrid/MaybeElectricity.png";
        }

        private string CreateString()
        {
            var sbuilder = new StringBuilder();
            sbuilder.Append("Вiдключення свiтла:");
            foreach (var specOutage in this)
            {
                sbuilder.Append("\n" + specOutage);
            }
            return sbuilder.ToString();
        }
    }
}