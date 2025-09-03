---
uid: Validator_2_21_22
---

# CheckOptionsAttribute

## HeaderTrailerLinkShouldHaveConnection

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

When a connection is not specified, then ALL connections will listen for this header/trailer. Even though a connection could have a response without a trailer, it will still be listening for that trailer to appear.
When SLPort is listening for a trailer, it will reject the data when the trailer is not present.
Rejected data by SLPort will not appear in the StreamViewer and is therefore harder to investigate.
It is strongly recommended to always use headerTrailerLink option in combination with a specified intended connection, even when there would be only one connection present as it could quickly result into issues when another connection is added in the future.

### Example code

<Type options="headerTrailerLink=1;connection=0">
