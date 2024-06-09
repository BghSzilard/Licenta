using System.IO;
using System.Text;
using System.Windows.Forms;
using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class ScaleCreatorViewModel : ObservableObject
{
    private NotificationService _notificationService;
    public ScaleCreatorViewModel(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [ObservableProperty]
    private string _uploadedDocument;

    private string _scale;

    [ObservableProperty]
    private bool _scaleLoaded = false;

    [ObservableProperty]
    private bool _isProcessing = false;

    [ObservableProperty]
    private double _opacity = 1;

    private async Task<string> CreateScale(string doc)
    {
        LLMManager lLMManager = new LLMManager();
        var scale = await lLMManager.CreateScale(doc);
        return scale;
    }

    [RelayCommand]
    public async Task OpenDocument()
    {
        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
        openFileDialog.Filter = "Doc files (*.pdf, *.docx, *.txt)|*.pdf;*.txt;*.docx";
        if (openFileDialog.ShowDialog() == true)
        {
            IsProcessing = true;
            Opacity = 0.3;
            string selectedFileName = openFileDialog.FileName;
            AutoCorrectorEngine.Settings.ProjectsPath = selectedFileName;
            UploadedDocument = selectedFileName;
            _notificationService.NotificationText = "Creating Scale...";
            string doc = "";

            await Task.Run(async () =>
            {
                if (selectedFileName.EndsWith(".txt"))
                {
                    using (StreamReader sr = new StreamReader(selectedFileName))
                    {
                        doc = sr.ReadToEnd();
                    }
                }
                else if (selectedFileName.EndsWith(".docx"))
                {
                    doc = ReadWordDocument(selectedFileName);
                }
                else
                {
                    doc = await ReadPdfFile(selectedFileName);
                }

                _scale = await CreateScale(doc);
                _notificationService.NotificationText = "Scale Created!";
                ScaleLoaded = true;
            });

           IsProcessing = false;
           Opacity = 1;
        }
    }

    public static string ReadWordDocument(string filePath)
    {
        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
        {
            Body body = doc.MainDocumentPart.Document.Body;
            return body.InnerText;
        }
    }

    public static Task<string> ReadPdfFile(string fileName)
    {
        StringBuilder text = new StringBuilder();

        using (PdfReader reader = new PdfReader(fileName))
        {
            PdfDocument pdfDoc = new PdfDocument(reader);
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                text.Append(currentText);
            }
        }

        return Task.FromResult(text.ToString());
    }

    [RelayCommand]
    private async void SaveScale()
    {
        var saveFileDialog = new System.Windows.Forms.SaveFileDialog
        {
            Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
            DefaultExt = ".xml",
            Title = "Save XML File",
            RestoreDirectory = true
        };

        FileInfo file = new FileInfo("temp");

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            string fileName = saveFileDialog.FileName;
            file = new FileInfo(fileName);
            File.WriteAllText(file.FullName, _scale);
        }
    }
}