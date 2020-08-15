namespace UnderMineControl.Twitch
{
    public enum FilterType
    {
        Default = 0,
        Users = 1,
        Subscribers = 2,
        Moderators = 4,
        Broadcaster = 8,
        Custom = 16,

        All = Users | Subscribers | Moderators | Broadcaster
    }
}
