namespace FileZipper.Utils
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    public static class Helper
    {
        public static string GetFolderName(string path)
        {
            var dirName = new DirectoryInfo(path).Name;

            return dirName;
        }

        public static string SelectFolder()
        {
            var targetFolder = new FolderBrowserDialog();
            if (targetFolder.ShowDialog() != DialogResult.OK)
            {
                Environment.Exit(0);
            }

            return targetFolder.SelectedPath;
        }

        public static string ToAlphaNumberc(string text)
        {
            var res = new string(
                text
                    .Where(ch => char.IsLetterOrDigit(ch) ||
                                 char.IsWhiteSpace(ch) ||
                                 ch == '_' ||
                                 ch == '-').ToArray());

            res = res.Replace(' ', '_').ToLower();

            return res;
        }

        public static string Md5Encode(string text)
        {
            var encodedPassword = new UTF8Encoding().GetBytes(text);
            var hash = ((HashAlgorithm)CryptoConfig
                .CreateFromName("MD5"))
                .ComputeHash(encodedPassword);

            var md5 = BitConverter.ToString(hash)
               .Replace("-", string.Empty)
               .ToLower();

            return md5;
        }
    }
}