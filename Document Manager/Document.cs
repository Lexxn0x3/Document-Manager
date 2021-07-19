using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document_Manager
{
    class Document
    {
        public Uri Url { get; set; }
        public string Name { get; set; }
        public List<string> Tags { get; set; }

        string path = Directory.GetCurrentDirectory() + @"\data.dat";

        public Document(Uri url, string name, params string[] tags)
        {
            this.Tags = new List<string>();
            this.Url = url;
            this.Name = name;

            foreach (string tag in tags)
            {
                this.Tags.Add(tag);
            }
        }

        public Document(Uri url, string name, List<string> tags)
        {
            this.Tags = new List<string>();
            this.Url = url;
            this.Name = name;

            this.Tags = tags;
        }

        public Document() { }

        public List<Document> ReadFile()
        {
            if (System.IO.File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\robin\Desktop\data.txt");

                int dataLine = 1;

                Uri url = null;
                string name = null;
                List<string> tags = new List<string>();
                List<Document> docs = new List<Document>();

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
                                string url2 = url.ToString() + "#toolbar=0";
                                url = new Uri(url2, UriKind.Absolute);
                                break;
                        }
                        dataLine++;
                    }
                    else
                    {
                        docs.Add(new Document(url, name, tags));
                        dataLine = 1;
                        url = null;
                        name = null;
                        tags = new List<string>();
                    }

                }
                return (docs);
            }
            else
                return new List<Document>();
        }

        public void WriteFile(List<Document> docs)
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

            //string[] lines = file.Split("\n");

            File.WriteAllText(path, file);
        }
    }
}
