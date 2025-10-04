using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AICalorieCalculator.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FoodController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FoodController> _logger;
    private readonly string _geminiApiKey = ""; // Replace with your Gemini API key

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

        var client = _httpClientFactory.CreateClient();
        var geminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-vision:generateContent";
        var imageData = request.ImageBase64.Split(",")[1]; // Remove data:image/jpeg;base64,

        var prompt = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new { text = "Analyze this food image and tell me what food items you see. Also estimate calories and macronutrients (protein, carbs, fat) per serving." },
                        new { inline_data = new { mime_type = "image/jpeg", data = imageData } }
                    }
                }
            }
        };

        client.DefaultRequestHeaders.Add("x-goog-api-key", _geminiApiKey);
        var geminiResponse = await client.PostAsync(
            geminiUrl,
            new StringContent(JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json")
        );
        var geminiBody = await geminiResponse.Content.ReadAsStringAsync();
        _logger.LogInformation("Gemini API response: {GeminiBody}", geminiBody);

        try
        {
            var json = JsonDocument.Parse(geminiBody);
            var text = json.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
            return Ok(new { result = text });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Gemini response");
            return Ok(new { result = "Could not analyze the food image." });
        }
    }
}

public class FoodAnalyzeRequest
{
    public string ImageBase64 { get; set; } = string.Empty;
}
