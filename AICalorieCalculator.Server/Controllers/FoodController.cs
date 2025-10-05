using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AICalorieCalculator.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FoodController> _logger;
        private readonly string _geminiApiKey = ""; // <-- Replace with your Gemini API key

        public FoodController(IHttpClientFactory httpClientFactory, ILogger<FoodController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] FoodAnalyzeRequest request)
        {
            if (string.IsNullOrEmpty(request.ImageBase64))
                return BadRequest("Image is required.");

            try
            {
                var client = _httpClientFactory.CreateClient();
                var geminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

                // Clean the base64 data
                var imageData = request.ImageBase64.Contains(",")
                    ? request.ImageBase64.Split(",")[1]
                    : request.ImageBase64;

                // 🧠 Prompt to get structured nutrition data
                var promptText = @"
Analyze the food image and identify each visible food item. 
Estimate the approximate calorie count per serving and macronutrients in grams (protein, carbs, fat).
Return your answer strictly in the following JSON format only:

{
  ""foods"": [
    {
      ""name"": ""Food item name"",
      ""calories"": number,
      ""protein_g"": number,
      ""carbs_g"": number,
      ""fat_g"": number
    }
  ],
  ""total_calories"": number
}

Do not include any explanation or text outside of JSON.
";

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new
                                {
                                    inline_data = new
                                    {
                                        mime_type = "image/jpeg",
                                        data = imageData
                                    }
                                },
                                new { text = promptText }
                            }
                        }
                    }
                };

                var requestBody = JsonSerializer.Serialize(payload);
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, geminiUrl)
                {
                    Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Add("x-goog-api-key", _geminiApiKey);

                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Gemini API response: {GeminiBody}", responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini API failed: {StatusCode} {Body}", response.StatusCode, responseContent);
                    return StatusCode((int)response.StatusCode, "Gemini API error");
                }

                // Try to extract the text from Gemini response
                string? geminiText = null;
                try
                {
                    var json = JsonDocument.Parse(responseContent);
                    geminiText = json.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    geminiText = geminiText
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing Gemini API response JSON structure");
                    return Ok(new { result = "Invalid Gemini API format." });
                }

                if (string.IsNullOrWhiteSpace(geminiText))
                    return Ok(new { result = "No response from Gemini." });

                // 🧩 Try to parse the JSON Gemini returned
                try
                {
                    var foodResult = JsonSerializer.Deserialize<GeminiFoodResponse>(geminiText,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (foodResult != null)
                        return Ok(foodResult);
                    else
                        return Ok(new { result = geminiText });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing Gemini JSON response");
                    // fallback to raw text
                    return Ok(new { result = geminiText });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
                return StatusCode(500, "Internal server error while analyzing image.");
            }
        }
    }

    public class FoodAnalyzeRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
    }

    public class GeminiFoodResponse
    {
        public List<FoodItem> Foods { get; set; } = new();
        public double Total_Calories { get; set; }
    }

    public class FoodItem
    {
        public string Name { get; set; } = string.Empty;
        public double Calories { get; set; }
        public double Protein_g { get; set; }
        public double Carbs_g { get; set; }
        public double Fat_g { get; set; }
    }
}
