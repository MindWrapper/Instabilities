# Project description

`Instabilities.ProcessOutputConsumer` is a console program, which starts  `Instabilities.ProcessOutputProducer` and pass it an argument, saying how many lines ProcessOutputProducer must produce.
When read messages `ProcessOutputConsumer` accumulates them in a string queue.

# Wrong assumption

`ProcessOutputConsumer` will process all the messages produced by `ProcessOutputProducer`, so `actualMessageCount` is equal to a requested message count.

# Why it is wrong

Very simple. `queue` is a subject of race condition. 


```
p.OutputDataReceived += (sender, eventArgs) =>
{
    if (!string.IsNullOrEmpty(eventArgs.Data))
    {
        processOutout.Enqueue(eventArgs.Data);
    }
};
```

`OutputDataReceived` is not invoked in the main thread and therefor there is a race condition on a `processOutout` object.

Possible outcomes:

1. Everything is OK.

```
Lines read: 100 Expected: 100
Lines read: 1000 Expected: 1000
Lines read: 10000 Expected: 10000
Press any key to continue . . .
```

2. Some messages are missed


```
Lines read: 100 Expected: 100
Lines read: 1000 Expected: 1000
Lines read: 9999 Expected: 10000
Press any key to continue
```

3. "Done" message is missed

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


# Some obesravations

- Longer Thread.Sleep value is, less likely it is to get outcome #4.
- Adding  


