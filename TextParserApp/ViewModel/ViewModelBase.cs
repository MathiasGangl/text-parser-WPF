using System.Collections.Generic;
using System.ComponentModel;
using TextParsingLibrary;

namespace TextParserApp.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void TriggerPropertyChanged(string p)
        {
            CheckPropertyNameExists(p);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public void CheckPropertyNameExists(string name)
        {
            if (TypeDescriptor.GetProperties(this)[name] == null)
            {
                throw new System.Exception("Error: property with name \"" + name + "\" not found!");
            }
        }
    }
}
