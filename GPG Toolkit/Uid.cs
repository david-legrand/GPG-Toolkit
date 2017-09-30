using System;

namespace GPGToolkit
{
    public class Uid
    {
        public string trustLevel { get; set; }
        public bool revoked { get; set; }
        public DateTime creationDate { get; set; }
        public string fingerprint { get; set; }
        public string id { get; set; }
        public string email { get; set; }
    }
}
