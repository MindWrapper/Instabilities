Instability has many faces: random crashes, sporadic data corruption, flaky tests and others. They are hard to reproduce and fix. And they might be a cause why people burn-out or get fired. See  [Johnny's story](http://drugalya.com/johnny-the-automator/).

Each instability is an undetected wrong assumption, which survives testing and can an even sneak into a public release.

I believe that if an incorrect assumption survived someone's critical thinking, there is a chance that others made the same mistake. Therefore I decided to start this project, where I'll be sharing such incorrect assumptions.

I encourage you to share your findings. It can be anything: text description or code, or both. Whatever form you choose, please keep it simple. Any programming languages are welcome.

Imagine, you spent days of debugging, found a real source of instability and shared it. Someone reads your finding and claims "Shit, I might have the same issue in my code base!" It is a plus to karma.

And one final thing. Often, dumbest instabilities are hiding under the umbrella of complexity. When distilled, it might feel shameful that something like this was a part of your code base. Please share it anyway. You can still explain why it was hard to discover and if there is a way how others might find it.
