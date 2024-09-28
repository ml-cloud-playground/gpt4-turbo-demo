using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using GptTurboDemo.Commands;
using GptTurboDemo.Helpers;
using GptTurboDemo.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        ServiceProvider serviceProvider = BuildServiceProvider();

        var commandLineBuilder = new CommandLineBuilder();
        var chatCommand = (Command) serviceProvider.GetService<IChatCommand>();
        commandLineBuilder.AddCommand(chatCommand);


        var build = commandLineBuilder.UseDefaults().Build();

        return await build.InvokeAsync(args).ConfigureAwait(false);
    }
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        services.AddSingleton<IConfiguration>(config);
        services.AddSingleton<IChatCommand, ChatCommand>();
        services.AddSingleton(typeof(IAIEntity<>), typeof(AIEntity<>));
        services.AddOptions<OpenAIOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(OpenAIOptions.OpenAI).Bind(settings);
                });

        return services.BuildServiceProvider();
    }
}