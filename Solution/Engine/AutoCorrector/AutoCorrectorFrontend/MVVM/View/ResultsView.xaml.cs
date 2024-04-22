using System.Windows.Controls;
using System.Windows.Data;
using AutoCorrector;
using AutoCorrectorEngine;

namespace AutoCorrectorFrontend.MVVM.View
{
    public partial class ResultsView : UserControl
    {
        private List<StudentInfo> _students = new List<StudentInfo>();
        
        public ResultsView()
        {
            InitializeComponent();
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            _students = new List<StudentInfo>
{
    new StudentInfo
    {
        Name = "John Doe",
        Grade = 85.5f,
        CodeCompiles = true,
        Requirements = new List<Requirement>
        {
            new Requirement
            {
                Title = "Task 1",
                Points = 10,
                SubRequirements = new List<SubRequirement>
                {
                    new SubRequirement { Title = "Subtask 1.1", Points = 5 },
                    new SubRequirement { Title = "Subtask 1.2", Points = 5 }
                }
            },
            new Requirement
            {
                Title = "Task 2",
                Points = 15,
                SubRequirements = new List<SubRequirement>
                {
                    new SubRequirement { Title = "Subtask 2.1", Points = 7.5f },
                    new SubRequirement { Title = "Subtask 2.2", Points = 7.5f }
                }
            }
        }
    },
    new StudentInfo
    {
        Name = "Jane Smith",
        Grade = 90.0f,
        CodeCompiles = false,
        Requirements = new List<Requirement>
        {
            new Requirement
            {
                Title = "Task 1",
                Points = 10,
                SubRequirements = new List<SubRequirement>
                {
                    new SubRequirement { Title = "Subtask 1.1", Points = 5 },
                    new SubRequirement { Title = "Subtask 1.2", Points = 5 }
                }
            },
            new Requirement
            {
                Title = "Task 2",
                Points = 15,
                SubRequirements = new List<SubRequirement>
                {
                    new SubRequirement { Title = "Subtask 2.1", Points = 7.5f },
                    new SubRequirement { Title = "Subtask 2.2", Points = 7.5f }
                }
            }
        }
    }
};

            dataGrid.AutoGenerateColumns = false;

            dataGrid.Columns.Add(new DataGridTextColumn { Header="Name", Binding = new Binding("Name")});
            dataGrid.Columns.Add(new DataGridTextColumn { Header="Grade", Binding = new Binding("Grade")});
            dataGrid.Columns.Add(new DataGridTextColumn { Header="Code Compiles", Binding = new Binding("CodeCompiles")});

            int index = 1;

            foreach (var requirement in _students[0].Requirements)
            {
                dataGrid.Columns.Add(new DataGridTextColumn { Header=$"Task {index} function", Binding = new Binding($"Requirements[{index - 1}].Title")});
                dataGrid.Columns.Add(new DataGridTextColumn { Header=$"Task {index} points", Binding = new Binding($"Requirements[{index - 1}].Points")});

                int subIndex = 1;
                foreach (var subReq in requirement.SubRequirements)
                {
                    dataGrid.Columns.Add(new DataGridTextColumn { Header=$"Task {index}.{subIndex} points", Binding = new Binding($"Requirements[{index - 1}].SubRequirements[{subIndex - 1}].Points")});

                    dataGrid.Columns.Add(new DataGridTextColumn { Header=$"Task {index}.{subIndex} reason", Binding = new Binding($"Requirements[{index - 1}].SubRequirements[{subIndex - 1}].Title")});

                    subIndex++;

                }
                index++;
            }

            dataGrid.ItemsSource = _students;
        }
    }
}
