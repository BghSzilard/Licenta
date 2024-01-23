using AutoCorrector;

class Program
{

    public static async Task Main()
    {
        StudentManager studentManager = new StudentManager();
        await studentManager.GetNames();

    }
}