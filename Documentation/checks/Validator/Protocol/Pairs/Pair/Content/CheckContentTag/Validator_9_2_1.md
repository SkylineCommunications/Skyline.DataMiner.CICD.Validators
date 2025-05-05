---
uid: Validator_9_2_1
---

# CheckContentTag

## MissingClearResponseRoutine

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Typically when a pair contains multiple responses, a clear response routine needs to be implemented in order to make sure parameters gets updated correctly.
There are 2 possible ways to implement the clear routine.
However, the first one is recommended as it is more efficient.
    - Recommended way: After response X, clear all other responses present in the same pair.
    - Alternative way: After response X, clear response X.

<!-- Uncomment to add example code -->
<!--### Example code-->
