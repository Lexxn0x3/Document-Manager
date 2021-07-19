using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
                        element += (child.Childs.Count()).ToString() + "\n";
                    }
                    catch (ArgumentNullException)
                    {

                        element += 0 + "\n";
                    }

                    string chd = GetString(child.Childs);
                    if (chd != "" && chd != null)
                    {
                        element += "\n" + GetString(child.Childs);
                    }
                    

                    file += element + "\n";
                }
            }
            return (file);
        }

        private List<Tree> GetTree(string[] file, List<Tree> tree)   //To be implemented
        {
            List<TreeViewItem> treeViewItems = new List<TreeViewItem>();
            Tree element;
            int counter = 0;

            foreach (string line in file)
            {
                switch (counter)
                {
                    case 0:
                        element = new Tree();
                        element.Header = line;
                        counter++;

                        break;
                    case 1:
                        counter++;
                        break;
                    case 2:
                        counter = 0;
                        break;

                }

                
            }

            return(tree);
        }

        public void WriteFile()
        {
            string file = GetString(Childs);
            string path = Directory.GetCurrentDirectory() + @"\tree.dat";
            File.WriteAllText(path, file);
        }
        public void Readfile()
        {
            string path = Directory.GetCurrentDirectory() + @"\tree.dat";

            List<Tree> tree = GetTree(System.IO.File.ReadAllLines(path), Childs);
        }
    }
}
