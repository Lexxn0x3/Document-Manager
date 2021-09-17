using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document_Manager
{
    class Tags
    {
        public List<string> TagList = new List<string>();

        string path = MainWindow.getDataDirectory("tags.dat");

        public Tags()
        {
            TagList = ReadFile();
        }

        public bool AddTag(string tag)
        {
            if (TagList == null)
            {
                TagList = new List<string>();
                TagList.Add(tag);
                return true;
            }

            else if (!TagList.Contains(tag))
            {
                TagList.Add(tag);
                return true;
            }
            else
                return false;
        }

        public List<string> ReadFile()
        {
            if (File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    if (!TagList.Contains(line))
                        TagList.Add(line);
                }
            }

            return (TagList);
        }
        public void WriteFile(List<string> tags)
        {
            string file = "";
            foreach (string tg in tags)
            {
                file += (tg + "\n");
            }
            File.WriteAllText(path, file);
        }

        public void WriteFile()
        {
            string file = "";
            foreach (string tg in TagList)
            {
                file += (tg + "\n");
            }
            File.WriteAllText(path, file);
        }
    }
}
