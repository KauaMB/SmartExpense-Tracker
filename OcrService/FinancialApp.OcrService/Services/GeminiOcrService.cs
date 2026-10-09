using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration; // Adicione este using

namespace FinancialApp.OcrService.Services;

public class GeminiOcrService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

    // Injetamos o IConfiguration aqui
    public GeminiOcrService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> ProcessReceiptImageAsync(IFormFile file)
    {
        // Puxa a chave do arquivo de segredos ou variáveis de ambiente
        var apiKey = _configuration["Gemini:ApiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Gemini API Key is missing from configuration.");
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var base64Image = Convert.ToBase64String(memoryStream.ToArray());

        var prompt = @"Analyze this receipt image. 
               Extract the total value (as a number), the merchant name (as text), and the date (YYYY-MM-DD). 
               Also, extract every single product line by line and classify each one into a logical Category (e.g., Alimentação, Limpeza, Higiene, etc.).
               Return ONLY a valid JSON object exactly like this, without Markdown formatting: 
               { 
                 ""MerchantName"": ""Name"", 
                 ""TotalValue"": 0.00, 
                 ""Date"": ""YYYY-MM-DD"",
                 ""Items"": [
                   { ""Name"": ""Product Name"", ""Category"": ""Limpeza"", ""Price"": 1.50 }
                 ]
               }";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new object[] {
                        new { text = prompt },
                        new { inline_data = new { mime_type = file.ContentType, data = base64Image } }
                }}
            }
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // Usa a chave que lemos da configuração
        var response = await _httpClient.PostAsync($"{_apiUrl}?key={apiKey}", jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API Error: {error}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(responseJson);
        var extractedText = document.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return extractedText;
    }
}