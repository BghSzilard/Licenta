using System.Collections.ObjectModel;

namespace AutoCorrectorEngine;

public static class ObservalbeCollectionExtension
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
    {
        return new ObservableCollection<T>(collection);
    }
}