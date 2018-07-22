using System;
using System.Collections.Generic;
using System.Text;

namespace Services.AuthService.Models
{
    public class Node
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public Guid FieldID { get; set; }
        public bool IsFolder { get; set; }
    }
}
