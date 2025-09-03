---
uid: Validator_18_8_5
---

# CheckParameterAttribute

## NonExistingId

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

The 'Tab@parameter' attribute has different meaning depending on the 'Tab@type' attribute value.
    - parameters: a comma-separated list of PIDs is expeted. Those should refer to columns of the main table for this TreeControl level or params added to its main section via ExtraDetail tags.
    - relation: The PID of a column containing a foreignKey to the main table for this TreeControl level.
    - summary: The PID of a table is a 'grand-chidren' of the main table for this TreeControl level.
    - default: No 'Tab@parameter' attribute expected in this case.
    - chart: No 'Tab@parameter' attribute expected in this case.
    - web: No 'Tab@parameter' attribute expected in this case.

Note that in any case, parameters referenced are expected to have the RTDisplay tag set to true.

<!-- Uncomment to add example code -->
<!--### Example code-->
