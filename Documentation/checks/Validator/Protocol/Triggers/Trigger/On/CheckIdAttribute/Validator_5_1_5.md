---  
uid: Validator_5_1_5  
---

# CheckIdAttribute

## InvalidValue

### Details

When triggering on:  
  \- parameter  
  \- command  
  \- response  
  \- pair  
  \- group  
  \- timer  
  \- session  
A 'Trigger\/On@id' attribute is expected and should have one of the following values:  
  \- 'each': in this case, the protocol should have at least one item of the type mentioned in the 'Trigger\/On' tag.  
  \- The ID of an item of the type mentioned in the 'Trigger\/On' tag.  
When triggering on:  
  \- protocol  
  \- communication  
No 'Trigger\/On@id' attribute is expected.
