---
uid: Validator_2_21_20
---

# CheckOptionsAttribute

## UnrecommendedSshOptions

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Using SSH Username, SSH Password, or SSH Options will restrict the protocol to a single SSH connection over port 22. Attempting to use these with additional SSH connections or a different port may result in unpredictable behavior.

It is recommended to reference the parameter in the PortSettings instead. This allows you to define multiple SSH connections and configure any port number.

### Example code

 <PortSettings name="SSH Connection">
  <IPport>
   <DefaultValue>22</DefaultValue>
  </IPport>
  <BusAddress>
   <Disabled>true</Disabled>
  </BusAddress>
  <PortTypeSerial>
   <Disabled>true</Disabled>
  </PortTypeSerial>
  <PortTypeUDP>
   <Disabled>true</Disabled>
  </PortTypeUDP>
  <SSH>
   <Credentials>
    <Username pid="1" />
    <Password pid="2" />
   </Credentials>
  </SSH>
 </PortSettings>
