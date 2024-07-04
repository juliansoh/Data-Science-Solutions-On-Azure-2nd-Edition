// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System.Text.Json.Serialization;

public class LightModel
{
   [JsonPropertyName("id")]
   public int Id { get; set; }

   [JsonPropertyName("name")]
   public string? Name { get; set; }

   [JsonPropertyName("is_on")]
   public bool? IsOn { get; set; }

   [JsonPropertyName("brightness")]
   public enum? Brightness { get; set; }

   [JsonPropertyName("color")]
   [Description("The color of the light with a hex code (ensure you include the # symbol)")]
   public string? Color { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Brightness
{
   Low,
   Medium,
   High
}

//Define variables to hold API endpoint, deployment name, and api key
var azureEndpoint = "https://jsopenai-east.openai.azure.com/";
var model = "gpt-35";
var apiKey = "75b6983f87db4926ab950aa947fba9c3";

// 1. Create the kernel with the Lights plugin
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
//builder.Plugins.AddFromType<LightsPlugin>("Lights");

Kernel kernel = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// 2. Enable automatic function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var history = new ChatHistory();

string? userInput;
do {
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();

    // Add user input
    history.AddUserMessage(userInput);

    // 3. Get the response from the AI with automatic function calling
    //var result = await chatCompletionService.GetChatMessageContentAsync(
    //    history,
    //    executionSettings: openAIPromptExecutionSettings,
     //   kernel: kernel);

    var result = await kernel.InvokePromptAsync(prompt, new KernelArguments(settings));

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);

//Functions
static string GetCurrentTime()
{
	return DateTime.UtcNow.ToString();
}

public class LightsPlugin
{
   private readonly List<LightModel> _lights;

   public LightsPlugin(LoggerFactory loggerFactory, List<LightModel> lights)
   {
      _lights = lights;
   }

   [KernelFunction("get_lights")]
   [Description("Gets a list of lights and their current state")]
   [return: Description("An array of lights")]
   public async Task<List<LightModel>> GetLightsAsync()
   {
      return _lights;
   }

   [KernelFunction("change_state")]
   [Description("Changes the state of the light")]
   [return: Description("The updated state of the light; will return null if the light does not exist")]
   public async Task<LightModel?> ChangeStateAsync(LightModel changeState)
   {
      // Find the light to change
      var light = _lights.FirstOrDefault(l => l.Id == changeState.Id);

      // If the light does not exist, return null
      if (light == null)
      {
         return null;
      }

      // Update the light state
      light.IsOn = changeState.IsOn;
      light.Brightness = changeState.Brightness;
      light.Color = changeState.Color;

      return light;
   }
}

