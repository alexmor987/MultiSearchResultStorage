using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Services.GoogleSearch
{
    public class GoogleSearchResponse
    {
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string Kind { get; set; }
        public string Title { get; set; }
        public string HtmlTitle { get; set; }
        public string Link { get; set; }
        public string DisplayLink { get; set; }
        public string Snippet { get; set; }
        public string HtmlSnippet { get; set; }
        public string FormattedUrl { get; set; }
        public string HtmlFormattedUrl { get; set; }
        public Pagemap Pagemap { get; set; }
    }

    public class Pagemap
    {
        public List<Metatags> Metatags { get; set; }
        public List<CseImage> CseImage { get; set; }
    }

    public class Metatags
    {
    }

    public class CseImage
    {
        public string Src { get; set; }
    }

}
