class Program
{

    public static async Task Main()
    {
        CompilationCheckerWrapper compilationCheckerWrapper = new CompilationCheckerWrapper();
        Console.WriteLine(compilationCheckerWrapper.ContainsMain("C:\\Users\\sziba\\Desktop\\Asd.cpp"));
    }
}