using Microsoft.Win32;
using System.Windows;

namespace NDSRenameUI
{
    public partial class MainWindow : Window
    {
        private NDSMetadataProcessor _ndsMetaDataProcessor;

        public MainWindow()
        {
            try
            {
                this._ndsMetaDataProcessor = new NDSMetadataProcessor();
            } catch (NDSDatabaseLoadException e)
            {
                this.DisplayError(e.Message);
            }
            this.InitializeComponent();
        }

        private void ResetView()
        {
            this.PartialNameTextBox.Text = "";
            this.NewFileNameTextBox.Text = "";
            this.GameIdTextBox.Text = "";
            this.GameTDBMappingTextBox.Text = "";
            this.CurrentFileNameTextBox.Text = "";
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            var result = openFileDlg.ShowDialog();
            if (result == true)
            {
                this.ResetView();
                try
                {
                    var ndsMetaData = this._ndsMetaDataProcessor.ProcessNDSFile(openFileDlg);
                    this.PartialNameTextBox.Text = ndsMetaData.PartialName;
                    this.NewFileNameTextBox.Text = ndsMetaData.NewFileName;
                    this.GameIdTextBox.Text = ndsMetaData.GameId;
                    this.GameTDBMappingTextBox.Text = ndsMetaData.GameTDBMapping;
                    this.CurrentFileNameTextBox.Text = ndsMetaData.CurrentFileName;
                }
                catch (NDSFileStreamException ex)
                {
                    this.DisplayError(ex.Message);
                }
                catch (NDSFileException ex)
                {
                    this.DisplayError(ex.Message);
                }
            }
        }

        private void DisplayError(string error) => MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        private void RenameFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
