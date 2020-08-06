namespace UnderMineControl.API.Models
{
    public interface IMod
    {
        string JsonFilePath { get; }
        IModSettings Data { get; }
        IVersion ModVer { get; }
        IVersion GameVer { get; }
        IVersion ApiVer { get; }
    }
}
