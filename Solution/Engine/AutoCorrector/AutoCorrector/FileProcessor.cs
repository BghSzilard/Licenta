using SharpCompress.Archives;
using System.IO.Compression;

namespace AutoCorrector;

public class FileProcessor
{
    public void ExtractZip(string sourcePath, string destinationPath)
    {
        ZipFile.ExtractToDirectory(sourcePath, destinationPath);
    }

    public void ExtractRar(string sourcePath, string destinationPath)
    {
        using (var archive = ArchiveFactory.Open(sourcePath))
        {
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