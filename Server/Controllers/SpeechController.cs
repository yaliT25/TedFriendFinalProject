using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using BlazorGoogleLogin.Shared.DTOs;

namespace BlazorGoogleLogin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private readonly IConfiguration _config;

        public SpeechController(IConfiguration config)
        {
            _config = config;
            _client = new HttpClient();
            _apiKey = config.GetValue<string>("OpenAI:Key");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        // פונקציית התמלול (Whisper)
        [HttpPost("transcribe")]
        public async Task<IActionResult> Transcribe(IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0) return BadRequest("No file");

            var model = _config.GetValue<string>("OpenAI:WhisperModel") ?? "whisper-1";

            using var content = new MultipartFormDataContent();
            using var stream = audioFile.OpenReadStream();
            content.Add(new StreamContent(stream), "file", audioFile.FileName);
            content.Add(new StringContent(model), "model");

            var response = await _client.PostAsync("https://api.openai.com/v1/audio/transcriptions", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<WhisperResponse>();
                return Ok(result);
            }
            return BadRequest("Error in Transcription");
        }

        // פונקציית ההקראה (TTS)
        [HttpPost("speak")]
        public async Task<IActionResult> Speak([FromBody] TTSRequest request)
        {
            if (string.IsNullOrEmpty(request.Text)) return BadRequest("No text");

            var model = _config.GetValue<string>("OpenAI:TTSModel") ?? "tts-1";

            var requestBody = new
            {
                model = model,
                input = request.Text,
                voice = "echo"
            };

            var response = await _client.PostAsJsonAsync("https://api.openai.com/v1/audio/speech", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var audioStream = await response.Content.ReadAsStreamAsync();
                return File(audioStream, "audio/mpeg", "speech.mp3");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest($"OpenAI Error: {errorContent}");
            }
        }
    }
}