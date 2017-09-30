using System;
using System.IO;

namespace GPGToolkit
{
    public partial class GpgManager
    {
        private static string tmpFile = Path.GetTempPath() + "gpgtemp.txt";
        private static string encTmpFile = tmpFile + ".asc";

        public string StringEncrypt(string message, string recipient)
        {
            string encoded = "";
            Clean();

            File.WriteAllText(tmpFile, message);
            gpgExecute("--trust-model always --armor -e -r " + recipient + " " + tmpFile);

            if (File.Exists(encTmpFile))
            {
                encoded = File.ReadAllText(encTmpFile);
                Clean();
            }

            return encoded;
        }

        public string StringDecrypt(string message)
        {
            string decoded = "";
            Clean();

            File.WriteAllText(encTmpFile, message);
            gpgExecute("--output " + tmpFile + " -d " + encTmpFile);

            if (File.Exists(tmpFile))
            {
                decoded = File.ReadAllText(tmpFile);
                Clean();
            }

            return decoded;
        }

        public void Clean()
        {
            if (File.Exists(encTmpFile)) File.Delete(encTmpFile);
            if (File.Exists(tmpFile)) File.Delete(tmpFile);
            GC.Collect();
        }

    }
}
