using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AssemblyVersionUpdater
{
  internal class Program
  {
    static void Main(string[] args)
    {
      string[] assemblyInfoPaths = FindAssemblyInfoPaths();
      if (assemblyInfoPaths.Length == 0)
      {
        Console.WriteLine("No AssemblyInfo.cs files found.");
        return;
      }

      foreach (string assemblyInfoPath in assemblyInfoPaths)
      {
        UpdateAssemblyVersion(assemblyInfoPath);
      }
    }

    static void UpdateAssemblyVersion(string assemblyInfoPath)
    {
      DateTime currentDate = DateTime.Now;
      string assemblyInfoContent = File.ReadAllText(assemblyInfoPath);

      string pattern = @"AssemblyVersion\(""(\d+)\.(\d+)(\.(\*|\d+))(\.(\d+))?""\)";

      string updatedContent = Regex.Replace(assemblyInfoContent, pattern, match =>
      {
        string major = match.Groups[1].Value;
        string minor = match.Groups[2].Value;
        string build = match.Groups[4].Value;
        string revision = match.Groups[6].Success ? match.Groups[6].Value : "";

        if (build == "*")
        {
          major = currentDate.Year.ToString();
          minor = currentDate.Month.ToString();
          build = currentDate.Day.ToString();
          // Use a smaller number for revision to stay within the 0-65535 range
          int totalSeconds = (currentDate.Hour * 3600) + (currentDate.Minute * 60) + currentDate.Second;
          // Divide total seconds by 10 to work in 10-second increments
          int revisionNumber = totalSeconds / 10;
          // Ensure the revision number is within the 0-65535 range
          revisionNumber = revisionNumber % 65536;
          return $@"AssemblyVersion(""{major}.{minor}.{build}.{revision}"")";
        }
        else
        {
          major = currentDate.Year.ToString();
          minor = currentDate.Month.ToString();
          build = currentDate.Day.ToString();
          // Use a smaller number for revision to stay within the 0-65535 range
          int totalSeconds = (currentDate.Hour * 3600) + (currentDate.Minute * 60) + currentDate.Second;
          // Divide total seconds by 10 to work in 10-second increments
          int revisionNumber = totalSeconds / 10;
          // Ensure the revision number is within the 0-65535 range
          revisionNumber = revisionNumber % 65536;
          revision = revisionNumber.ToString();
          return $@"AssemblyVersion(""{major}.{minor}.{build}.{revision}"")";
        }
      });

      File.WriteAllText(assemblyInfoPath, updatedContent);
      Console.WriteLine($"AssemblyVersion updated in {assemblyInfoPath}");
    }

    static string[] FindAssemblyInfoPaths()
    {
      string currentDirectory = Directory.GetCurrentDirectory();
      string targetFileName = "AssemblyInfo.cs";
      int maxDepth = 4;
      List<string> assemblyInfoPaths = new List<string>();

      for (int i = 0; i <= maxDepth; i++)
      {
        string[] files = Directory.GetFiles(currentDirectory, targetFileName, SearchOption.AllDirectories);
        if (files.Length > 0)
        {
          assemblyInfoPaths.AddRange(files);
          break;
        }

        DirectoryInfo parentDir = Directory.GetParent(currentDirectory);
        if (parentDir == null)
        {
          break;
        }
        currentDirectory = parentDir.FullName;
      }

      Console.WriteLine($"Found {assemblyInfoPaths.Count} AssemblyInfo.cs file(s).");
      foreach (string path in assemblyInfoPaths)
      {
        Console.WriteLine($"Found: {path}");
      }

      return assemblyInfoPaths.ToArray();
    }
  }
}