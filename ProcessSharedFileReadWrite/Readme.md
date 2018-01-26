# Project description

There are two processes reader process and writer process (Instabilities.ProcessSharedFileReader and Instabilities.ProcessSharedFileWriter respectively)
Writer process writes to 

Reader process starts writer process respectively. Writer process receives 3 parameters:

```
var linesCount = int.Parse(args[0]);
var lineLength = int.Parse(args[1]);
var fileName = args[2];
```

which tells it, how many lines of which length to write into the faileName.

Reader process reads lines from a given file, using StreamReader.ReadLine() and checks
if each lines has expected length and if there are expected number of lines.

Instabilities.ProcessSharedFileReader starts Instabilities.ProcessSharedFileWriter

#Wrong assumption

Reader process will read all the lines and all line will have an expected length.

#Why it is wrong

Writer process has some random delays (In real application these delays can happens for plenty of reasons)

```
for (var i = 0; i < linesCount; ++i) 
{
    Thread.Sleep(random.Next(0, 5));
    sw.WriteLine(line);
}
```

These delay might cause reader process to think what there is no more data in the input stream and flush a stream buffer, 
which could contain an incomplete string.

Possible outcomes:

```
Read lines count: 1017 Expected 1000. Incomplete lines count 34
Press any key to continue . . .
```

#Some observations

Setting `sw.Autoflush = true` in the writer  improves the situation, but still 
if we set `linesCount` and line `lineLength` to higher values:

```
linesCount = 100000
lineLength = 100000
```

It still happens

```
Read lines count: 100001 Expected 100000. Incomplete lines count 2
Press any key to continue . . .
```

#Why it survived
-  `streamReader.ReadLine()` looked  innocent to draw someone's attention
-   One of the unit tests for related classed detected never shown this problem

#How to find it in your code-base

Just analyze the code responsible for a log reading from a process.

#How to fix

I came up with this fix. Might not have the best performance, but does the job. TODO

