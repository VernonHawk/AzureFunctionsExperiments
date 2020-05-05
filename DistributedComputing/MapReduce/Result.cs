namespace DistributedComputing.MapReduce
{
    public class Result<KeyT, ValueT>
    {
        public KeyT Key { get; set; }
        public ValueT Value { get; set; }

        public Result(KeyT key, ValueT value) => (Key, Value) = (key, value);

        public override string ToString() => $"{Key}: {Value}";
    }
}