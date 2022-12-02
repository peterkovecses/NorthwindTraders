namespace Northwind.Application.Exceptions
{
    public class ItemNotFoundException<T> : Exception
    {
        public T Id { get; init; }

        public ItemNotFoundException(T Id) : base($"Item with Id {Id} does not exist.")
        {
            this.Id = Id;
        }
    }
}
