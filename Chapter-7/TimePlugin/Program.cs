﻿// Copyright (c) Microsoft. All rights reserved.
#pragma warning disable VSTHRD111 // Use ConfigureAwait(bool)
#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task

using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

//Define variables to hold API endpoint, deployment name, and api key
var azureEndpoint = "https://jsopenai-east.openai.azure.com/";
var model = "gpt-4-0125";
var apiKey = "75b6983f87db4926ab950aa947fba9c3";

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build()
    ?? throw new InvalidOperationException("Configuration is not provided.");

ArgumentNullException.ThrowIfNull(config["OpenAI:ChatModelId"], "OpenAI:ChatModelId");
ArgumentNullException.ThrowIfNull(config["OpenAI:ApiKey"], "OpenAI:ApiKey");

//var kernelBuilder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(
//    modelId: config["OpenAI:ChatModelId"]!,
//    AzureEndpoint: config["OpenAI:AzureEndpoint"]!,
//    apiKey: config["OpenAI:ApiKey"]!);
var kernelBuilder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);

kernelBuilder.Plugins.AddFromType<TimeInformationPlugin>();
kernelBuilder.Plugins.AddFromType<InventoryInformationPlugin>();
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
/// A plugin that returns the current time.
/// </summary>
public class TimeInformationPlugin
{
    /// <summary>
    /// Retrieves the current time in UTC.
    /// </summary>
    /// <returns>The current time in UTC. </returns>
    [KernelFunction, Description("Retrieves the current time in UTC.")]
    public string GetCurrentUtcTime()
        => DateTime.UtcNow.ToString("R");
}

public class InventoryInformationPlugin
{
    /// <summary>
    /// Retrieves the current time in UTC.
    /// </summary>
    /// <returns>The current time in UTC. </returns>
    [KernelFunction, Description("Lists the current inventory.")]
    public string ListInventory()
        => "Inventory: 10 Giro helmet Medium, 5 Giro helmet Large";
}
