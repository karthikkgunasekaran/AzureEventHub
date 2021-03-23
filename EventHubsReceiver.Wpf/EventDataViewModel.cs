using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EventHubsReceiver.Wpf
{
    public class EventDataViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<EventData> EventDataList
        {
            get { return _eventDataList; }
            set
            {
                _eventDataList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("EventDataListObservable"));
                }
            }
        }

        private ObservableCollection<EventData> _eventDataList;

        public EventDataViewModel()
        {
            EventDataList = new ObservableCollection<EventData>();
        }
    }
}
