---
uid: Validator_2_80_1
---

# CheckInconsistentSnmpReadWriteTypes

## InconsistentSnmpReadWriteTypes

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

<!-- Uncomment to add extra details -->
### Details

When an SNMP parameter has both a read parameter and a write parameter targeting the same OID (`/Protocol/Params/Param/SNMP/OID`), the `/Protocol/Params/Param/SNMP/Type` of both parameters should match. If the read parameter and the write parameter use a different SNMP type, DataMiner may encode or decode the value inconsistently between polling (read) and setting (write) the parameter.

If the type for read and write is different but intentional, please provide a Suppression reasoning.

<!-- Uncomment to add example code -->
### Example code

Incorrect:

```xml
<Param id="1">
   <Name>snmpParam</Name>
   <Type>read</Type>
   <SNMP>
      <Enabled>true</Enabled>
      <OID type="complete">1.3.6.1.2.1.1.6.0</OID>
      <Type>integer</Type>
   </SNMP>
</Param>
<Param id="51">
   <Name>snmpParam</Name>
   <Type>write</Type>
   <SNMP>
      <Enabled>true</Enabled>
      <OID type="complete">1.3.6.1.2.1.1.6.0</OID>
      <Type>octetstring</Type>
   </SNMP>
</Param>
```

Correct:

```xml
<Param id="1">
   <Name>snmpParam</Name>
   <Type>read</Type>
   <SNMP>
      <Enabled>true</Enabled>
      <OID type="complete">1.3.6.1.2.1.1.6.0</OID>
      <Type>octetstring</Type>
   </SNMP>
</Param>
<Param id="51">
   <Name>snmpParam</Name>
   <Type>write</Type>
   <SNMP>
      <Enabled>true</Enabled>
      <OID type="complete">1.3.6.1.2.1.1.6.0</OID>
      <Type>octetstring</Type>
   </SNMP>
</Param>
```

