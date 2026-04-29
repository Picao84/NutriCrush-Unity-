using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class FilterEvent
    {
        public bool isDescending {  get; }

        public SortType sortType { get; }

        public FilterEvent(bool isDescending, SortType sortType)
        {
            this.isDescending = isDescending;
            this.sortType = sortType;
        }   
    }
}
