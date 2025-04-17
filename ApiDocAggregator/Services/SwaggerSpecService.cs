using System.Text.Json;

namespace ApiDocAggregator.Services
{
    public class SwaggerSpecService
    {
        private readonly HttpClient _httpClient;

        // Dynamic list of APIs
        private readonly List<ApiInfo> _apiList = new List<ApiInfo>
        {
            new ApiInfo { Name = "Project1",baseUrl = "https://localhost:7078/", Url = "https://localhost:7078/swagger/v1/swagger.json", Version = "v1" },
            new ApiInfo { Name = "Project2",baseUrl = "https://localhost:7275/", Url = "https://localhost:7275/swagger/v1/swagger.json", Version = "v1" },
            // Add more projects as needed
        };

        public SwaggerSpecService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DownloadSwaggerSpecsAsync()
        {
            var folderPath = Path.Combine("wwwroot", "swagger");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            foreach (var api in _apiList)
            {
                Console.WriteLine($"Downloading Swagger from {api.Url}");
                var response = await _httpClient.GetAsync(api.Url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var fileName = $"{api.Name}_{api.Version}.json";
                    var filePath = Path.Combine(folderPath, fileName);

                    // Dynamically modify Swagger JSON
                    var updatedJson = UpdateSwaggerJson(content, api.baseUrl);

                    await File.WriteAllTextAsync(filePath, updatedJson);
                    Console.WriteLine($"Successfully Downloaded and Updated Swagger from {api.Url}");
                }
                else
                {
                    Console.WriteLine($"Failed to download Swagger from {api.Url}. Status: {response.StatusCode}");
                }
            }
        }

        private string UpdateSwaggerJson(string content, string baseUrl)
        {
            //var baseUrl = new Uri(apiUrl).GetLeftPart(UriPartial.Authority);

            // Parse and modify JSON
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            jsonObject["servers"] = new[]
            {
                new { url = baseUrl }
            };

            return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
        }

        // Class to hold API info
        private class ApiInfo
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string baseUrl { get; set; }
            public string Version { get; set; }
        }
    }
}