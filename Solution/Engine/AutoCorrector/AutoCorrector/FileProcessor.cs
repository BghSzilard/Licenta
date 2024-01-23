using SharpCompress;
using SharpCompress.Archives;
using System.IO.Compression;

namespace AutoCorrector;

public class FileProcessor
{
    public void ExtractZip(string sourcePath, string destinationPath)
    {
        try
        {
            ZipFile.ExtractToDirectory(sourcePath, destinationPath, true);
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void ExtractRar(string sourcePath, string destinationPath)
    {
        try
        {
            using var archive = ArchiveFactory.Open(sourcePath);
            foreach (var entry in archive.Entries)
            {
                if (!entry.IsDirectory)
                {
                    entry.WriteToDirectory(destinationPath, new SharpCompress.Common.ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void ExtractArchivesRecursively(string rootDirectory)
    {
        string[] zipFiles = Directory.GetFiles(rootDirectory, "*.zip");
        string[] rarFiles = Directory.GetFiles(rootDirectory, "*.rar");

        foreach (string zipFile in zipFiles)
        {
            ExtractZip(zipFile, rootDirectory);
            File.Delete(zipFile);
        }

        foreach (string rarFile in rarFiles)
        {
            ExtractRar(rarFile, rootDirectory);
            File.Delete(rarFile);
        }

        string[] subDirectories = Directory.GetDirectories(rootDirectory);

        foreach (string subDirectory in subDirectories)
        {        
            ExtractArchivesRecursively(subDirectory);
        }

        Array.Clear(zipFiles);
        Array.Clear(rarFiles);

        zipFiles = Directory.GetFiles(rootDirectory, "*.zip");
        rarFiles = Directory.GetFiles(rootDirectory, "*.rar");

        if (zipFiles.Length > 0 || rarFiles.Length > 0)
        {
            ExtractArchivesRecursively(rootDirectory);
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
}