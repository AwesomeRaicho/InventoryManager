﻿using InventoryManager.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class PropertyInstanceGetRequest
    {
        public string? SearchText { get; set; }
        public string? PropertyTypeId { get; set; }
        public string? ProductTypeId { get; set; }

        //pagination

        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public OrderBy OrderBy { get; set; } = OrderBy.Asc;
    }
}
