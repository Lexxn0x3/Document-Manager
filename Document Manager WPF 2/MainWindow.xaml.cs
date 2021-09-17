using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Document_Manager_WPF_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Document> Docs { set; get; }
        public ObservableCollection<Document> FilterDocs { set; get; }
        static public List<string>? gloabalTags { get; set; }

        public MainWindow()
        {
            Docs = new ObservableCollection<Document>();
            FilterDocs = new ObservableCollection<Document>();
            gloabalTags = new List<string>();

            Docs = ReadFile();
            InitializeComponent();

            refreshGlobalTags();
            resetSearch();
            elementDataGrid.ItemsSource = FilterDocs;
        }

        static public string getDataDirectory(string file)
        {
            //string fileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Document Manager",file);

            string directory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Document Manager");
            string fileName = System.IO.Path.Combine(directory, file);

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
            return (fileName);
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            bool another = false;
            string tags = "";

            do
            {
                another = false;
                Add_Document w = new Add_Document();
                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                w.tagsBox.Text = tags;
                w.ShowDialog();

                if (w.Doc != null)
                {
                    Docs.Add(w.Doc);


                    refreshGlobalTags();
                    //resetSearch();
                    refreshElementList();
                    saveDocuments(Docs);

                    if (w.AnotherDocument == true)
                    {
                        tags = w.Tags;
                        another = true;
                    }
                }
            } while (another);
        }

        private void elementDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (elementDataGrid.SelectedItem != null)
            {
                Document doc = ((Document)elementDataGrid.SelectedItem);
                string url = doc.Url.ToString() + "#toolbar=0";
                web.Source = new Uri(url);

                FileInfo info = new FileInfo(doc.Url.LocalPath);
                filenameBox.Text = info.Name;
                fileLastEditBox.Text = info.LastWriteTime.ToString();
                fileDateOfCreation.Text = info.CreationTime.ToString();
                fileUrlBox.Text = doc.Url.LocalPath;
                fileSizeBox.Text = Math.Round(((info.Length) * 9.5367431640625 * Math.Pow(10,-7)), 2).ToString() + "Mb";
                fileTagsBox.Text = Document.arrayToTagString(doc.Tags);
            }
        }
        private void tagsBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            refreshElementList();
            //string[] searchTags = Document.stringToTagList(tagsBox.Text);

            //if (tagsBox.Text == "")
            //{
            //    resetSearch();
            //    return;
            //}

            //FilterDocs.Clear();

            //for (int i = 0; i < Docs.Count; i++)
            //{
            //    bool incluedes = true;
            //    foreach (string tag in searchTags)
            //    {
            //        if (!Docs[i].Tags.Contains(tag))
            //        {
            //            incluedes = false;
            //        }
            //    }
            //    if (incluedes)
            //    {
            //        FilterDocs.Add(Docs[i]);
            //    }
            //}

            //tagsBoxAutoComplete.Text = autoComplete(tagsBox.Text, gloabalTags);

        }

        private void refreshElementList()
        {
            string[] searchTags = Document.stringToTagList(tagsBox.Text);

            if (tagsBox.Text == "")
            {
                resetSearch();
                return;
            }

            FilterDocs.Clear();

            for (int i = 0; i < Docs.Count; i++)
            {
                bool incluedes = true;
                foreach (string tag in searchTags)
                {
                    if (!Docs[i].Tags.Contains(tag))
                    {
                        incluedes = false;
                    }
                }
                if (incluedes)
                {
                    FilterDocs.Add(Docs[i]);
                }
            }

            tagsBoxAutoComplete.Text = autoComplete(tagsBox.Text, gloabalTags);
        }

        private void resetSearch()
        {
            tagsBox.Text = "";
            FilterDocs.Clear();
            foreach (Document document in Docs)
            {
                FilterDocs.Add(document);
            }
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            removeDocument((Document)elementDataGrid.SelectedItem);
        }

        private void removeDocument(Document selectedDocument)
        {
            Docs.Remove(selectedDocument);
            FilterDocs.Remove(selectedDocument);
        }

        private void saveDocuments(ObservableCollection<Document> docs)
        {
            string file = "";
            foreach (Document doc in docs)
            {
                file += (doc.Name + "\n");

                string tags = "";
                foreach (string tag in doc.Tags)
                {
                    tags += (tag + " ");
                }
                file += (tags + "\n");

                file += (doc.Url.LocalPath.ToString() + "\n");

                file += "\n";
            }

            File.WriteAllText(getDataDirectory("data.dat"), file);
        }

        public ObservableCollection<Document> ReadFile()
        {
            string path = getDataDirectory("data.dat");

            if (System.IO.File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);

                int dataLine = 1;

                Uri url = null;
                string name = null;
                List<string> tags = new List<string>();
                ObservableCollection<Document> docs = new ObservableCollection<Document>();

                foreach (string line in lines)
                {
                    if (line != "")
                    {
                        switch (dataLine)
                        {
                            case 1:
                                name = line;
                                break;
                            case 2:
                                string[] tgs = line.Split(' ');

                                foreach (string tag in tgs)
                                {
                                    tags.Add(tag);
                                }
                                break;
                            case 3:
                                url = new Uri(line);
                                string url2 = url.ToString();
                                url = new Uri(url2, UriKind.Absolute);
                                break;
                        }
                        dataLine++;
                    }
                    else
                    {
                        string[] tagsArray = tags.ToArray();
                        docs.Add(new Document(name, url, tagsArray));
                        dataLine = 1;
                        url = null;
                        name = null;
                        tags = new List<string>();
                    }

                }
                return (docs);
            }
            else
                return new ObservableCollection<Document>();
        }

        public void refreshGlobalTags()
        {
            foreach (Document doc in Docs)
            {
                foreach (string tag in doc.Tags)
                {
                    if (!gloabalTags.Contains(tag))
                    {
                        gloabalTags.Add(tag);
                    }
                }
            }
        }
        static public string autoComplete(string text, List<string> globalTags)
        {
            string[] tags = Document.stringToTagList(text);
            List<string> taglist = tags.ToList();

            string phrase = taglist[taglist.Count-1];

            if (phrase.Length > 1)
            {
                string suggestion = "";

                foreach (string tag in globalTags)
                {
                    if (tag.Contains(phrase))
                    {
                        for (int i = 0; i < taglist.Count - 1; i++)
                        {
                            suggestion += taglist[i] + ", ";
                        }
                        suggestion += tag;
                    }
                }
                return suggestion;
            }
            else return "";
        }
        private void Tags_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tagsBox.Text = tagsBoxAutoComplete.Text;

                tagsBox.CaretIndex = tagsBox.Text.Length;  //Cursor to Last position

            }
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(((Document)elementDataGrid.SelectedItem).Url.LocalPath);
        }
        private void OpenFile(string filePath)
        {

            if (File.Exists(filePath))
            {
                FileInfo info = new FileInfo(filePath);
                Process.Start(@"explorer.exe", info.FullName);
            }
            else
            {
                MessageBox.Show("File not available", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OpenFolder(string folderPath)
        {
            FileInfo info = new FileInfo(folderPath);

            if (Directory.Exists(info.DirectoryName))
            {
                Process.Start(@"explorer.exe", info.DirectoryName);
            }
            else
            {
                MessageBox.Show("Directory not available", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tagsBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void openFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder(((Document)elementDataGrid.SelectedItem).Url.LocalPath);
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (((Document)elementDataGrid.SelectedItem) != null)
            {
                string[] tags = ((Document)elementDataGrid.SelectedItem).Tags;
                string tagsBoxText = Document.arrayToTagString(tags);


                Add_Document w = new Add_Document();
                w.Title = "Editing: " + ((Document)elementDataGrid.SelectedItem).Name;
                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                w.tagsBox.Text = tagsBoxText;
                w.fileNameBox.Text = ((Document)elementDataGrid.SelectedItem).Name;
                w.fileDirectoryBox.Text = ((Document)elementDataGrid.SelectedItem).Url.LocalPath;
                w.nextButton.Visibility = Visibility.Hidden;
                w.ShowDialog();

                if (w.Doc != null)
                {
                    removeDocument((Document)elementDataGrid.SelectedItem);

                    Docs.Add(w.Doc);

                    refreshGlobalTags();
                    refreshElementList();
                    //resetSearch();
                    saveDocuments(Docs);
                }
            }
        }
    }
}