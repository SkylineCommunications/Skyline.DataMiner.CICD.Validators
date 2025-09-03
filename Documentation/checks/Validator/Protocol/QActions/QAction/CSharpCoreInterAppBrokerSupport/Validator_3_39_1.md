---
uid: Validator_3_39_1
---

# CSharpCoreInterAppBrokerSupport

## InvalidInterAppReplyLogic

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

To maintain proper message handling, always use the .Reply() method on the original incoming message object received from an external source.

Do not use .Reply() on messages you create anew. 
Also, refrain from using .SetParameter(9000001, data); or .Send to the ReturnAddress, as these practices are incorrect.

Using the .Reply() method is crucial because it incorporates specific logic that optimizes communication between applications. It dynamically chooses the best delivery method, utilizing either SLNet Subscriptions or a more efficient message broker when available, ensuring that responses are handled efficiently and effectively based on the current configuration of the running agent. 

This optimization helps maintain system integrity and improves performance.

### Example code

foreach (var receivedMessage in receivedCall.Messages)
{
    Message response;
    receivedMessage.TryExecute(protocol, protocol, Shared.Mapping, out response);
    
    if (response != null)
    {
         receivedMessage.Reply(protocol.SLNet.RawConnection, response, Shared.KnownTypes);
    }
}
