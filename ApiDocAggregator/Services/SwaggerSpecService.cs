namespace ApiDocAggregator.Services
{
    public class SwaggerSpecService
    {
        private readonly HttpClient _httpClient;
        private readonly string[] _apiUrls = {
        "https://localhost:7078/swagger/v1/swagger.json", // Project 1
        "https://localhost:44303/swagger/v1/swagger.json"  // Project 2
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

            foreach (var apiUrl in _apiUrls)
            {
                Console.WriteLine($"Downloading Swagger from {apiUrl}");
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiName = apiUrl.Contains("7078") ? "Project1" : "Project2";
                    var version = "v1";
                    var fileName = $"{apiName}_{version}.json";
                    var filePath = Path.Combine(folderPath, fileName);
                    await File.WriteAllTextAsync(filePath, content);
                    Console.WriteLine($"Successfully Downloaded Swagger from {apiUrl}");
                }
                else
                {
                    Console.WriteLine($"Failed to download Swagger from {apiUrl}. Status: {response.StatusCode}");
                }
            }
        }
    }
}
