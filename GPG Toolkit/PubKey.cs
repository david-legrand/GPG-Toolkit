using System;
using System.Collections.Generic;

namespace GPGToolkit
{
    // Détails de la doc sur le listing des clefs via l'option --with-colons :
    // http://git.gnupg.org/cgi-bin/gitweb.cgi?p=gnupg.git;a=blob_plain;f=doc/DETAILS

    public class PubKey
    {
        public string trustLevel { get; set; }
        public bool revoked { get; set; }
        public string bitLength { get; set; }
        public string algorithm { get; set; }
        public string keyID { get; set; }
        public List<Uid> identifiants { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime expiryDate { get; set; }
        public string capabilities { get; set; }
        public string fingerprint { get; set; }

    }
}
