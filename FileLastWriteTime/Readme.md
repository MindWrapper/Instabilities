# Project description

This project demonstrates a flaky test build around incorrect assumption about file last time access timestamp granularity on different platforms (Win, OSX).

# Wrong assumption
The file was changed on recompile. 
So timestamp of a initially compiled file is always older than the newer, recompiled file.

Run `Compile_ValidCode_AlwaysRecompilesDLL` test several times to see that on OSX it will either pass or fail, while on Win it is always passing.
```
[Test]
public void Compile_ValidCode_AlwaysRecompilesDLL()
{
    var compiledPath = Compile("");
    var compiledFileTimestamp = File.GetLastWriteTime(compiledPath);

    var recompiledPath = Compile("");
    var recompiledFileTimestamp = File.GetLastWriteTime(recompiledPath);

    Assert.That(compiledFileTimestamp, Is.LessThan(recompiledFileTimestamp));
}
```

# Why is it wrong?
Different platforms, diffrent rules!

Windows timestamp insludes misiseconds, so our assumption is totally valid.
But OSX has a different granularity. It stores timestamps to a granularity of 1 second. 
So, depending on how "quick" you code is, the test may pass or fail. 

For example, if both compile and recompile are called within 1 second timeframe, the test will turn red.
Test fails:
```
 Errors, Failures and Warnings

1) Failed : Instabilities.FileLastWriteTime.FileLastWriteTime.Compile_ValidCode_AlwaysRecompilesDLL
  Expected: less than 2018-01-10 18:53:53
  But was:  2018-01-10 18:53:53
  at Instabilities.FileLastWriteTime.FileLastWriteTime.Compile_ValidCode_AlwaysRecompilesDLL () [0x00033] in <68dc093641884782952363535085c74a>:0 

Test Run Summary
  Overall result: Failed
  Test Count: 1, Passed: 0, Failed: 1, Warnings: 0, Inconclusive: 0, Skipped: 0
```

If the code takes longer to execute, or you were lucky to invoke the second recompile closer the end of 1 second frame, 
the test run will be green.
```
Test Run Summary
  Overall result: Passed
  Test Count: 1, Passed: 1, Failed: 0, Warnings: 0, Inconclusive: 0, Skipped: 0
```

# Why it survived
The test has not been run on all supported platforms. It has been executed only once, before commiting.

# How to avoid
Make sure that the file's last access time should differ at least by 1 second.
Run test multiple times.
