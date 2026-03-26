namespace KK.Agent.Library.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddRange<K, V>(this Dictionary<K, V> target,
            IEnumerable<KeyValuePair<K, V>> source)
        {
            foreach (var kvp in source)
            {
                target[kvp.Key] = kvp.Value;
            }
        }

        public static void AddRangeWithoutOverwrite<K, V>(this Dictionary<K, V> target,
            IEnumerable<KeyValuePair<K, V>> source)
        {
            foreach (var kvp in source)
            {
                if (!target.ContainsKey(kvp.Key))
                    target[kvp.Key] = kvp.Value;
            }
        }
    }
}
