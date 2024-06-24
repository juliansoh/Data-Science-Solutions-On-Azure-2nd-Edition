// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

var builder = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
builder.Plugins.AddFromType<TimePlugin>();
builder.Plugins.AddFromPromptDirectory("./../../../Plugins/WriterPlugin");
Kernel kernel = builder.Build();

var currentTime = await kernel.InvokeAsync("TimePlugin", "UtcNow");
Console.WriteLine(currentTime);

var poemResult = await kernel.InvokeAsync("WriterPlugin", "ShortPoem", new()
{
    { "input", currentTime }
});
Console.WriteLine(poemResult);