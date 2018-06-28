using System.ComponentModel;

namespace ThnOlgLauncher.model
{
    internal class Game : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Executable { get; set; }
        public bool RunAsAdmin { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}