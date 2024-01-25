class Program
{

    public static async Task Main()
    {
        CompilationCheckerWrapper compilationCheckerWrapper = new CompilationCheckerWrapper();
        Console.WriteLine(compilationCheckerWrapper.Compiles("C:\\Users\\sziba\\Desktop\\Asd.cpp"));
    }
}