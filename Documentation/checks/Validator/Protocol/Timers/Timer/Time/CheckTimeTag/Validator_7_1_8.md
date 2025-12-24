---
uid: Validator_7_1_8
---

# CheckTimeTag

## TooSimilarTimers

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

<!-- Uncomment to add extra details -->
### Details

Each timer is a thread which uses resources. Timers are there to be able to have different polling rates. It should not be used to split groups on other factors than the polling rate. Therefore, we expect each timer to have Timer/Time different value (differences between them should be at very least more than 1 second)

<!-- Uncomment to add example code -->
<!--### Example code-->
