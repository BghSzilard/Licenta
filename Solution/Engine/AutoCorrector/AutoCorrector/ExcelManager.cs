using OfficeOpenXml;

namespace AutoCorrector;

public class ExcelManager
{
    public async Task SaveExcelFile(List<Student> data, string filePath)
    {
        DeleteIfExists(filePath);
        FileInfo fileInfo = new FileInfo(filePath);

        using var package = new ExcelPackage(fileInfo);

        var ws = package.Workbook.Worksheets.Add("MainReport");

        var range = ws.Cells["A1"].LoadFromCollection(data, true);
        range.AutoFitColumns();

        await package.SaveAsync();
    } 

    private void DeleteIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}