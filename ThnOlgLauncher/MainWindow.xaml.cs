using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThnOlgLauncher.controller;
using ThnOlgLauncher.model;
using static System.Environment;

namespace ThnOlgLauncher {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //The operation was canceled by the user.
        private const int ERROR_CANCELLED = 1223;

        private DataStore data = new DataStore();

        public MainWindow() {
            InitializeComponent();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e) {
            JsonStorage.fileName = System.IO.Path.Combine(GetFolderPath(SpecialFolder.UserProfile), "thnolgdb.json");
            JsonStorage.loadJson(data);
            gameList.ItemsSource = data.games;
            serverList.ItemsSource = data.servers;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void launchButtonUpdate() {
            launchButton.IsEnabled =
                tabControl.SelectedItem.Equals(serverTab)
                && mainWindow.serverList.SelectedItem != null;
        }

        private void updateSaveButtonEnable() {
            saveButton.IsEnabled = JsonStorage.isDirty(data);
        }

        private void serverList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            launchButtonUpdate();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            launchButtonUpdate();
        }

        private void launchButton_Click(object sender, RoutedEventArgs e) {
            Server server = (Server) mainWindow.serverList.SelectedItem;
            Game game = data.games.Find(item => item.tag == server.gameTag);

            String arguments = " +connect " + server.address + ":" + server.port;
            String workingDirectory = new FileInfo(game.executable).Directory.FullName;

            ProcessStartInfo process = new ProcessStartInfo(game.executable);
            process.WorkingDirectory = workingDirectory;
            process.Arguments = arguments;
            process.UseShellExecute = true;
            if(game.runAsAdmin == true) {
                process.Verb = "runas";
            }

            try {
                Process.Start(process);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void gameList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            updateSaveButtonEnable();
        }

        private void serverList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            updateSaveButtonEnable();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {
            JsonStorage.saveJson(data);
            updateSaveButtonEnable();
        }
    }
}
