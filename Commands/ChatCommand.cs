using GptTurboDemo.Helpers;
using GptTurboDemo.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Json;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace GptTurboDemo.Commands
{
    public class ChatCommand : Command, IChatCommand
    {
        IAIEntity<City> _aiCity;
        public ChatCommand(IServiceProvider serviceProvider)
           : base("question", "Displays data parsed from GptTurbo.")
        {
            using var service = serviceProvider.CreateScope();

            _aiCity = (IAIEntity<City>)service.ServiceProvider.GetService(typeof(IAIEntity<City>));

            var question = new Option<string>("--question")
            {
                Name = "question",
                Description = "Question for GptTurbo.",
                IsRequired = true
            };

            AddOption(question);

            Handler = CommandHandler.Create(
                (string question) => HandleCommand(question));
        }

        public async Task<int> HandleCommand(string question)
        {
            try
            {
                var aiResponse = await _aiCity.GetEntity(question);
                var response = JsonConvert.SerializeObject(aiResponse, Formatting.Indented);
                var display = new JsonText(response);
                AnsiConsole.Write(
                 new Panel(display)
                     .Header("Data from GptTurbo")
                     .Collapse()
                     .RoundedBorder()
                     .BorderColor(Color.Yellow));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }

            return 0;
        }
    }
}
