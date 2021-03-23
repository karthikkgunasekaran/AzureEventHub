using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EventHubsReceiver.AzureActivityLog.Wpf
{
    public static class ObservableCollectionExtension
    {
        public static void OrderByDescendingCustom<TSource, TKey>(this ObservableCollection<TSource> observableCollection, Func<TSource, TKey> keySelector)
        {
            var a = observableCollection.OrderByDescending(keySelector).ToList();
            observableCollection.Clear();
            foreach (var b in a)
            {
                observableCollection.Add(b);
            }
        }
    }
}
