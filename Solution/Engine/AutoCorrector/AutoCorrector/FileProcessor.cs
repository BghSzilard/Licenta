using System.IO.Compression;

namespace AutoCorrector;

public class FileProcessor
{
    public void ExtractZip(string sourcePath, string destinationPath)
    {
        ZipFile.ExtractToDirectory(sourcePath, destinationPath);
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