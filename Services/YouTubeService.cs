namespace DesafioBackEndRDManipulacao.Services
{
    // Services/YouTubeService.cs
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using DesafioBackEndRDManipulacao.Models;
    using Microsoft.Extensions.Configuration; //Para usar o IConfiguration
    using DesafioBackEndRDManipulacao.Models;

    public class YouTubeService
    {

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public YouTubeService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = Environment.GetEnvironmentVariable("YouTubeApiKey");
       
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("YouTube API Key not found.");
            }
            _httpClient.BaseAddress = new Uri("https://www.googleapis.com/youtube/v3/");

        }

        public async Task<List<Video>> SearchVideosAsync()
        {
            // Construir a URL da requisição.
            var requestUrl = $"search?part=snippet&type=video&q=manipulação+medicamentos&regionCode=BR&publishedAfter=2025-01-01T00:00:00Z&key={_apiKey}";
            
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode(); 

            var content = await response.Content.ReadAsStringAsync();

            
            using (JsonDocument doc = JsonDocument.Parse(content))
            {
                JsonElement root = doc.RootElement;
                JsonElement items = root.GetProperty("items");

                List<Video> videos = new List<Video>();
                foreach (JsonElement item in items.EnumerateArray())
                {
                    
                    if (item.TryGetProperty("id", out JsonElement idElement) && idElement.TryGetProperty("videoId", out JsonElement videoIdElement))
                    {
                        
                        if (item.TryGetProperty("snippet", out JsonElement snippetElement))
                        {
                            
                            Video video = new Video
                            {
                                VideoId = videoIdElement.GetString(),
                                Titulo = snippetElement.GetProperty("title").GetString(),
                                Descricao = snippetElement.GetProperty("description").GetString(),
                                Autor = snippetElement.GetProperty("channelTitle").GetString(),
                                
                                DataPublicacao = snippetElement.TryGetProperty("publishedAt", out JsonElement publishedAtElement)
                                    ? publishedAtElement.GetDateTime()
                                    : DateTime.MinValue, 
                                                         
                                Duracao = await GetVideoDuration(videoIdElement.GetString())
                            };
                            videos.Add(video);
                        }
                    }
                }
                return videos;
            }
        }
        
        private async Task<string> GetVideoDuration(string videoId)
        {
            var detailsUrl = $"videos?part=contentDetails&id={videoId}&key={_apiKey}";
            var detailsResponse = await _httpClient.GetAsync(detailsUrl);
            detailsResponse.EnsureSuccessStatusCode();
            var detailsContent = await detailsResponse.Content.ReadAsStringAsync();

            using (JsonDocument detailsDoc = JsonDocument.Parse(detailsContent))
            {
                JsonElement detailsRoot = detailsDoc.RootElement;
                if (detailsRoot.TryGetProperty("items", out JsonElement itemsElement) && itemsElement.EnumerateArray().Any()) 
                {
                    JsonElement firstItem = itemsElement.EnumerateArray().First(); 
                    if (firstItem.TryGetProperty("contentDetails", out JsonElement contentDetailsElement))
                    {
                        if (contentDetailsElement.TryGetProperty("duration", out JsonElement durationElement))
                        {
                            return durationElement.GetString();
                        }
                    }
                }
            }
            return string.Empty; //Ou null, dependendo de como quer tratar.
        }
    }
}
