using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
            //style.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            //style.Setters.Add(new Setter(VerticalContentAlignmentProperty, VerticalAlignment.Center));
            //style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            //style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Stretch));
            style.Setters.Add(new Setter(Control.FontSizeProperty, 16.0));


            var nameStyle = new Style(typeof(DataGridCell));
            //nameStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            //nameStyle.Setters.Add(new Setter(VerticalContentAlignmentProperty, VerticalAlignment.Center));
            //nameStyle.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            //nameStyle.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Stretch));
            nameStyle.Setters.Add(new Setter(Control.FontSizeProperty, 16.0));

            var columnName = new DataGridTemplateColumn();
            columnName.Header = "Name";
            columnName.CellTemplate = new DataTemplate(typeof(TextBlock));
            columnName.CellTemplate.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
            columnName.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            columnName.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            columnName.CellTemplate.VisualTree.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            columnName.CellStyle = style;

            var columnGrade = new DataGridTemplateColumn();
            columnGrade.Header = "Grade";
            columnGrade.CellTemplate = new DataTemplate(typeof(TextBlock));
            columnGrade.CellTemplate.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
            columnGrade.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding("Grade"));
            columnGrade.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            columnGrade.CellTemplate.VisualTree.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            columnGrade.CellStyle = style;

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

                var columnReqPoint = new DataGridTemplateColumn();
                columnReqPoint.Header = $"Task {index} points";
                columnReqPoint.CellTemplate = new DataTemplate(typeof(TextBlock));
                columnReqPoint.CellTemplate.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
                columnReqPoint.CellTemplate.VisualTree.SetBinding(TextBlock.TextProperty, new Binding($"Requirements[{index - 1}].Points"));
                columnReqPoint.CellTemplate.VisualTree.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                columnReqPoint.CellTemplate.VisualTree.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);

                dataGrid.Columns.Add(columnReqPoint);

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
                    
                    subIndex++;

                }

                
                index++;
            }
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var row = (StudentInfo)button.DataContext;

            await _processExecutor.ExecuteProcess("powershell", "code", row.SourceFile!);
        }
    }
}