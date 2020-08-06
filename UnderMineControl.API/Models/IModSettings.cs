namespace UnderMineControl.API.Models
{
    public interface IModSettings
    {
        string Name { get; }
        string TagLine { get; }
        string Author { get; }
        string Url { get; }
        string[] EntryFiles { get; }
    }
}
