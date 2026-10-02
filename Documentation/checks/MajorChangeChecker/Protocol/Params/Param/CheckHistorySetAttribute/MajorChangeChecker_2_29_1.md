---
uid: MajorChangeChecker_2_29_1
---

# CheckHistorySetAttribute

## EnabledHistorySet

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

<!-- Uncomment to add extra details -->
### Details

Even though nothing actually breaks, adding ´historySet´ is a change of behavior of the connector so there is some user impact.

Additionally, adding the ´historySet´ feature may cause some new trending data to overlap with the trending period of a previous version, and those new ´historySet´ calls may insert data into a timeslot that is already closed for average trending calculation, resulting in misleading average trending for that period.

Other than that, nothing will actually break and the impact will be temporary (only for the period of data overlap).

<!-- Uncomment to add example code -->
<!--### Example code-->
