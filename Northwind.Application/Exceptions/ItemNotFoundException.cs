namespace Northwind.Application.Exceptions
{
    public abstract class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class ItemNotFoundException<T> : ItemNotFoundException
    {
        public T Id { get; init; }

        public ItemNotFoundException(T Id) : base($"Item with Id {Id} does not exist.")
        {
            this.Id = Id;
        }
    }
}
