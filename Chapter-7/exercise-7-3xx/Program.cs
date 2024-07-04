// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Kernel = Microsoft.SemanticKernel.Kernel;

//Define plugins directory
var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(),"prompt_template_samples", "InventoryPlugins");
//Define variables to hold API endpoint, deployment name, and api key
var azureEndpoint = "https://jsopenai-east.openai.azure.com/";
var model = "gpt-35";
var apiKey = "75b6983f87db4926ab950aa947fba9c3";

// 1. Create the kernel with the plugins in pluginsDirectory
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
//builder.Plugins.AddFromType<LightsPlugin>("Lights");
Kernel kernel = builder.Build();

var InventoryPluginFunctions = kernel.ImportPluginFromPromptDirectory(pluginsDirectory);

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
    //    kernel: kernel);
    var result = await kernel.InvokeAsync(InventoryPluginFunctions[userInput]);

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);