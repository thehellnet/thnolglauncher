using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThnOlgLauncher.model
{
    class Game : INotifyPropertyChanged {

        public String name { get; set; }
        public String tag { get; set; }
        public String executable { get; set; }
        public bool runAsAdmin { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string p) {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }
}
