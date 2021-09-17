using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using Document_Manager;
using Windows.Storage;
using Windows.Data.Pdf;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace Document_Manager_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PdfDocument _myDocument { get; set; }
        static List<Document> docs = new List<Document>();
        Tags tags = new Tags(); //Auto Read from File
        public MainPage()
        {
            this.InitializeComponent();

            Document dc = new Document();
            docs = dc.ReadFile();

            UpdateList(new string[0]);
        }
        private void UpdateList(string[] tags)
        {
            //Document dc = new Document();
            //trv.Items.Clear();
            //docs = dc.ReadFile();
            //Tree myTree = new Tree();
            //trv.Items.Add(myTree.Readfile());
            //TreeViewItem tree = (TreeViewItem)trv.Items[0];
            //fillTree(tree, new Stack<string>());


            Document dc = new Document();
            List<Document> searchedDocs = new List<Document>();
            docs = dc.ReadFile();
            documentListView.ItemsSource = searchedDocs;
            searchedDocs.Clear();

            foreach (Document item in docs)
            {
                bool containsTags = true;
                foreach (string tag in tags)
                {
                    if (!item.Tags.Contains(tag))
                    {
                        containsTags = false;
                    }
                }

                if (containsTags)
                {
                    searchedDocs.Add(item);
                }
            }
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string[] tags = tagBox.Text.Split(',');
            for (int i = 0; i < tags.Length; i++)
            {
                tags[i] = tags[i].Trim();
            }
            UpdateList(tags);
        }

        private async void documentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Document doc = (Document)documentListView.SelectedItem;


            var file = await StorageFile.GetFileFromPathAsync(doc.Url.AbsoluteUri);

            await OpenPDFAsync(file);
            await DisplayPage(0);

            //web.Source = doc.Url;   //<------ webview not working

        }

        private async void Button_Click_Open_PDF(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".pdf");

            StorageFile file = await picker.PickSingleFileAsync();

            //copy file

            // Open and display the PDF.
        }

        private async Task OpenPDFAsync(StorageFile file)
        {
            if (file == null) throw new ArgumentNullException();

            _myDocument = await PdfDocument.LoadFromFileAsync(file);
        }

        private async Task DisplayPage(uint pageIndex)
        {
            if (_myDocument == null)
            {
                throw new Exception("No document open.");
            }

            if (pageIndex >= _myDocument.PageCount)
            {
                throw new ArgumentOutOfRangeException($"Document has only {_myDocument.PageCount} pages.");
            }

            // Get the page you want to render.
            var page = _myDocument.GetPage(pageIndex);

            // Create an image to render into.
            var image = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);
                await image.SetSourceAsync(stream);

                // Set the XAML `Image` control to display the rendered image.
                PdfImage.Source = image;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button_Click_Open_PDF(sender, e);
        }
    }
}
