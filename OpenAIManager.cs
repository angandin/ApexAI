using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using static System.Net.Mime.MediaTypeNames;

namespace ApexAI
{
    public class OpenAIManager
    {
        private readonly Kernel _kernel;
        private readonly string _deploymentName;
        private readonly string _apiKey;
        private readonly string _endpoint;

        public OpenAIManager(IConfiguration config)
        {
            _deploymentName = config["OpenAIModelName"];
            _apiKey = config["OpenAIKey"];
            _endpoint = config["OpenAIEndpoint"];

            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                _deploymentName,
                _endpoint,
                _apiKey
                //serviceId: "Azure_curie"                // alias used in the prompt templates' config.json
                );

            _kernel = builder.Build();
        }

        public async Task<string> GetCompletion(string prompt)
        {
            var response = await _kernel.InvokePromptAsync(prompt);
            //Console.WriteLine(response);
            return response.ToString();
        }

        public async Task<string> GetCompletionV2(string prompt)
        {
            var chat = _kernel.GetRequiredService<IChatCompletionService>();
            var history = new ChatHistory();

            history.AddSystemMessage("You are a useful assistant that replies using a direct style");
            var imageContent = new ImageContent();
            imageContent.Uri = new Uri("https://raw.githubusercontent.com/elbruno/gpt4o-labs-csharp/main/src/GPT4o_AOAI_lab01/imgs/foggyday.png");

            var collectionItems = new ChatMessageContentItemCollection
            {
                new TextContent(prompt),
                imageContent
            };

            history.AddUserMessage(collectionItems);
            var result = await chat.GetChatMessageContentsAsync(history);
            return result[^1].Content;
        }
    }
}
