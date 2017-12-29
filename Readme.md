When writing a code we often assume things work in one way, but they work in another. Usually, we discover it fast, especially if we practice an early testing. However, some wrong assumptions work as we expect, survive testing and eventually sneak into released versions. It is one of the ways how a software product got infected with instabilities.

Instability has many faces: random crashes, sporadic data corruption, flaky tests and many others. None of them are good. In addition, they are hard to reproduce and fix. And they might be even a cause why people burn-out or get fired.

If a wrong assumption survived someone’s critical thinking, there is a good chance that others made the same mistake. Therefore I decided to start this project, where I’ll be sharing my findings.

I encourage you to share your findings. Imagine, you spent days of debugging and found a true source of instability and shared it. Someone reads and claims “Shit, I might have the same issue in my code base!”.  It is definitely a plus to your karma. Whether it is a text description or code, or both, it should be simple. So that other people can comprehend this. Any programming languages are welcome.

And one final thing. Often, dumbest instabilities are hiding under umbrella an of complexity. When distilled, it might feel shameful that something like this was a part of your code base. It should not stop you from sharing it. You can still explain why it was hard to discover and if there is a way how other people might discover it.
