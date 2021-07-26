using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
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
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace Document_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        static List<Document> docs = new List<Document>();
        Tags tags = new Tags(); //Auto Read from File
        public int MyProperty { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            web.Visibility = Visibility.Hidden;

            Document dc = new Document();
            docs = dc.ReadFile();

            UpdateTree();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (((TreeViewItem)trv.SelectedItem) != null)
            {
                object selItem = ((TreeViewItem)trv.SelectedItem).DataContext;
                if (selItem is Document)
                {
                    Document doc = (Document)selItem;
                    web.Visibility = Visibility.Visible;
                    web.Source = doc.Url;


                    // Get Attributes for file.

                    FileInfo info = new FileInfo(doc.Url.LocalPath);
                    FileAttributes attributes = info.Attributes;

                    details.Text = string.Format("Name: {0}\nSize: {1}mbytes\nPath: {2}\nLast Write: {3}", info.Name, Math.Round(((info.Length) * 0.000001), 2).ToString(), info.FullName, info.LastWriteTime.ToString());

                    OpenDocumentButton.DataContext = (string)doc.Url.LocalPath;
                }
                else
                    web.Visibility = Visibility.Hidden;
            }

        }

        private void MenuItem_New_Document_Click(object sender, RoutedEventArgs e)
        {
            bool anotherDocument = false;
            string lastTags = "";

            do
            {
                Add_Document w = new Add_Document(lastTags);
                w.ShowDialog();

                anotherDocument = w.AnotherDocument;
                lastTags = w.AutoTags;

                UpdateTree();
            }
            while (anotherDocument == true);            
        }

        private TreeViewItem GetStructure(Tree myTree)
        {
           TreeViewItem newChild = new TreeViewItem();
            newChild.Foreground = Brushes.White;

            if (myTree.Childs != null)
            {
                foreach (Tree tree in myTree.Childs)
                {
                    newChild.Items.Add(GetStructure(tree));
                    newChild.Foreground = Brushes.Yellow;
                }
            }

            newChild.Header = myTree.Header;
            return (newChild);
        }

        private Tree ReturnStructure(TreeViewItem treeViewItem)
        {
            Tree newTree = new Tree();
            newTree.Header = (string)treeViewItem.Header;

            if (treeViewItem.Items != null)
            {
                foreach (TreeViewItem item in treeViewItem.Items)
                {
                    Tree child = new Tree();
                    child.Header = (string)item.Header;
                    child.Childs = ReturnStructure(item).Childs;

                    if (item.DataContext == null)
                    {
                        newTree.Add(child);
                    }
                    
                }
            }

            return (newTree);
        }

        private void fillTree(TreeViewItem tree, Stack<string> tags)
        {
            if (tree.Items.Count > 0)
            {
                tree.IsExpanded = true;                    //<----------------Not Permanant!!!----------

                foreach (TreeViewItem child in tree.Items)
                {
                    child.IsExpanded = true;                    //<----------------Not Permanant!!!----------

                    tags.Push("" + child.Header);
                    if (child.Items.Count > 0)
                    {
                        fillTree(child, tags);
                    }
                    else
                    {
                        foreach (Document doc in docs)
                        {
                            bool contains = true;
                            foreach (string newTag in tags)
                            {
                                if (!doc.Tags.Contains(newTag))
                                {
                                    contains = false;
                                }
                            }

                            if (contains && tags.Count > 0)
                            {
                                TreeViewItem newChild2 = new TreeViewItem();
                                newChild2.Header = doc.Name;
                                newChild2.DataContext = doc;
                                newChild2.Foreground = Brushes.DeepSkyBlue;
                                child.Items.Add(newChild2);
                                child.Foreground = Brushes.Yellow;
                            }
                            
                        }
                        tags.Pop();
                    }
                }
                try
                {
                    tags.Pop();
                }
                catch(Exception e) 
                {
                    MessageBox.Show(e.Message, "Error");
                }
                
                return;
            }            
        }
        private void UpdateTree()
        {
            Document dc = new Document();
            trv.Items.Clear();
            docs = dc.ReadFile();
            Tree myTree = new Tree();
            trv.Items.Add(myTree.Readfile());
            TreeViewItem tree = (TreeViewItem)trv.Items[0];
            fillTree(tree, new Stack<string>());
        }

        private void MenuItem_Click_New_Sub(object sender, RoutedEventArgs e)
        {
            var w = new AddTag();
            w.ShowDialog();

            string foo = w.Tags;

            if (trv.SelectedItem != null && foo != null)
            {
                if(foo != "" && foo.Length > 2)
                {
                    string[] tgs = foo.Split(",");

                    foreach (string tag in tgs)
                    {
                        TreeViewItem newChild2 = new TreeViewItem();
                        newChild2.Foreground = Brushes.White;
                        newChild2.Header = tag.Trim();
                        ((TreeViewItem)trv.SelectedItem).Items.Add(newChild2);

                        tags.AddTag(tag);
                    }


                    Tree tree = ReturnStructure((TreeViewItem)trv.Items[0]);
                    trv.Items.Clear();
                    trv.Items.Add(GetStructure(tree));
                    tree.WriteFile();

                    tags.WriteFile();

                    UpdateTree();
                }
 
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (MessageBox.Show("Delete item?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //do no stuff
                }
                else
                {
                    //do yes stuff
                }
            }
        }

        private void OpenDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFile((string)OpenDocumentButton.DataContext);
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
                MessageBox.Show("File no available", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
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


        private void OpenDocumentFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder((string)OpenDocumentButton.DataContext);
        }
    }
}
