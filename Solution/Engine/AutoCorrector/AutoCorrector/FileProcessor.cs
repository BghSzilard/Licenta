using SharpCompress;
using SharpCompress.Archives;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Text;
using AutoCorrectorEngine;
using CsvHelper;
using SharpCompress.Common;
using System.Xml;

namespace AutoCorrector;

public class FileProcessor
{
    public async Task ExtractZip(string sourcePath, string destinationPath)
    {
        try
        {
            await Task.Run(() => ZipFile.ExtractToDirectory(sourcePath, destinationPath, true));
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task ExtractRar(string sourcePath, string destinationPath)
    {
        try
        {
            using var archive = ArchiveFactory.Open(sourcePath);
            foreach (var entry in archive.Entries)
            {
                if (!entry.IsDirectory)
                {
                    await Task.Run(() =>
                    {
                        entry.WriteToDirectory(destinationPath, new SharpCompress.Common.ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task ExtractArchivesRecursively(string rootDirectory)
    {
        string[] zipFiles = Directory.GetFiles(rootDirectory, "*.zip");
        string[] rarFiles = Directory.GetFiles(rootDirectory, "*.rar");

        foreach (string zipFile in zipFiles)
        {
            await ExtractZip(zipFile, rootDirectory);
            await Task.Run(() => { File.Delete(zipFile); });
        }

        foreach (string rarFile in rarFiles)
        {
            await ExtractRar(rarFile, rootDirectory);
            await Task.Run(() => { File.Delete(rarFile); });
        }

        string[] subDirectories = Directory.GetDirectories(rootDirectory);

        foreach (string subDirectory in subDirectories)
        {        
            await ExtractArchivesRecursively(subDirectory);
        }

        Array.Clear(zipFiles);
        Array.Clear(rarFiles);

        zipFiles = Directory.GetFiles(rootDirectory, "*.zip");
        rarFiles = Directory.GetFiles(rootDirectory, "*.rar");

        if (zipFiles.Length > 0 || rarFiles.Length > 0)
        {
            await ExtractArchivesRecursively(rootDirectory);
        }
    }

    

    public void ExtractFiles(string rootDirectory, string currentDirectory, List<string> extensions)
    {
        var subdirectories = Directory.GetDirectories(currentDirectory);

        List<string> files = new List<string>();

        foreach (var extension in extensions)
        {
            foreach (var file in (Directory.GetFiles(currentDirectory, $"*{extension}")))
            {
                files.Add(file);
            }
        }

        foreach (var file in files)
        {
            File.Copy(file, Path.Combine(rootDirectory, Path.GetFileName(file)), true);
        }

        foreach (var subdirectory in subdirectories)
        {
            ExtractFiles(rootDirectory, subdirectory, extensions);
        }

        DeleteDirectory(currentDirectory);
        
    }
    public void DeleteDirectory(string rootDirectory)
    {
        string[] files = Directory.GetFiles(rootDirectory);
        string[] dirs = Directory.GetDirectories(rootDirectory);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(rootDirectory, false);
    }

    public List<string> GetSubdirectoryNames(string path)
    {
        var directory = new DirectoryInfo(path);

        var subdirectories = directory.GetDirectories().Select(x => x.Name).ToList();

        return subdirectories;
    }

    public string SeparateString(string stringToSeparate, char separator)
    {
        return stringToSeparate.Split(separator)[0];
    }

    public async Task<List<string>> FindHeaders(string path)
    {
        List<string> files = Directory.GetFiles(path, "*.h").ToList();
        files.AddRange(Directory.GetFiles(path, "*.hpp").ToList());

        return files;
    }
    public async Task<string?> FindSourceFile(string path)
    {
        var files = Directory.GetFiles(path, "*.cpp");

        CompilationCheckerWrapper compilationCheckerWrapper = new CompilationCheckerWrapper();

        if (files.Length == 1)
        {
            return files[0];
        }

        foreach (var file in files)
        {
            bool containsMain = await Task.Run(() => compilationCheckerWrapper.ContainsMain(file)).ConfigureAwait(false);
            if (containsMain)
            {
                return file;
            }
        }

        return null;
    }

    public bool Compiles(string path)
    {
        CompilationCheckerWrapper compilationCheckerWrapper = new CompilationCheckerWrapper();

        return compilationCheckerWrapper.Compiles(path);
    }

    public string GetFolder(string path, string name)
    {
        var subfolder = Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories)
    .FirstOrDefault(folder => folder.Contains(name));

        return subfolder;
    }

    public string FindIncludes(string filePath)
    {
        StringBuilder includesBuilder = new StringBuilder();
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Match match = Regex.Match(line, @"^\s*#include\s*[<""](.*)[>""]\s*$");
                if (match.Success)
                {
                    includesBuilder.AppendLine(line);
                }
            }
        }
        return includesBuilder.ToString();
    }
}