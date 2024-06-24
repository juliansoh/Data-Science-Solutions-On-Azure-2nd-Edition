// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = Kernel.CreateBuilder();

builder.AddAzureOpenAIChatCompletion(
         "gpt-35",                      // Azure OpenAI Deployment Name
         "https://jsopenai-east.openai.azure.com/", // Azure OpenAI Endpoint
         "75b6983f87db4926ab950aa947fba9c3");      // Azure OpenAI Key

// Alternative using OpenAI
//builder.AddOpenAIChatCompletion(
//         "gpt-3.5-turbo",                  // OpenAI Model name
//         "...your OpenAI API Key...");     // OpenAI API Key

var kernel = builder.Build();

var prompt = @"{{$input}}

Draft an email to this person detailing a special sales event in July  with up to 50% off on cycling gear. Include disclaimer in the footnote of the email that they are receiving this email because they opted into our mailing list.";


var createemail = kernel.CreateFunctionFromPrompt(prompt, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 400 });

Console.WriteLine("Type the name of the person you want to send the email to:");
var name = Console.ReadLine();

Console.WriteLine(await kernel.InvokeAsync(createemail, new() { ["input"] = name }));

