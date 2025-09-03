---
uid: Validator_11_1_2
---

# CheckResponseLogic

## SmartSerialResponseShouldContainHeaderTrailer

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

If there is a header/trailer parameter defined with a headerTrailerLink on a specific connection that is smart-serial, or when there is no connection specified it will automatically apply to the smart-serial connection, then SLPort will be filtering on this header/trailer pairs regardless on how the response definition looks like.
This means if there is e.g. a response defined for the smart-serial connection that consists of one next param type parameter without header/trailer with the intention to receive ALL data that is entering on the socket then this will not work as expected as SLPort will be filtering on the header/trailer pairs and will only foward that data to SLProtocol. Data that does not match the header/trailer will be dropped in the background by SLPort and will also not be shown in the StreamViewer. When there are headerTrailerLink parameters defined for a smart-serial connection then these parameters should also be used on the response for the smart-serial connection.

<!-- Uncomment to add example code -->
<!--### Example code-->
