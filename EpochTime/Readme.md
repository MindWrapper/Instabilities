# Project description

This project demonstrates a flaky test build around incorrect assumption about floating point rounding.

# Wrong assumption

The assertion bellow is always true.

```
[Test]
public void GetUTCNowMs_ReturnsValueLessOrEqualCurrentTime()
{
    var result = GetEpochUtcNowInMilliseconds();
    var now = (DateTime.UtcNow - s_Epoch).TotalMilliseconds;
    Assert.That(result, Is.LessThanOrEqualTo(now));
}

static DateTime s_Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();

private static long GetEpochUtcNowInMilliseconds()
{
    return Convert.ToInt64((DateTime.UtcNow - s_Epoch).TotalMilliseconds);
}
```

# Why is it wrong?

First, let's consider when the test above fails.

For simplicity let suppose that `(DateTime.UtcNow - s_Epoch).TotalMilliseconds` 
inside `GetEpochUtcNowInMilliseconds` returns 0.7. `Convert.ToInt64(0.7)` returns a value rounded to the 
nearest 64-bit signed integer which is `1` 

So `result` inside the test got assigned to 1. Now suppose that `now` got assigned to 0.8. It gives us: 
`Assert.That(1, Is.LessThanOrEqualTo(0.8));`
which is obviously false.

Let's consider when test passes:

`(DateTime.UtcNow - s_Epoch).TotalMilliseconds` inside `GetEpochUtcNowInMilliseconds` return 0.3, 
which is rounded to 0.

So, inside the test `result` holds `1`. Now let's say `now` equals to `0.5`. It gives us:
`Assert.That(0, Is.LessThanOrEqualTo(0.5));`

Which is true

In real-world numbers are bigger, so the test fails like this:

Test fails:
```
 Expected: less than or equal to 1514451833690.5906d
  But was:  1514451833691
```


# Why it survived

It is easy to forget that `TimeSpan.TotalMilliseconds` returns double. In addition
compiler will not warn you about integer to floating-point comparison using `<=`
(which is what `LessThanOrEqualTo` does).

Try this to check:
```
double d = 5;
long i = 5;
if (i <= d)
{
    Console.WriteLine("<=");
}
```
If it was `==` instead, the most of the tools would warn.

# How to avoid

Make `GetEpochUtcNowInMilliseconds` mockable and compare against a concrete value.
Run test multiple times.





