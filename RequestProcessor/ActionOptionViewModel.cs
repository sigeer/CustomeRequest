using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestProcessor
{
    public class ActionOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string FromType { get; set;} = null!;
        public string ReturnType { get; set; } = null!;
        public List<string> Params { get; set; } = new List<string>();
    }
}
