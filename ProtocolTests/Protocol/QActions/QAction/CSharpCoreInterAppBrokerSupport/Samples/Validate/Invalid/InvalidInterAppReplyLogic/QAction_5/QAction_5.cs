using System;
using System.Collections.Generic;

using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
using Skyline.DataMiner.Core.InterAppCalls.Common.Shared;
using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocol protocol)
    {
        try
        {
            string raw = Convert.ToString(protocol.GetParameter(protocol.GetTriggerParameter()));
            IInterAppCall receivedCall = InterAppCallFactory.CreateFromRaw(raw, new List<Type> { });
            if (receivedCall == null)
            {
                protocol.Log("QA" + protocol.QActionID + "|Run|ERR: Value in Parameter was empty.", LogType.Error, LogLevel.NoLogging);
                return;
            }

            foreach (var message in receivedCall.Messages)
            {
                ReturnAddress returnAddress = message.ReturnAddress;
                Message returnMessage;
                message.TryExecute(protocol, protocol, new Dictionary<Type, Type>(), out returnMessage);
                if (returnMessage != null)
                {
                    returnMessage.Send(
                        protocol.SLNet.RawConnection,
                        returnAddress.AgentId,
                        returnAddress.ElementId,
                        returnAddress.ParameterId, new List<Type> { });
                }
            }
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}
