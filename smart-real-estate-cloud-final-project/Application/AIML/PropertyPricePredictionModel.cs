using Microsoft.ML;

namespace Application.AIML
{
    public class PropertyPricePredictionModel
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public PropertyPricePredictionModel()
        {
            _mlContext = new MLContext(seed: 0);
        }
        
        public void Train(string dataPath)
        {
            // Define the data schema (using PropertyData)
                var data = _mlContext.Data.LoadFromTextFile<PropertyData>(
                    path: dataPath,
                    hasHeader: true,
                    separatorChar: ',');

            // Split data into training and testing partitions
            var splits = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            // Data transformation pipeline:
            // 1. Convert City and Zone (categorical/text) to categorical numeric keys.
            // 2. One-hot encode these keys to create indicator columns.
            // 3. Concatenate all features (City, Zone, Rooms, Size) into one feature vector.
            var dataProcessPipeline = _mlContext.Transforms.Categorical.OneHotEncoding(
                                         new[]
                                         {
                                             new InputOutputColumnPair("CityEncoded", nameof(PropertyData.City)),
                                             new InputOutputColumnPair("ZoneEncoded", nameof(PropertyData.Location))
                                         })
                                       .Append(_mlContext.Transforms.Concatenate("Features", "CityEncoded", "ZoneEncoded", nameof(PropertyData.RoomsNr), nameof(PropertyData.Surface)))
                                       .Append(_mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(PropertyData.Price)));

            // Choose a regression trainer. SdcaRegression is a good start.
            // You could also try other regressors like FastTree, FastForest, or LightGbm to see if you get better results.
            var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features", maximumNumberOfIterations: 100);
            var trainingPipeline = dataProcessPipeline.Append(trainer); 

            // Train the model
            _model = trainingPipeline.Fit(splits.TrainSet);

            // Evaluate the model on the test set
            var predictions = _model.Transform(splits.TestSet);
            var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

            // Log or output evaluation metrics
            Console.WriteLine($"R^2: {metrics.RSquared}");
            Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError}");
            Console.WriteLine($"Mean Absolute Error: {metrics.MeanAbsoluteError}");
            Console.WriteLine($"Mean Squared Error: {metrics.MeanSquaredError}");
        }

        /// <summary>
        /// Predict the price for a given property.
        /// </summary>
        public float Predict(PropertyData input)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<PropertyData, PropertyDataPrediction>(_model);
            var prediction = predictionEngine.Predict(input);
            return prediction.PredictedPrice;
        }

        /// <summary>
        /// Save the trained model to disk.
        /// </summary>
        public void SaveModel(string modelPath)
        {
            _mlContext.Model.Save(_model, null, modelPath);
        }

        /// <summary>
        /// Load a previously saved model from disk.
        /// </summary>
        public void LoadModel(string modelPath)
        {
            _model = _mlContext.Model.Load(modelPath, out var modelInputSchema);
        }
    }
}
