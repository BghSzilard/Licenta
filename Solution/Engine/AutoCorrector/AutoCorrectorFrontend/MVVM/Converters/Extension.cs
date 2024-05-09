using System.Windows.Controls;

namespace AutoCorrectorFrontend.MVVM.Converters;

public static class Extension
{
    public static DataGridRow GetRow(this DataGrid grid, int index)
    {
        DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
        if (row == null)
        {
            // May be virtualized, bring into view and try again.
            grid.UpdateLayout();
            grid.ScrollIntoView(grid.Items[index]);
            row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
        }
        return row;
    }
}