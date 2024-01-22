using AutoCorrector;

class Program
{

    public static void Main()
    {
        string sourcePath = "C:\\Users\\sziba\\Desktop\\AF_2023-Testare Parțial 1 - 17 noiembrie 1600-53027.zip";
        string destinationPath = "C:\\Users\\sziba\\Desktop\\MyFolder";

        FileProcessor processor = new FileProcessor();
        List<Student> students = new List<Student>();
        var subdirectoryNames = processor.GetSubdirectoryNames(destinationPath);

        foreach (var subdirectoryName in subdirectoryNames)
        {
            var studentName = processor.SeparateString(subdirectoryName, '_');
            students.Add(new Student() { Name = studentName});
        }

        foreach (var studentName in students)
        {
            Console.WriteLine(studentName.Name);
        }
    }
}