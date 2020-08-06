namespace UnderMineControl.API.Models
{
    public interface IVersion
    {
        int Major { get; }
        int Minor { get; }
        int Patch { get; }
        int Revision { get; }
    }
}
