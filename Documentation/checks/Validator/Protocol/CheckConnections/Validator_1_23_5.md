---
uid: Validator_1_23_5
---

# CheckConnections

## InvalidConnectionCount

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

For each port that is defined, a PortSettings element should be defined. In addition, the order of these PortSettings elements must correspond with the order of the con­nections defined in the Protocol/Type@advanced attribute.
- Connection count = number of connections defined in 'Protocol/Type@advanced' + 1 for the main connection define in 'Protocol/Type' tag.
- PortSettings count = number of PortSettings in 'Protocol/Ports' tag + 1 for main PortSettings define in 'Protocol/PortSettings'.

<!-- Uncomment to add example code -->
<!--### Example code-->
