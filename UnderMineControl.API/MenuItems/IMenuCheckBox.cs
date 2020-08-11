namespace UnderMineControl.API.MenuItems
{
    public interface IMenuCheckBox : IMenuItem
    {
        string Text { get; }
        bool Value { get; set; }
    }
}
