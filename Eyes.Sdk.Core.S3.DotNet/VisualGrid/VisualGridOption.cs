namespace Applitools.VisualGrid
{
    public struct VisualGridOption
    {
        public VisualGridOption(string key, object value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public object Value { get; set; }
    }
}