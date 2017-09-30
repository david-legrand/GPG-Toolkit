using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GPGToolkit
{
    public partial class GpgManager
    {

        private string _gpgPath, _version = string.Empty;
        private Boolean _initialized = false;

        public Boolean initialized { get => _initialized; }
        public string version { get => _version; }
        public string path { get => DetectPath(); }

        private string DetectPath()
        {
            List<string> defautBinaries = new List<string>() {
                    @"\GnuPG\bin\gpg2.exe",
                    @"\GNU\GnuPG\gpg2.exe",
                    @"\GnuPG\bin\gpg.exe",
                    @"\GNU\GnuPG\gpg.exe" };

            string gpgPath = string.Empty;
            gpgPath = Environment.GetEnvironmentVariable("PROGRAMFILES")
                    + defautBinaries.First(b => File.Exists(Environment.GetEnvironmentVariable("PROGRAMFILES") + b));

            return gpgPath;
        }

        public void Init(string path)
        {
            _gpgPath = string.IsNullOrEmpty(path) ? DetectPath() : path;
            if (!File.Exists(_gpgPath)) throw new GpgException("GPG not found");

            try { _version = gpgExecute("--version").Split('\n')[0]; }
            catch { throw new GpgException("GPG not working"); }

            if (!_version.StartsWith("gpg (GnuPG)")) throw new GpgException("GPG version not found");

            _initialized = true;
        }

        internal string gpgExecute(string arguments)
        {
            string output = string.Empty;
            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.FileName = _gpgPath;
                    p.StartInfo.Arguments = arguments;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                }
            }
            catch { throw new GpgCommandException("GPG command halted"); }

            return output;
        }
    }

    public class GpgException : Exception
    {
        public GpgException() : base() { }
        public GpgException(string message) : base(message) { }
    }

    public class GpgCommandException : Exception
    {
        public GpgCommandException() : base() { }
        public GpgCommandException(string message) : base(message) { }
    }
}
