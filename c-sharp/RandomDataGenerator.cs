namespace Barin.RandomDataGen
{
  public static class RandomDataGen 
  {
  
    public static V Tweak<T, V>(
            this V original, T val, string name)
    {
        var t = typeof(DomesticTransaction);

        var prop = t.GetProperty(name);

        prop.SetValue(original, val);

        return original;
    }
  
    public static T RandomOf<T>()
    {
        var d = Seed<T>;

        foreach (var prop in typeof(T).GetProperties())
        {
            var newVal = RandomBy(prop.PropertyType);

            prop.SetValue(d, newVal);
        }

        return d;
    }
    
    public static dynamic RandomBy(Type t) 
    {
        var rnd = new Random();

        Func<dynamic> rndStr = () =>
            new string(Enumerable
                .Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10)
                .Select(s => s[rnd.Next(s.Length)])
                .ToArray());

        var pool = new Dictionary<Type, Func<dynamic>>() {
            { typeof(string), rndStr},
            { typeof(int), () => rnd.Next(-100, 100)},
            { typeof(uint), () => Convert.ToUInt32(rnd.Next(1, 200))},
            { typeof(ulong), () => Convert.ToUInt64(rnd.Next(1, 10000))},
            { typeof(decimal), () => Math.Round(new decimal(rnd.NextDouble()), 2)},
        };

        Func <dynamic> f;

        return pool.TryGetValue(t, out f) ? f() : null;
    }
  }
}
