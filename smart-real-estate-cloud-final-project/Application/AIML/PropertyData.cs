using Microsoft.ML.Data;

namespace Application.AIML
{
    public class PropertyData
    {
        [LoadColumn(0)]
        public float Price { get; set; }
        [LoadColumn(1)]
        public string City { get; set; }

        [LoadColumn(2)]
        public string Location { get; set; }

        [LoadColumn(3)]
        public float RoomsNr { get; set; }

        [LoadColumn(4)]
        public float Surface { get; set; }
    }
}
