using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GPGToolkit
{
    public partial class GpgManager
    {
        public List<PubKey> GetPublicKeys()
        {
            List<PubKey> KeyStore = new List<PubKey>();

            string publicKeys = gpgExecute("-k --with-colons");
            string[] keysArray = publicKeys.Split('\n');

            for (int i = 1; i < keysArray.Length; i++)
            {
                if (keysArray[i].Contains("pub:"))
                {
                    PubKey pk = new PubKey();
                    string[] keyData = keysArray[i].Split(':');

                    if (keyData.Length > 11)
                    {
                        pk.trustLevel = keyData[1];
                        pk.bitLength = keyData[2];
                        pk.algorithm = keyData[3];
                        pk.keyID = keyData[4];
                        pk.revoked = pk.trustLevel.Equals("r");

                        int creationSeconds, expirySeconds;
                        bool csr = Int32.TryParse(keyData[5], out creationSeconds);
                        bool esr = Int32.TryParse(keyData[6], out expirySeconds);
                        if (csr) pk.creationDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().AddSeconds(creationSeconds);
                        if (esr) pk.expiryDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().AddSeconds(expirySeconds);

                        pk.capabilities = Regex.Replace(keyData[11], "[^A-Z]", "");
                        if (i + 1 <= keysArray.Length) pk.fingerprint = keysArray[i + 1].Split(new string[] { ":::::::::" }, StringSplitOptions.None)[1].Split(':')[0];

                        List<Uid> uids = new List<Uid>();
                        int j = i + 2;
                        if (j <= keysArray.Length)
                        {
                            while (keysArray[j].StartsWith("uid") || j == keysArray.Length)
                            {
                                Uid uid = new Uid();
                                string[] uidData = keysArray[j].Split(':');

                                uid.trustLevel = uidData[1];
                                uid.fingerprint = uidData[7];
                                uid.id = uidData[9];
                                uid.revoked = uid.trustLevel.Equals("r");

                                string pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
                                Regex myRegex = new Regex(pattern);
                                string email = myRegex.Match(uidData[9]).ToString();
                                if (email.Length > 0) uid.email = email;

                                int creationSecondsuid;
                                bool csruid = Int32.TryParse(keyData[5], out creationSecondsuid);
                                if (csr) uid.creationDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().AddSeconds(creationSecondsuid);
                                uids.Add(uid);
                                j++;
                            }
                        }
                        pk.identifiants = uids;
                        KeyStore.Add(pk);
                    }
                }
            }

            return KeyStore;
        }
    }
}
