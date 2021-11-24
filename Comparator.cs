﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace PBE_NewFileExtractor
{
    public class FilesComparator
    {
        private const string AssetsFilePath = @".\Resources\Assets.txt";
        private const string NewAssetsFilePath = @".\Resources\NewAssets.txt";
        private const string DifferencesFilePath = @".\Resources\Differences.txt";
        
        private readonly List<string> _differencesList;
        public FilesComparator()
        {
            _differencesList = new List<string>();
        }
        public async Task CheckFilesDiffAsync()
        {
            try
            {
                Log.Logger = LogSettings.CreateLogger();
                Log.Information("Checking for differences between {0} and {1} ", AssetsFilePath.Split('\\').Last(), NewAssetsFilePath.Split('\\').Last());
                await Task.Delay(3000);
                DifferenceList();
                Log.Information("Found {0} differences", _differencesList.Count);
                await Task.Delay(3000);
            }
            catch (Exception e)
            {
                Log.Error("Error during checking for differences!\nError:\n{0}", e);
                Console.ReadKey();
            }

            try
            {
                await File.WriteAllLinesAsync(DifferencesFilePath, _differencesList);
                switch (File.Exists(DifferencesFilePath))
                {
                    case false:
                        Log.Information("File created: {0}", Path.GetFullPath(DifferencesFilePath));
                        break;
                    case true:
                        Log.Information("File updated: {0}", Path.GetFullPath(DifferencesFilePath));
                        break;
                }
                await Task.Delay(3000);
            }
            catch (Exception e)
            {
                Log.Error("Error during file {1} creation!\nError:\n{0}", e, DifferencesFilePath.Split('\\').Last());
                Console.ReadKey();
            }
        }

        public void ReplaceAssetsFile()
        {
            try
            {
                File.Replace(NewAssetsFilePath, AssetsFilePath, null);
            }
            catch (Exception e)
            {
                Log.Error("Error during file replace!\nError:\n{0}", e);
                Console.ReadKey();
            }
        }

        public int DifferenceList()
        {
            var assetsFileLines = File.ReadAllLines(AssetsFilePath);
            var newAssetsFileLines = File.ReadAllLines(NewAssetsFilePath);
            var differences = newAssetsFileLines.Except(assetsFileLines);
            foreach (var difference in differences)
            {
                if (difference.Contains(".png") ||
                    difference.Contains(".jpg") ||
                    difference.Contains(".ogg") ||
                    difference.Contains(".webm"))
                {
                    _differencesList.Add(difference);
                }
            }
            return _differencesList.Count;
        }
    }
}