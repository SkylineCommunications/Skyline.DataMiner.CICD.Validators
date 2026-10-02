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

Additionally, Adding ´historySet´ feature may cause some new trending data to overlap with trending period of previous version and those new ´historySet´ calls may be inserting data into a timeslot that is already closed for average trending calculation resulting in misleading average trending for that period.

Other than that, nothing will actually break and the impact will be temporary (only for the period of data overlap).

<!-- Uncomment to add example code -->
<!--### Example code-->
