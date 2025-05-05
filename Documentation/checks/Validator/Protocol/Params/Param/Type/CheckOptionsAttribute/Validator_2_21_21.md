---
uid: Validator_2_21_21
---

# CheckOptionsAttribute

## InvalidMixOfSshOptionsAndPortSettings

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Conflicting SSH configurations detected.
You're using both SSH Username, SSH Password, SSH Options and 'PortSettings/SSH'.
SSH Username, SSH Password, SSH Options are restricted to one SSH connection on port 22 only and shouldn't be mixed with 'PortSettings/SSH'.
Use only one of these configurations.
'PortSettings/SSH' is generally better as it supports multiple SSH connections and use any port number.


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
            <Username pid="1100"/>
            <Password pid="1101"/>
        </Credentials>
    </SSH>
</PortSettings>
