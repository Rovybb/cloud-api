using Application.AIML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyPricePredictionController : ControllerBase
    {
        private readonly PropertyPricePredictionModel _predictionModel;

        public PropertyPricePredictionController(PropertyPricePredictionModel predictionModel)
        {
            _predictionModel = predictionModel;
        }

        [HttpPost("predict")]
        public ActionResult<float> PredictPrice([FromBody] PropertyData propertyData)
        {
            if (propertyData == null)
            {
                return BadRequest("Invalid property data.");
            }

            float predictedPrice = _predictionModel.Predict(propertyData);
            Console.WriteLine($"Predicted price: {predictedPrice}");
            return Ok(predictedPrice);
        }
    }
}
