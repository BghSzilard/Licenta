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

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Grade", Binding = new Binding("Grade") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Code Compiles", Binding = new Binding("CodeCompiles") });

            int index = 1;

            foreach (var requirement in Settings.StudentSample.Requirements)
            {
                dataGrid.Columns.Add(new DataGridTextColumn { Header = $"Task {index} function", Binding = new Binding($"Requirements[{index - 1}].Title") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = $"Task {index} points", Binding = new Binding($"Requirements[{index - 1}].Points") });

                int subIndex = 1;
                foreach (var subReq in requirement.SubRequirements)
                {
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = $"Task {index}.{subIndex} points", Binding = new Binding($"Requirements[{index - 1}].SubRequirements[{subIndex - 1}].Points") });

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