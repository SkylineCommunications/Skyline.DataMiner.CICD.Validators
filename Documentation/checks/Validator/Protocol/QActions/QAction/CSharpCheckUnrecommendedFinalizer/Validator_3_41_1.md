---
uid: Validator_3_41_1
---

# CSharpCheckUnrecommendedFinalizer

## UnrecommendedFinalizer

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Finalizers need careful implementation as any exception thrown in a finalizer will result in a process crash as this code is executed by the finalizer thread. The performance impact arises from the delayed cleanup until the finalizer finalizes the object. Finalizers can clean up unmanaged resources in case the dispose method was not called, but it is preferred to use a SafeHandle to avoid the need for a finalizer. For resource management, it is recommended to use the IDisposable interface and the dispose pattern instead. More information can be found on the Microsoft docs (https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/finalizers).

<!-- Uncomment to add example code -->
<!--### Example code-->
