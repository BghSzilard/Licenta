using AutoCorrector;

class Program
{

    public static async Task Main()
    {
        FileProcessor processor = new FileProcessor();
        var root = "C:\\Users\\sziba\\Desktop\\MyFolder";

        processor.ExtractRar("C:\\Users\\sziba\\Desktop\\AF_2023-Testare Parțial 1 - 17 noiembrie 1600-53027.zip",
            "C:\\Users\\sziba\\Desktop\\MyFolder");

        var subd = Directory.GetDirectories(root);


        foreach (var sub in subd)
        {
            processor.ExtractArchivesRecursively(sub);
            processor.Asd(sub);
        }
    }
}