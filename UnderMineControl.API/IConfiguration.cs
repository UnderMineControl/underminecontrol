namespace UnderMineControl.API
{
    public interface IConfiguration
    {
        T Get<T>(string filename = null);

        bool Set<T>(T data, string filename = null);
    }
}
