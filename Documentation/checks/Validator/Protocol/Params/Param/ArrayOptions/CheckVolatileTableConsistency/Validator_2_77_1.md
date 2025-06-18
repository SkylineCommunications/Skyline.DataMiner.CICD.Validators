---
uid: Validator_2_77_1
---

# CheckVolatileTableConsistency

## InvalidVolatileTableUsage

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Volatile tables are meant for temporary data. If a table uses features that persist data (like save, alarm, or foreign key relations), the volatile option should not be used, as this can lead to inconsistencies or performance issues.

<!-- Uncomment to add example code -->
<!--### Example code-->
