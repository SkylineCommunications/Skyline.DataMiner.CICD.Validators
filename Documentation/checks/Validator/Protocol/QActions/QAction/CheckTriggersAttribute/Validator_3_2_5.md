---
uid: Validator_3_2_5
---

# CheckTriggersAttribute

## NonExistingGroup

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

QActions should always have the QAction@triggers defined. It should contain a 'semi-colon' separated list of parameter IDs.
Exceptions are to be made in following cases:
 - Precompiled QActions: no triggers attribute required.
 - QActions triggered by multi-threaded timers: no triggers attribute required.
 - QAction using the options="group": triggers required but refers to Groups instead of Params.

<!-- Uncomment to add example code -->
<!--### Example code-->
