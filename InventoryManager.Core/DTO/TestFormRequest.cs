using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class TestFormRequest
    {
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }


        public string? Email { get; set;}
        public string? Password { get; set; }
    }
}
