using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
   public class SearchModel
    {
        public int draw { get; set; }
        public IList<Column> columns { get; set; }
        public IList<Order> order { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
    }
    public class Search
    {
        public string value { get; set; }
    }

    public class Column
    {
        public string name { get; set; }
        public Search search { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public class SearchObjectModel
    {
        public int draw { get; set; }
        public IList<Column> columns { get; set; }
        public IList<Order> order { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public string appId { get; set; }
    }
}
