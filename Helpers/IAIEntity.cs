namespace GptTurboDemo.Helpers
{
    public interface IAIEntity<T>
    {
        Task<List<T>?> GetEntity(string input);
    }
}