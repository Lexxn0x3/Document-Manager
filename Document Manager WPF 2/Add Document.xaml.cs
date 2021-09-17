using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Document_Manager_WPF_2
{
    /// <summary>
    /// Interaction logic for Add_Document.xaml
    /// </summary>
    public partial class Add_Document : Window
    {
        public Document? Doc { get; set; }
        public bool? AnotherDocument { get; set; }
        public string? Tags { get; set; }
        public Add_Document()
        {
            InitializeComponent();
        }

        private void openFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            openFileDialog.ShowDialog();

            FileInfo? info = null;

            try
            {
                info = new FileInfo(openFileDialog.FileName);
                fileDirectoryBox.Text = info.FullName;
                fileNameBox.Text = (string)System.IO.Path.GetFileNameWithoutExtension(info.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open file:\n\n" + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Document doc = new Document(fileNameBox.Text, new Uri(fileDirectoryBox.Text), tagsBox.Text);

            Doc = doc;

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Document doc = new Document(fileNameBox.Text, new Uri(fileDirectoryBox.Text), tagsBox.Text);

            Doc = doc;

            AnotherDocument = true;
            Tags = tagsBox.Text;

            this.Close();
        }

        private void tagsBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            tagsBoxAutoComplete.Text = MainWindow.autoComplete(tagsBox.Text, MainWindow.gloabalTags);
        }

        private void tagsBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tagsBox.Text = tagsBoxAutoComplete.Text;

                tagsBox.CaretIndex = tagsBox.Text.Length;  //Cursor to Last position

            }
        }
    }
}
