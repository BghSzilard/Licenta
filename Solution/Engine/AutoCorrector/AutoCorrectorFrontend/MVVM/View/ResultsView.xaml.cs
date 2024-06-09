using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AutoCorrector;
using AutoCorrectorEngine;

namespace AutoCorrectorFrontend.MVVM.View
{
    public partial class ResultsView : UserControl
    {
        private ProcessExecutor _processExecutor = new ProcessExecutor();

        public ResultsView()
        {
            InitializeComponent();

            if (Settings.StudentSample != null)
            {
                PopulateDataGrid();
            }

        }
        private void PopulateDataGrid()
        {
           
            dataGrid.AutoGenerateColumns = false;

            var style = new Style(typeof(DataGridCell));
            style.Setters.Add(new Setter(Control.FontSizeProperty, 16.0));


            var nameStyle = new Style(typeof(DataGridCell));
            nameStyle.Setters.Add(new Setter(Control.FontSizeProperty, 16.0));

            var columnName = new DataGridTemplateColumn();
            columnName.Header = "Name";
            columnName.CellTemplate = new DataTemplate(typeof(TextBlock));
            columnName.CellTemplate.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
            columnName.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            columnName.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            columnName.CellTemplate.VisualTree.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            //columnName.CellStyle = style;

            var columnGrade = new DataGridTemplateColumn();
            columnGrade.Header = "Grade";
            columnGrade.CellTemplate = new DataTemplate(typeof(TextBlock));
            columnGrade.CellTemplate.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
            columnGrade.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("Grade"));
            columnGrade.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            columnGrade.CellTemplate.VisualTree.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            //columnGrade.CellStyle = style;

            var columnCompile = new DataGridTemplateColumn();
            columnCompile.Header = "Code Compiles";
            columnCompile.CellTemplate = new DataTemplate(typeof(TextBlock));
            columnCompile.CellTemplate.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
            columnCompile.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("CodeCompiles"));
            columnCompile.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            columnCompile.CellTemplate.VisualTree.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);

            dataGrid.Columns.Add(columnName);
            dataGrid.Columns.Add(columnGrade);
            dataGrid.Columns.Add(columnCompile);

            int index = 1;

            foreach (var requirement in Settings.StudentSample.Requirements)
            {

                if (requirement.Type == "method")
                {
                    var columnReqFunc = new DataGridTemplateColumn();
                    columnReqFunc.Header = $"Task {index} function";
                    columnReqFunc.CellTemplate = new DataTemplate(typeof(TextBlock));
                    columnReqFunc.CellTemplate.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
                    columnReqFunc.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding($"Requirements[{index - 1}].Title"));
                    columnReqFunc.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    columnReqFunc.CellTemplate.VisualTree.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);

                    dataGrid.Columns.Add(columnReqFunc);
                }

                

                int subIndex = 1;
                foreach (var subReq in requirement.SubRequirements)
                {
                    var columnsubReqPoint = new DataGridTemplateColumn();

                    columnsubReqPoint.CellTemplate = new DataTemplate();
                    columnsubReqPoint.Header = $"Task {index}.{subIndex} points";
                    var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding($"Requirements[{index - 1}].SubRequirements[{subIndex - 1}].Points"));
                    textBlockFactory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    textBlockFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                    columnsubReqPoint.CellTemplate.VisualTree = textBlockFactory;

                    columnsubReqPoint.CellEditingTemplate = new DataTemplate();
                    var textBoxFactory = new FrameworkElementFactory(typeof(TextBox));
                    textBoxFactory.SetBinding(TextBox.TextProperty, new Binding($"Requirements[{index - 1}].SubRequirements[{subIndex - 1}].Points"));
                    textBoxFactory.SetValue(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Stretch); // Stretch horizontally
                    textBoxFactory.SetValue(TextBox.VerticalAlignmentProperty, VerticalAlignment.Stretch); // Stretch vertically
                    columnsubReqPoint.CellEditingTemplate.VisualTree = textBoxFactory;

                    dataGrid.Columns.Add(columnsubReqPoint);

                    if (subReq.Type == "unitTest")
                    {
                        var columnsubReqButton = new DataGridTemplateColumn();

                        columnsubReqButton.CellTemplate = new DataTemplate();
                        columnsubReqButton.Header = $"Task {index}.{subIndex} test";
                        var buttonFactory = new FrameworkElementFactory(typeof(Button));

                        buttonFactory.SetValue(Button.ContentProperty, "Open Test");

                        // Create a new style
                        Style buttonStyle = new Style(typeof(Button));

                        // Set the properties for the style
                        buttonStyle.Setters.Add(new Setter(Button.BackgroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#007ACC"))));
                        buttonStyle.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.White));
                        buttonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#007ACC"))));
                        buttonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(1)));
                        buttonStyle.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(10, 5, 10, 5)));
                        buttonStyle.Setters.Add(new Setter(Button.MarginProperty, new Thickness(10)));

                        // Set the style to the button
                        buttonFactory.SetValue(Button.StyleProperty, buttonStyle);


                        buttonFactory.SetValue(Button.CommandParameterProperty, $"{index}.{subIndex}");

                        // Set the click event handler of the button
                        buttonFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Button_ClickTest));

                        // Set the alignment of the button
                        buttonFactory.SetValue(Button.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                        buttonFactory.SetValue(Button.VerticalAlignmentProperty, VerticalAlignment.Center);

                        columnsubReqButton.CellTemplate.VisualTree = buttonFactory;

                        dataGrid.Columns.Add(columnsubReqButton);
                    }

                    subIndex++;

                }

                
                index++;
            }
        }

        private async void Button_ClickTest(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var row = (StudentInfo)button.DataContext;
            string task = button.CommandParameter as string;

            await _processExecutor.ExecuteProcess("powershell", "code", $"{System.IO.Path.Combine(Settings.UnitTestsPath, $"{row.Name} {task}.cpp")}");
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var row = (StudentInfo)button.DataContext;

            string files = $"'{row.SourceFile}'";

            foreach (var header in row.HeaderFiles)
            {
                files += " ";
                files += $"'{header}'";
            }

            Process process = new Process();

            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = $"code {files}";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            await process.WaitForExitAsync();
        }
    }
}