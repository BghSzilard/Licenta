using OfficeOpenXml;

namespace AutoCorrector;

public class ExcelManager
{
    public ExcelManager() 
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    public async Task SaveExcelFile<T>(List<T> data, FileInfo fileInfo)
    {
        DeleteIfExists(fileInfo);
        
        using var package = new ExcelPackage(fileInfo);

        var ws = package.Workbook.Worksheets.Add("Rezultate");

        var range = ws.Cells["A1"].LoadFromCollection(data, true);
        range.AutoFitColumns();

        await package.SaveAsync();
    } 

    private void DeleteIfExists(FileInfo fileInfo)
    {
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
    }
}