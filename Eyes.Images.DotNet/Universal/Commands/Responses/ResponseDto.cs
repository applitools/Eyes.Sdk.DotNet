namespace Applitools.Commands.Responses
{
    public class ResponseDto<T>
    {
        public ResponseDto()
        {
        }

        public ResponseDto(string name, string key, ResponsePayload<T> payload)
        {
            Name = name;
            Key = key;
            Payload = payload;
        }

        public string Name { get; set; }

        public string Key { get; set; }

        public ResponsePayload<T> Payload { get; set; }
    }

    public class ResponsePayload<T>
    {
        public ResponsePayload()
        {
        }

        public ResponsePayload(T result, ResponsePayloadError error = null)
        {
            Result = result;
            Error = error;
        }

        public ResponsePayloadError Error { get; set; }

        public T Result { get; set; }
    }
}