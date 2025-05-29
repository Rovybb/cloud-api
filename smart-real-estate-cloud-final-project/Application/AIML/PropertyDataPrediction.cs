using Microsoft.ML.Data;

namespace Application.AIML
{
    public class PropertyDataPrediction
    {
        [ColumnName("Score")]
        public float PredictedPrice { get; set; }
    }
}
