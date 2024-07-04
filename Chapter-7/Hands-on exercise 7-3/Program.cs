using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Kernel = Microsoft.SemanticKernel.Kernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

//Define variables to hold API endpoint, deployment name, and api key
var azureEndpoint = "https://jsopenai-east.openai.azure.com/";
var model = "gpt-4o";
var apiKey = "75b6983f87db4926ab950aa947fba9c3";

// Inject your logger 
// see Microsoft.Extensions.Logging.ILogger @ https://learn.microsoft.com/dotnet/core/extensions/logging
ILoggerFactory myLoggerFactory = NullLoggerFactory.Instance;

var builder = Kernel.CreateBuilder();
builder.Services.AddSingleton(myLoggerFactory);

builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);

var kernel = builder.Build();

// MarketingEmail directory path
var MarketingEmailPluginDirectoryPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "prompt_template_samples", "EmailPlugin");
var InventoryPluginDirectoryPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "prompt_template_samples", "InventoryPlugins");

// Load the MarketingEmailplugin from the Plugins Directory
var MarketingEmailPluginFunctions = kernel.ImportPluginFromPromptDirectory(MarketingEmailPluginDirectoryPath);
var InventoryPluginFunctions = kernel.ImportPluginFromPromptDirectory(InventoryPluginDirectoryPath);

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// 2. Enable automatic function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

//create a cart using GUID for this session
var cartId = Guid.NewGuid().ToString();

var history = new ChatHistory();

//Add this cartID to this sesson as part of the history
history.AddUserMessage(cartId);

string? userInput;
do {
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput))
    {
        // Leaves if the user hit enter without typing any word
        break;
    } 

    // Add user input
    history.AddUserMessage(userInput);

    // 3. Get the response from the AI with automatic function calling
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);

