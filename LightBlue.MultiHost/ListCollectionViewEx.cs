using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;

namespace LightBlue.MultiHost
{
    public class ListCollectionViewEx : ListCollectionView, IWeakEventListener
    {
        public ListCollectionViewEx(IList list) : base(list)
        {
            var changed = list as INotifyCollectionChanged;
            if (changed != null)
            {
                changed.CollectionChanged -= OnCollectionChanged;
                CollectionChangedEventManager.AddListener(changed, this);
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (!(e is NotifyCollectionChangedEventArgs)) return false;
            OnCollectionChanged(sender, (e as NotifyCollectionChangedEventArgs));
            return true;
        }
    }
}