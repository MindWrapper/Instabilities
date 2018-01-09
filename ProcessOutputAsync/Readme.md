# Project description

`ProcessOutputConsumer` is a console program, which starts  `ProcessOutputProducer` and pass it an integer, saying 
how many lines it must produce. `ProcessOutputConsumer` receives messages using async API's and accumulates them in a string queue -
`processOutputQueue`

To run it go to ProcessOutputAsync folder and set  `Instabilities.ProcessOutputProducer` as a startup project.


# Wrong assumption

`ProcessOutputConsumer` will process all the messages produced by `ProcessOutputProducer`, so `actualMessageCount` is equal 
to a requested message count.

# Why it is wrong

`processOutputQueue` queue is a subject of a race condition. Look at this callback:

```
p.OutputDataReceived += (sender, eventArgs) =>
{
    processOutputQueue.Enqueue(eventArgs.Data);
};
```

`OutputDataReceived` is not invoked in the main thread and therefore there is a race condition on a `processOutputQueue` object.

Possible outcomes:

1. Everything is OK.
All iterations produced and expected output.
```
Lines read: 1000 Expected: 1000
...
```

2. Some messages are missed

```
Lines read: 1000 Expected: 1000
Lines read: 1000 Expected: 1000
Lines read: 999 Expected: 1000
Instability found!
```

3. "Done" message is missed or instead  null messages is dequeued `CountProducedLines`

If this case `ProcessOutputConsumer` will hang.

4. Queue throws an exception

Depending on the framework version, exception might vary in my case it was 
 
```
Unhandled Exception: System.ArgumentException: Source array was not long enough. Check srcIndex and length, and the array's lower bounds.
   at System.Array.Copy(Array sourceArray, Int32 sourceIndex, Array destinationArray, Int32 destinationIndex, Int32 length, Boolean reliable)
   at System.Array.Copy(Array sourceArray, Int32 sourceIndex, Array destinationArray, Int32 destinationIndex, Int32 length)
   at System.Collections.Generic.Queue`1.SetCapacity(Int32 capacity)
   at System.Collections.Generic.Queue`1.Enqueue(T item)
   at Instabilities.ProcessOutputConsumer.Program.<>c__DisplayClass2_0.<StartProducerProcess>b__0(Object sender, DataReceivedEventArgs eventArgs) in C:\sandbox\Instabilities\Instabilities\Instabilities.ProcessOutputConsumer\Program.cs:line 46
   at System.Diagnostics.AsyncStreamReader.FlushMessageQueue()
   at System.Diagnostics.AsyncStreamReader.GetLinesFromStringBuilder()
   at System.Diagnostics.AsyncStreamReader.ReadBuffer(IAsyncResult ar)
   at System.Runtime.Remoting.Messaging.AsyncResult.SyncProcessMessage(IMessage msg)
   at System.Runtime.Remoting.Messaging.StackBuilderSink.AsyncProcessMessage(IMessage msg, IMessageSink replySink)
   at System.Runtime.Remoting.Proxies.AgileAsyncWorkerItem.ThreadPoolCallBack(Object o)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading._ThreadPoolWaitCallback.PerformWaitCallbackInternal(_ThreadPoolWaitCallback tpWaitCallBack)
   at System.Threading._ThreadPoolWaitCallback.PerformWaitCallback(Object state)
```

Looks like queue re-allocation blown up.

# Some obesrvations

- Longer Thread.Sleep value is, less likely it is to get outcome #4 and more likely it is to get #1 (everything is OK)
- Adding `Trace.WriteLine(msg)` into message reading loop will also increase chances for #1


# How it was detected

Consumer process was reading an output of Producer process. Consumer was expecting some control messages. 
Rarely these control messages didn't arrive. It didn't show up to often and therefore was not causing a significant pain. 
Until one day logging traffic increased. We found that `Producer's` logs, saved on disk, contained all missed control messages, 
which made us to start look into what was wrong with a Consumer's code. 

We build a test which stresses the same set classes as production code. In a couple of iterations we distilled it into this project.

# Why it survived

- One of the unit tests for related classed detected never shown this problem.
- Wrong assumption is that `p.OutputDataReceived` is invoked in the same thread. It is _not_. I still see `Console.WriteLine` 
inside this callback. This could produce a data race on `System.Console` if main tread is still executing.
`p.OutputDataReceived` was too hidden

# How to find it in your codebase

Search for `OutputDataReceived` and `ErrorDataReceived` and check what they do.

For each message you log, add a thread id. Are there any logging in 'unexpected' threads? 
One of the way to do it (if you logging system can not) is ti hook Console.System.Out.

# How to fix
- synchronize access to queue or use one of the existing thread-safe queues
- get rid of shared data
- ensure no other work is going on other thread while processing output.

# Other variations

If Console.WriteLine is invoked within the callback it might cause a race condition on Console.Out and therefore output might be screwed up.

