class Program
{

    public static async Task Main()
    {
        //StudentManager studentManager = new StudentManager();
        //await studentManager.Solve();
        //CompilationCheckerWrapper compilationCheckerWrapper = new CompilationCheckerWrapper();
        CompilationCheckerWrapper compilationCheckerWrapper = new CompilationCheckerWrapper();
        //compilationCheckerWrapper.compiles("asd");
        Console.WriteLine(compilationCheckerWrapper.compiles("C:\\Users\\sziba\\Desktop\\Asd.cpp"));
    }
}