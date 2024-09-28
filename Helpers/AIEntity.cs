using GptTurboDemo.Options;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Text.Json;

namespace GptTurboDemo.Helpers
{
    public class AIEntity<T> : IAIEntity<T> where T : new()
    {
        readonly OpenAIAPI _openAIApi;
        public AIEntity(IOptions<OpenAIOptions> openAIOptions)
        {
            _openAIApi = new OpenAIAPI(openAIOptions.Value.OpenAIKey);
        }
        public async Task<List<T>?> GetEntity(string input)
        {
            T entity = new();
            var format = JsonSerializer.Serialize(entity);
            var entities = new List<T>();

            ChatRequest chatRequest = new ChatRequest()
            {
                Model = new OpenAI_API.Models.Model { ModelID = "gpt-4-turbo" },
                Temperature = 0.0,
                MaxTokens = 500,
                ResponseFormat = ChatRequest.ResponseFormats.JsonObject,
                Messages = new ChatMessage[] {
                    new ChatMessage(ChatMessageRole.System, $"You are a helpful assistant designed to output JSON. in this format {format}"),
                    new ChatMessage(ChatMessageRole.User, input)
                }
            };

            var results = await _openAIApi.Chat.CreateChatCompletionAsync(chatRequest);

            if (results != null && results.Choices.Any())
            {
                foreach (var result in results.Choices)
                {
                    if (!string.IsNullOrEmpty(result?.Message?.TextContent))
                    {
                        entities.Add(JsonSerializer.Deserialize<T>(result.Message.TextContent));
                    }
                }

                return entities;
            }

            return default;
        }
    }
}
