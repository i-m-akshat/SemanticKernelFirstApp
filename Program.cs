using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using OpenAI.Responses;//using this because i want to use gemini insted of open ai or azure open ai its a pre release version

string geminiApiKey = string.Empty;
string modelId = "gemini-1.5-flash";
var _kernelBuilder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
_kernelBuilder.AddGoogleAIGeminiChatCompletion(modelId, geminiApiKey);
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
//_kernelBuilder.Plugins.AddFromType<TimePlugin>();

//using dependency injection
//_kernelBuilder.Services.AddSingleton<TimePlugin>();
//_kernelBuilder.Services.AddSingleton<TextPlugin>();

// Register KernelPluginCollection from the plugins
//_kernelBuilder.Services.AddSingleton<KernelPluginCollection>(sp =>
//{
//    var plugins = new KernelPluginCollection
//    {
//        KernelPluginFactory.CreateFromObject(sp.GetRequiredService<TimePlugin>()),
//        KernelPluginFactory.CreateFromObject(sp.GetRequiredService<TextPlugin>())
//    };
//    return plugins;
//});

// Register the Kernel instance with plugin collection
//_kernelBuilder.Services.AddTransient<Kernel>(sp =>
//{
//    var pluginCollection = sp.GetRequiredService<KernelPluginCollection>();
//    return new Kernel(sp, pluginCollection);
//});

var kernel = _kernelBuilder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
string? UserInput = string.Empty;
#pragma warning disable SKEXP0070
GeminiPromptExecutionSettings geminiAiPromptExecutionSetting = new GeminiPromptExecutionSettings()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    Temperature=1.0,
    
};
#pragma warning restore SKEXP0070
do
{
    Console.Write("User> ");
    UserInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(UserInput)) break;

    history.AddUserMessage(UserInput);

    var result = await chatCompletionService.GetChatMessageContentsAsync(
        history,
        executionSettings: geminiAiPromptExecutionSetting,
        kernel: kernel
    );

    var assistantMessage = result.FirstOrDefault()?.Content ?? "[No response]";
    Console.WriteLine("Assistant> " + assistantMessage);

    history.AddMessage(
        result.FirstOrDefault()?.Role ?? AuthorRole.Assistant,
        assistantMessage
    );

} while (true);
//var result = await kernel.InvokePromptAsync("You are a perfect chatbot. Answer to this question - What is the max token limit ?");
//Console.WriteLine(result);
Console.ReadLine();