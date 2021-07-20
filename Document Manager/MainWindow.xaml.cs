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

namespace Document_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static List<Document> docs = new List<Document>();
        Tags tags = new Tags(); //Auto Read from File
        public int MyProperty { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            web.Visibility = Visibility.Hidden;

            //tags.AddTag("Siemens");
            //tags.AddTag("Arbeit");
            //tags.AddTag("Rechnung");

            Document dc = new Document();
            docs = dc.ReadFile();
            //tags.ReadFile();

            

            UpdateTree();

            //docs.Add(new Document(new Uri("file:///D:/Dokumente/Siemens/Siemens%20Ablauf%20Vorbereitung.pdf#toolbar=0"), "Siemens Ablauf Vorbereitung", tags.TagList[0]));
            //docs.Add(new Document(new Uri("file:///D:/Dokumente/Siemens/Siemens%20vorbereitung.pdf#toolbar=0"), "Siemens Vorbereitung", tags.TagList[0]));
            //docs.Add(new Document(new Uri("file:///D:/Dokumente/Siemens/Einstellung/Vertrag.pdf#toolbar=0"), "Siemens Vertrag", tags.TagList[0], tags.TagList[1]));
            //docs.Add(new Document(new Uri("file:///D:/Dokumente/Siemens/Einstellung/Vertrag.pdf#toolbar=0"), "Siemens Vertrag1", tags.TagList[0], tags.TagList[1]));
            //docs.Add(new Document(new Uri("file:///D:/Downloads/I210002769840%20(1).pdf#toolbar=0"), "Hetzner Rechnung", tags.TagList[2]));

            //foreach (string tag in tags.TagList)
            //{
            //    TreeViewItem newChild = new TreeViewItem();
            //    newChild.Header = tag;
            //    newChild.Foreground = Brushes.White;

            //    foreach (Document doc in docs)
            //    {
            //        if (doc.Tags.Contains(tag))
            //        {
            //            TreeViewItem newChild2 = new TreeViewItem();
            //            newChild2.Header = doc.Name;
            //            newChild2.DataContext = doc;
            //            newChild2.Foreground = Brushes.White;

            //            newChild.Items.Add(newChild2);
            //        }
            //    }
            //    trv.Items.Add(newChild);
            //}

            //UpdateTree();
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
                    //MessageBox.Show(((info.Length)*0.000001).ToString());
                }
                else
                    web.Visibility = Visibility.Hidden;
            }

        }

        private void MenuItem_New_Document_Click(object sender, RoutedEventArgs e)
        {
            Add_Document w = new Add_Document();
            w.ShowDialog();

            Document dc = new Document();
            docs = dc.ReadFile();
            tags.TagList = tags.ReadFile();
            trv.Items.Clear();
            UpdateTree();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {

            }
            
        }

        private void TextBlock_MouseDown_Close(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void TextBlock_MouseDown_Minimize(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TextBlock_MouseDown_Maximize(object sender, MouseButtonEventArgs e)
        {
            if (Header_MaximizeNormal.Visibility == Visibility.Hidden)
            {
                this.WindowState = WindowState.Maximized;
                Header_MaximizeNormal.Visibility = Visibility.Visible;
                Header_Maximize.Visibility = Visibility.Hidden;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                Header_MaximizeNormal.Visibility = Visibility.Hidden;
                Header_Maximize.Visibility = Visibility.Visible;
            }
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
                foreach (TreeViewItem child in tree.Items)
                {
                    tags.Push("" + child.Header);
                    if (child.Items.Count > 0)
                    {
                        fillTree(child, tags);
                        tags.Pop();
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
                                newChild2.Foreground = Brushes.White;
                                child.Items.Add(newChild2);
                            }
                        }
                        return;
                    }
                }
                return;
            }
            //    foreach (TreeViewItem child in tree.Items)
            //    {
            //        tags.Push( " " + child.Header);

            //        Stack<string> tag = new Stack<string>();
            //        tag.Push(fillTree(child, tags, stack));
                        
            //        if (tag.Pop() != "")
            //        {
            //            tags.Push(fillTree(child, tags, stack));
            //        }
            //        else
            //        {
            //            foreach (Document doc in docs)
            //            {
            //                bool contains = true;
            //                foreach (string newTag in tags)
            //                {
            //                    if (!doc.Tags.Contains(newTag))
            //                    {
            //                        contains = false;
            //                    }
            //                }

            //                if (contains && tags.Count >0)
            //                {
            //                    TreeViewItem newChild2 = new TreeViewItem();
            //                    newChild2.Header = doc.Name;
            //                    newChild2.DataContext = doc;
            //                    newChild2.Foreground = Brushes.White;
            //                    child.Items.Add(newChild2);
            //                }
                            
            //            }
            //        }
                    
            //    }
            //}
            //else
            //{
            //    //stack.Push(tags);
            //    //tags.Clear();
            //    tags.Push("");
            //    return (tags.Pop());
                
                
            //}
            //return ("");
            
        }
        private void UpdateTree()
        {
            trv.Items.Clear();
            Tree myTree = new Tree();
            trv.Items.Add(myTree.Readfile());
            TreeViewItem tree = (TreeViewItem)trv.Items[0];
            fillTree(tree, new Stack<string>());


            //foreach (string tag in tags.TagList)
            //{
            //    TreeViewItem newChild = new TreeViewItem();
            //    newChild.Header = tag;
            //    newChild.Foreground = Brushes.White;

            //    foreach (Document doc in docs)
            //    {
            //        if (doc.Tags.Contains(tag))
            //        {
            //            TreeViewItem newChild2 = new TreeViewItem();
            //            newChild2.Header = doc.Name;
            //            newChild2.DataContext = doc;
            //            newChild2.Foreground = Brushes.White;

            //            newChild.Items.Add(newChild2);
            //        }
            //    }
            //    trv.Items.Add(newChild);
            //}
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
                }
 
            }
        }
    }
}
