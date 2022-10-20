namespace Northwind.Application.Exceptions
{
    [Serializable]
    public class PropertyNotFoundException : Exception
    {
        public string PropertyName { get; set; }

        public PropertyNotFoundException(string propertyName) : base($"Property {propertyName} does not exist.")
        {
            PropertyName = propertyName;
        }
    }
}
