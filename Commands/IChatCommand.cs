namespace GptTurboDemo.Commands
{
    public interface IChatCommand
    {
        Task<int> HandleCommand(string name);
    }
}