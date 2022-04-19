namespace Applitools
{
    using System;
    using System.Runtime.Serialization;
    using Utils.Geometry;

    [Serializable]
    public class CoordinatesTypeConversionException : Exception
    {
        private readonly CoordinatesTypeEnum from;
        private readonly CoordinatesTypeEnum to;

        public CoordinatesTypeConversionException()
        {
        }

        public CoordinatesTypeConversionException(string message) : base(message)
        {
        }

        public CoordinatesTypeConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CoordinatesTypeConversionException(CoordinatesTypeEnum from, CoordinatesTypeEnum to)
        {
            this.from = from;
            this.to = to;
        }

        protected CoordinatesTypeConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}