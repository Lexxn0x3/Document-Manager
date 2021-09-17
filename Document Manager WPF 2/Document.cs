using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document_Manager_WPF_2
{
    public class Document
    {
        public string? Name { get; set; }
        public Uri? Url { get; set; }
        public string[]? Tags { get; set; }

        public Document(string name, Uri url, string[] tags)
        {
            this.Name = name;
            this.Url = url;
            this.Tags = tags;
        }
        public Document(string name, Uri url, string tags)
        {
            this.Name = name;
            this.Url = url;



            this.Tags = stringToTagList(tags);

            //string[] splitTags = tags.Split(',');
            //for (int i = 0; i < splitTags.Length; i++)
            //{
            //    splitTags[i] = splitTags[i].Trim();
            //}

            //this.Tags = splitTags;
        }

        public Document()
        {
            this.Name = "";
            this.Url = null;
            this.Tags = new string[0];
        }

        static public string[] stringToTagList(string tags)
        {
            string[] splitTags = tags.Split(',');

            for (int i = 0; i < splitTags.Length; i++)
            {
                splitTags[i] = splitTags[i].Trim();
            }

            return (splitTags);
        }

        static public string arrayToTagString(string[] tags)
        {
            string text = "";
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] != "")
                {
                    if (i == 0)
                    {
                        text += tags[i];
                    }
                    else
                    {
                        text += ", " + tags[i];
                    }
                }
            }
            return text;
        }
    }
}
