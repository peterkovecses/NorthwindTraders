using System.Runtime.Serialization;

namespace Northwind.Application.Exceptions
{
    [Serializable]
    public class PropertyNotFoundException : Exception
    {
        public string? PropertyName { get; set; }

        public override string Message => PropertyName != null ? $"Property {PropertyName} does not exist." : "Property does not exist.";

        public PropertyNotFoundException()
        {
        }

        public PropertyNotFoundException(string propertyName)
        {
            PropertyName = propertyName;
        }

        public PropertyNotFoundException(string propertyName, Exception innerException) 
            : base(message: null, innerException)
        {
            PropertyName = propertyName;
        }

        protected PropertyNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PropertyName = (string)info.GetValue(nameof(PropertyName), typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(PropertyName), PropertyName, typeof(string));
        }
    }
}
