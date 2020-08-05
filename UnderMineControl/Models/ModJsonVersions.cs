using System.Runtime.Serialization;

namespace UnderMineControl.Models
{
    [DataContract]
    public class ModJsonVersions
    {
        [DataMember]
        public string Mod { get; set; }
        [DataMember]
        public string Game { get; set; }
        [DataMember]
        public string Api { get; set; }
    }
}
