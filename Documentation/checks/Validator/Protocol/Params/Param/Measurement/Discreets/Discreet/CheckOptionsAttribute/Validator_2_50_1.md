---
uid: Validator_2_50_1
---

# CheckOptionsAttribute

## MisconfiguredConfirmOptions

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

A context menu action executing a critical action should have a confirmation message.
This can be done by adding the confirm option via the 'Discreet@options' attribute.

### Example code

<Discreet options="confirm:The selected item(s) will be deleted permanently.">
    <Display>Delete selected row(s)</Display>
    <Value>delete</Value>
</Discreet>
