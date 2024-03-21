using ARC.Models.ExternalModels;
using ARC.Models.InternalModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ARC.Modules
{
    public static class StaticValues
    {
        public static List<string> RequestedAvatarIds = new List<string>();
        public static string TempFileLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ARC\\";
        public static string ConfigLocation = $"{TempFileLocation}\\config.cfg";
        public static string ProgramLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string RippedLocation = $"{TempFileLocation}\\ripped.cfg";
        public static string FavLocation = $"{TempFileLocation}\\favourites.cfg";
        public static string DownloadLocation = $"{TempFileLocation}\\download.cfg";
        public static string ArcDocuments = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ARC\\";
        public static string VrcaDownloadFolder = $"{ArcDocuments}VRCA\\";
        public static string VrcwDownloadFolder = $"{ArcDocuments}VRCW\\";
        public static string ImagesDownloadFolder = $"{ArcDocuments}Images\\";
        public static string UnityProject = $"{ArcDocuments}Unity\\";
        public static string VrcaViewer = $"{ArcDocuments}Viewer\\";
        public static string AssetRipper = $"{ArcDocuments}AssetRipper\\";
        public static string ExtractedFiles = $"{ArcDocuments}ExtractedFiles\\";
        public static ConfigSave<Config> Config = new ConfigSave<Config>(StaticValues.ConfigLocation);
        public static ConfigSave<List<AvatarModel>> RippedList;
        public static ConfigSave<List<AvatarModel>> FavList;
        public static ConfigSave<ListDown> DownloadQueue;
        public static List<AvatarModel> Avatars;
    }
}