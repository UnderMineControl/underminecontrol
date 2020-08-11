namespace UnderMineControl.API.MenuItems
{
    public interface IMenuTextBox : IMenuItem
    {
        string LabelText { get; }
        string StartingText { get; }
        bool MultiLine { get; }
        string Value { get; set; }
    }
}
