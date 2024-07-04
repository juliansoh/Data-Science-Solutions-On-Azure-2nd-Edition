// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

//Define variables to hold API endpoint, deployment name, and api key
var azureEndpoint = "https://.../";
var model = "gpt-4o";
var apiKey = "ApiKey";

var kernelBuilder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);

kernelBuilder.Plugins.AddFromType<InformationPlugin>();
var kernel = kernelBuilder.Build();

// Get chat completion service
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Enable auto function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

Console.WriteLine("Ask questions to use the Time Plugin such as:\n" +
                  "- What time is it?");

ChatHistory chatHistory = [];
string? input = null;
while (true)
{
    Console.Write("\nUser > ");
    input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        // Leaves if the user hit enter without typing any word
        break;
    }
    chatHistory.AddUserMessage(input);
    var chatResult = await chatCompletionService.GetChatMessageContentAsync(chatHistory, openAIPromptExecutionSettings, kernel);
    Console.Write($"\nAssistant > {chatResult}\n");
}

/// <summary>
/// A plugin that returns time and inventory information.
/// </summary>
public class InformationPlugin
{
    /// <summary>
    /// Retrieves the current time in UTC.
    /// </summary>
    /// <returns>The current time in UTC. </returns>
    [KernelFunction, Description("Retrieves the current time in UTC.")]
    public string GetCurrentUtcTime()
        => DateTime.UtcNow.ToString("R");

    /// <summary>
    /// Returns the current inventory.
    [KernelFunction, Description("Lists the current inventory.")]
    public string ListInventory()
        => "Inventory: 10 Giro helmet Medium, 5 Giro helmet Large";
}