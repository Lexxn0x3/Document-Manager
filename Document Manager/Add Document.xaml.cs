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
        public bool AnotherDocument { get; set; }
        public string AutoTags { get; set; }
        public Add_Document(string tags)
        {
            InitializeComponent();
            Tags.Text = tags;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            fileName.Text = openFileDialog.FileName;

        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AnotherDocument = true;
            AutoTags = Tags.Text;
            Button_Click_1(sender, e);

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

            if (CopyFileCheckBox.IsChecked == true)
            {
                FileInfo info = new FileInfo(fileName.Text);
                url = new Uri(Directory.GetCurrentDirectory() + @"\" + info.Name);

                if (File.Exists(url.LocalPath))
                {
                    url = new Uri(url.LocalPath);
                }
                else
                {
                    File.Copy(fileName.Text, url.LocalPath);
                }
            }
            else
            {
                url = new Uri(fileName.Text);
            }

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

        private void Tags_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tagsString = Tags.Text;
            string[] tgs = Tags.Text.Split(',');

            string tag = tgs[tgs.Length-1];
            tag = tag.Trim();

            Tags tags = new Tags();
            tags.ReadFile();

            foreach (string tg in tags.TagList)
            {
                if (tg.Trim().Contains(tag) && tag.Length > 0)
                {
                    tgs[tgs.Length - 1] = tg.Trim();

                    for (int i = 0; i < tgs.Length; i++)
                    {
                        tgs[i] = tgs[i].Trim();
                    }

                    tagsString = String.Join(", ", tgs);

                    autoCompleteText.Text = tagsString;

                }
            }
        }

        private void Tags_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Tags.Text = autoCompleteText.Text;

                Tags.CaretIndex = Tags.Text.Length;  //Cursor to Last position
                
            }
        }
    }
}
