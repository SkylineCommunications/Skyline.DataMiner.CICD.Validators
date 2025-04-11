---  
uid: Validator_1_22_9  
---

# CheckPageOrderAttribute

## DuplicateEntries

### Details

Skyline recommends the following structure for driver pages:  
\- General  
\- \-\-\-\-\-\-\-\-\-\-\-  
\- Data Page(s)  
\- \-\-\-\-\-\-\-\-\-\-\-  
\- WebInterface

### Example code

```xml
<Display defaultPage="General" pageOrder="General;----------;Data Page 1;Data Page 2;----------;WebInterface#http://[Polling Ip]/" />
```
