---  
uid: Validator_8_16_4  
---

# CheckIdAttribute

## InvalidValue

### Details

The id attribute is used internally as the identifier for each session.  
It is therefore mandatory and needs to follow a number of rules:  
\- Each session should have a unique id.  
\- Should be an unsigned integer.  
\- Only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).
