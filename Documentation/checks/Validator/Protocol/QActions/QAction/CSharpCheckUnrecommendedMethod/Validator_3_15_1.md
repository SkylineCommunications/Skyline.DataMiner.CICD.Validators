---
uid: Validator_3_15_1
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedThreadAbort

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

The Thread.Abort method should be used with caution.
Particularly when you call it to abort a thread other than the current thread, you do not know what code has executed or failed to execute when the ThreadAbortException is thrown.
You also cannot be certain of the state of your application or any application and user state that it's responsible for preserving.
For example, calling Thread.Abort may prevent the execution of static constructors or the release of unmanaged resources.

Instead, some logic to nicely end the thread should be implemented.
This is usually implemented according to the cooperative cancellation model.

<!-- Uncomment to add example code -->
<!--### Example code-->
