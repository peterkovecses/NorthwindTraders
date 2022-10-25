﻿using Northwind.Application.Interfaces;

namespace Northwind.Application.Models
{
    public class QueryParameters<T> where T : IFilter, new()
    {
        public Pagination Pagination { get; init; } = new Pagination();
        public Sorting Sorting { get; init; } = new Sorting();
        public T Filter { get; init; } = new T();
    }
}
