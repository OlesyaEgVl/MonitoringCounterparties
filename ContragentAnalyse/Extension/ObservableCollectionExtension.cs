using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ContragentAnalyse.Extension
{
    public static class ObservableCollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
        {
            foreach(T t in range)
            {
                collection.Add(t);
            }
            //range.ToList().ForEach(collection.Add);
        }
    }
}
