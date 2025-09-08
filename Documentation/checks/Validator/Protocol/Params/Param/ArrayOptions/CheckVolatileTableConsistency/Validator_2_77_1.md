---
uid: Validator_2_77_1
---

# CheckVolatileTableConsistency

## InvalidVolatileTableUsage

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Volatile tables are meant for temporary data, meaning none of the table data will be stored to database with the intention of sparing some resources.
This makes the volatile feature incompatible with any features that requires persistent (stored to database) data (like save, alarming, foreign keys, etc)..

<!-- Uncomment to add example code -->
<!--### Example code-->
