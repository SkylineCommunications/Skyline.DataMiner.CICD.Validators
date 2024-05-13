using System;
using System.Collections.Generic;

using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
using Skyline.DataMiner.Core.InterAppCalls.Common.Serializing;
using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class: ProcessInterAppReceived.
/// </summary>
public class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocolExt protocol)
	{
		try
		{
			string raw = Convert.ToString(protocol.GetParameter(protocol.GetTriggerParameter()));
			IInterAppCall receivedCall = InterAppCallFactory.CreateFromRaw(raw, new List<Type>());
			if (receivedCall == null)
			{
				protocol.Log("QA" + protocol.QActionID + "|Run|ERR: Value in Parameter was empty.", LogType.Error, LogLevel.NoLogging);
				return;
			}

			foreach (var message in receivedCall.Messages)
			{
				Message returnMessage;
				message.TryExecute(protocol, protocol, new Dictionary<Type,Type>(), out returnMessage);

				if (returnMessage != null)
				{
					var serializer = SerializerFactory.CreateInterAppSerializer(new List<Type>());
					var returnData = serializer.SerializeToString(returnMessage);
					protocol.SetParameter(message.ReturnAddress.ParameterId, returnData);
				}
			}
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}