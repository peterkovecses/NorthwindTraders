﻿using Northwind.Application.Interfaces;

namespace Northwind.Application.Models.Filters
{
    public class RegionFilter : IFilter
    {
        public string SearchTerm { get; set; } = null!;
    }
}