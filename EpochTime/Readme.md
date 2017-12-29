# Project description

This projects demonstrates a flaky test.

# Wrong assumption

The assertion bellow is always true.

```
static DateTime s_Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();

private static long GetUtcNowMs()
{
    return Convert.ToInt64((DateTime.UtcNow - s_Epoch).TotalMilliseconds);
}

Assert.That(GetUtcNowMs(), Is.LessThanOrEqualTo (
    (DateTime.UtcNow - s_Epoch).TotalMilliseconds)
);
```

# Why is it wrong?

`Is.LessThanOrEqualTo` hides type conversion. GetUtcNowMs return long. TotalMilliseconds returns double, which greater, because decimal point is still there

Possible outcomes:

Test passes: 
![](https://i.imgur.com/v5BgDX3.png)

Test fails:
```
 Expected: less than or equal to 1514451833690.5906d
  But was:  1514451833691
```

# Why it survided

It is easy to forget that `TimeSpan.TotalMilliseconds` returns double. `Is.LessThanOrEqualTo` hid long to floating-point comparison.

# How to avoid



Avoid such LessThanOrEqualTo comparisons. Make GetUtcNowMs mockable and compare against a concreate value.


The most of the code analyzers will not complain on the code like this:
```
double d = 5;
long i = 5;
if (i <= d)
{
    Console.WriteLine("<=");
}
```
If you put `==` instead, you'll most likely get a warning


