namespace CssParser.Model.Rules
{
    public class Tup<T1, T2>
    {
        public T1 Item1 { get; internal set; }
        public T2 Item2 { get; internal set; }

        public static Tup<T1, T2> Create(T1 item1, T2 item2)
        {
            Tup<T1, T2> tuple = new Tup<T1, T2>()
            {
                Item1 = item1,
                Item2 = item2
            };

            return tuple;
        }
    }
}