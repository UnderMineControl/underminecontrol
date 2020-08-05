using System.Runtime.Serialization;

namespace UnderMineControl.Models
{
    [DataContract]
    public class ModJson
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string TagLine { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string[] EntryFiles { get; set; }
        [DataMember]
        public ModJsonVersions Versions { get; set; }
    }
}
