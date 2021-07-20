using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Document_Manager
{
    class Tree
    {
        public List<Tree> Childs { get; set; }
        public string Header { get; set; }

        public Tree() { }
        public Tree(string header, List<Tree> childs)
        {
            this.Header = header;
            this.Childs = childs;
        }
        public Tree(string header)
        {
            this.Header = header;
            this.Childs = null;
        }

        public void Add(Tree treItem)
        {
            if (Childs == null)
            {
                Childs = new List<Tree>();
            }
            Childs.Add(treItem);
        }

        private string GetString(List<Tree> Children)
        {
            string file = "";
            if (Children != null)
            {
                foreach (Tree child in Children)
                {
                    string element = "";

                    element += child.Header + "\n";
                    try
                    {
                        element += (Count(child).ToString()) + "\n\n";
                        //element += (child.Childs.Count()).ToString() + "\n\n";
                    }
                    catch (ArgumentNullException)
                    {

                        element += 0 + "\n\n";
                    }

                    string chd = GetString(child.Childs);
                    if (chd != "" && chd != null)
                    {
                        element += GetString(child.Childs);
                    }


                    file += element;
                }
            }
            return (file);
        }

        private int Count(Tree child)
        {
            int count = 0;

            if (child.Childs.Count() > 0)
            {
                foreach (Tree c in child.Childs)
                {
                    count++;
                    if (c.Childs != null)
                    {
                        count += Count(c);
                    }
                }
            }

            return count;
        }
        private TreeViewItem GetTree(Queue<string[]> queue, TreeViewItem root)   //To be implemented
        {
            //TreeViewItem root = new TreeViewItem();

            while (queue.Count > 0)
            {
                string[] block = queue.Dequeue();

                TreeViewItem newChild = new TreeViewItem();
                newChild.Header = block[0];
                newChild.Foreground = Brushes.White;

                

                if (int.Parse(block[1]) > 0)
                {
                    Queue<string[]> newQueue = new Queue<string[]>();
                    for (int i = 0; i < int.Parse(block[1]); i++)
                    {
                        newQueue.Enqueue(queue.Dequeue());
                    }

                    root.Items.Add(GetTree(newQueue, newChild));
                }
                else
                {
                    root.Items.Add(newChild);
                }
                
            }


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

            return (root);
        }

        public void WriteFile()
        {
            string file = GetString(Childs);
            string path = Directory.GetCurrentDirectory() + @"\tree.dat";
            File.WriteAllText(path, file);
        }
        public TreeViewItem Readfile()
        {
            string[] file;
            string path = Directory.GetCurrentDirectory() + @"\tree.dat";
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            else
            {
                file = File.ReadAllLines(path);

                int counter = 0;

                Queue<string[]> blocks = new Queue<string[]>();

                string[] block = new string[2];

                foreach (string line in file)
                {
                    if (counter == 0)
                    {
                        block[0] = line;
                        counter++;
                    }
                    else if (counter == 1)
                    {
                        block[1] = line;
                        counter++;
                    }
                    else
                    {
                        counter = 0;
                        blocks.Enqueue(block);
                        block = new string[2];
                    }
                }

                TreeViewItem tree = new TreeViewItem();
                tree.Header = "Root";
                tree.Foreground = Brushes.White;
                return (GetTree(blocks, tree));
            }
            return (new TreeViewItem());
            
        }
    }
}
