---
uid: Validator_9_5_2
---

# CheckResponseTag

## EmptyTag

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

The Content tag of pairs can contain any number of Response tags.
Those should have as value an unsigned number and refer to the id of an existing Response.
A given pair can't refer to the same Response more than once (including both Response and ResponseOnBadCommand tags).

Also note that only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).

<!-- Uncomment to add example code -->
<!--### Example code-->
