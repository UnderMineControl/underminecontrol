namespace UnderMineControl.Twitch
{
    public class TwitchCreds
    {
        public string Username { get; set; }
        public string OAuthToken { get; set; }
        public string ClientId { get; set; }
        public char CommandCharacter { get; set; }

        public string[] Channels { get; set; }
    }
}
