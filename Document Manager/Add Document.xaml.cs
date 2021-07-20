using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using System.IO;

namespace Document_Manager
{
    /// <summary>
    /// Interaction logic for Add_Document.xaml
    /// </summary>
    public partial class Add_Document
    {
        public Add_Document()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            fileName.Text = openFileDialog.FileName;

        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<string> tags = new List<string>();
            Tags sytemTags = new Tags();
            string[] tgs = Tags.Text.Split(',');
            Uri url = null;

            foreach (string tag in tgs)
            {
                tags.Add(tag.Trim());
                sytemTags.AddTag(tag);
            }

            url = new Uri(fileName.Text);
            string url2 = url.ToString() + "#toolbar=0";
            url = new Uri(url2, UriKind.Absolute);

            List<Document> docs = new List<Document>();
            Document doc = new Document(url, Name.Text, tags);

            docs = doc.ReadFile();
            docs.Add(doc);
            doc.WriteFile(docs);
            sytemTags.WriteFile(sytemTags.TagList);


            this.Close();
        }
    }
}
