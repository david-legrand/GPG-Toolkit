using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GPGToolkit
{
    public partial class GpgManager
    {
        private string outPath = Path.GetTempPath() + @"\GPGSymmetricMT";
        private int nbFiles = 0, i = 0;

        public float progress { get => GetProgress(); }
        public void ShowOutputFiles() { Process.Start(outPath); }

        public int LaunchProcessing(string sourcePath, string password, bool isEncryption, int threadsToUse)
        {
            Object thisLock = new Object();
            int result = 0;

            if (Directory.Exists(sourcePath))
            {
                CleanDir();
                string[] fileEntries = Directory.GetFiles(sourcePath);
                nbFiles = fileEntries.Length;

                Stopwatch watch = Stopwatch.StartNew();

                Parallel.ForEach(fileEntries, new ParallelOptions
                {
                    MaxDegreeOfParallelism = threadsToUse
                }, (f) =>
                {
                    string doneFile = string.Empty;
                    if (isEncryption) doneFile = string.Format(@"{0}\{1}.gpg",
                        outPath,
                        Path.GetFileName(f));
                    else doneFile = string.Format(@"{0}\{1}",
                        outPath,
                        Path.GetFileName(f).Remove(Path.GetFileName(f).LastIndexOf('.'), 4));

                    while (!File.Exists(doneFile))
                    {
                        FileSymmetric(f, doneFile, password, isEncryption);
                    }

                    Interlocked.Increment(ref i);
                });

                watch.Stop();
                result = (int)watch.ElapsedMilliseconds / 1000;
                i = 0;
            }

            return result;
        }

        private void FileSymmetric(string fileName, string fileEncName, string password, bool isEncryption)
        {
            string cryptAction = isEncryption ? "c" : "d";
            string args = String.Format("--yes --batch --output \"{0}\" --passphrase \"{1}\" -{2} \"{3}\"",
                fileEncName,
                password,
                cryptAction,
                fileName);

            gpgExecute(args);
        }

        private void CleanDir()
        {
            if (Directory.Exists(outPath)) Directory.Delete(outPath, true);
            while (Directory.Exists(outPath)) { };
            Directory.CreateDirectory(outPath);
        }

        private float GetProgress()
        {
            float result = (float)i / (float)nbFiles;
            return result;
        }
    }
}
