﻿namespace FileZipper
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using Ionic.Zip;
    using Ionic.Zlib;
    using IO;
    using Utils;

    public class App
    {
        private readonly Io io;

        private NameValueCollection settings;

        private string sourceDirectory;
        private CompressionLevel compressionLevel;
        private string destinationDirectory;

        public App(Io io)
        {
            this.io = io;
            this.InitializeSettings();
        }

        public void Run()
        {
            this.ZipFolders();
            this.io.Write("All done! - Press any key to exit.", Colors.Green);
            this.io.Read();
        }

        private void ZipFolders()
        {
            var directoriesToZip = Directory.GetDirectories(this.sourceDirectory);
            foreach (var dir in directoriesToZip)
            {
                if (dir == this.destinationDirectory.TrimEnd(Path.DirectorySeparatorChar))
                {
                    continue;
                }

                var folderName = Helper.GetFolderName(dir);
                var md5Name = Helper.ToAlphaNumberc(folderName);

                var zippedFilePath = this.GenerateFilename(md5Name);
                this.ValidateFileDoesNotExist(zippedFilePath);

                this.io.Write($"Zipping: \"{folderName}\"...", Colors.Default, false);

                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(dir, folderName);
                    zip.CompressionLevel = CompressionLevel.BestSpeed;

                    if (bool.Parse(this.settings["IncludeComment"]))
                    {
                        zip.Comment = this.settings["Comment"];
                    }

                    zip.Save(zippedFilePath);
                }

                this.io.Write(" Done!", Colors.Blue);
            }
        }

        private void ValidateFileDoesNotExist(string zippedFilePath)
        {
            if (File.Exists(zippedFilePath))
            {
                File.Delete(zippedFilePath);
            }
        }

        private string GenerateFilename(string folderName)
        {
            return string.Format(
                "{0}{2}{1}.zip", 
                this.destinationDirectory,
                folderName,
                Path.DirectorySeparatorChar);
        }

        private void InitializeSettings()
        {
            this.io.Write("Select source folder");
            this.sourceDirectory = Helper.SelectFolder();

            this.ReadSettingsFile();

            Enum.TryParse(this.settings["CompressionLevel"], out this.compressionLevel);

            if (bool.Parse(this.settings["DefaultDestination"]))
            {
                this.destinationDirectory = 
                    this.sourceDirectory + 
                    Path.DirectorySeparatorChar +
                    this.settings["DefaultDestinationName"] +
                    Path.DirectorySeparatorChar;
            }
            else
            {
                this.destinationDirectory = Helper.SelectFolder();
            }

            this.CreateDestinationFolder();
        }

        private void CreateDestinationFolder()
        {
            if (!Directory.Exists(this.destinationDirectory))
            {
                Directory.CreateDirectory(this.destinationDirectory);
            }
        }

        private void ReadSettingsFile()
        {
            this.settings = ConfigurationManager.AppSettings;
        }
    }
}