using OfficeOpenXml;
namespace AutoCorrector;

public class ExcelManager
{
    public ExcelManager()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    public async Task SaveExcelFile(List<StudentInfo> data)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
            DefaultExt = ".xlsx",
            Title = "Save Excel File",
            RestoreDirectory = true
        };

        FileInfo excelFile = new FileInfo("temp");

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            string fileName = saveFileDialog.FileName;
            excelFile = new FileInfo(fileName);
            DeleteIfExists(excelFile);
        }

        using var package = new ExcelPackage(excelFile);
        var ws = package.Workbook.Worksheets.Add("Rezultate");

        ws.Cells["A1"].Value = "Name";
        ws.Cells["B1"].Value = "Grade";
        ws.Cells["C1"].Value = "Code Compiles";

        int row = 2;
        int column = 4;

        var scale = data[0].Requirements;

        var index = 1;

        foreach (var requirement in scale)
        {
            if (requirement.Type == "method")
            {
                ws.Cells[1, column].Value = $"Task {index} function";
                column++;
            }
            ws.Cells[1, column].Value = $"Task {index} points";
            column++;
            var subIndex = 1;
            foreach (var subreq in requirement.SubRequirements)
            {
                ws.Cells[1, column].Value = $"Task {index}.{subIndex} points";
                column++;
                subIndex++;
            }

            index++;
        }

        foreach (var student in data)
        {
            ws.Cells[$"A{row}"].Value = student.Name;
            ws.Cells[$"B{row}"].Value = student.Grade;
            ws.Cells[$"C{row}"].Value = student.CodeCompiles ? "Yes" : "No";

            index = 0;
            column = 4;

            foreach (var requirement in student.Requirements)
            {
                if (requirement.Type == "method")
                {
                    ws.Cells[row, column].Value = requirement.Title;
                    column++;
                }
                
                ws.Cells[row, column].Value = requirement.Points;
                column++;

                foreach (var subReq in requirement.SubRequirements)
                {
                    ws.Cells[row, column].Value = subReq.Points;
                    column++;
                }
            }

            row++;
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();


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