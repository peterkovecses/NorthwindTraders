namespace Northwind.Application.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public int IntId { get; init; }
        public string? StringId { get; init; }
        public (int, int) IntTupleId { get; init; }

        public ItemNotFoundException(int intId) : base($"Item with Id {intId} does not exist.")
        {
            IntId = intId;
        }

        public ItemNotFoundException(string stringId) : base($"Item with Id {stringId} does not exist.")
        {
            StringId = stringId;
        }

        public ItemNotFoundException((int, int) intTupleId) : base($"Item with Id {intTupleId} does not exist.")
        {
            IntTupleId = intTupleId;
        }
    }
}
