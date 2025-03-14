using InventoryManager.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class ProductInstanceGetRequest
    {
        public string? SearchText { get; set; }
        public string? ProductId { get; set; }



        public int PageNumber { get; set; } = 0;

        public int PageSize { get; set; } = 0;

        /// <summary>
        /// The 2 string values will be 'entrydate' or 'status'
        /// </summary>
        public string? OrderByColumn { get; set; }
        public OrderBy OrderBy { get; set; } = OrderBy.Desc;
    }
}
