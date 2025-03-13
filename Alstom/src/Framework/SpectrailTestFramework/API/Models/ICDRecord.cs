#region

using Newtonsoft.Json;
using SpectrailTestFramework.API.Models;

#endregion

namespace SpectrailTestDataProvider.Domain.Entities.ICD;

public class ICDRecord : BaseEntity
{
    [JsonProperty("variableAddress")] public string? VariableAddress { get; set; }
    [JsonProperty("signalOffset")] public string? SignalOffset { get; set; }
    [JsonProperty("signalName")] public string? SignalName { get; set; }
    [JsonProperty("controlBuildType")] public string? ControlBuildType { get; set; }
    [JsonProperty("complexNetworkType")] public string? ComplexNetworkType { get; set; }
    [JsonProperty("silLevel")] public string? SILLevel { get; set; }
    [JsonProperty("systemContent")] public string? SystemContent { get; set; }
    [JsonProperty("signalDescription")] public string? SignalDescription { get; set; }
    [JsonProperty("senderFunction")] public string? SenderFunction { get; set; }
    [JsonProperty("senderEquipment")] public string? SenderEquipment { get; set; }
    [JsonProperty("receiverEquipment")] public string? ReceiverEquipment { get; set; }
    [JsonProperty("receiverFunction")] public string? ReceiverFunction { get; set; }
    [JsonProperty("defaultValue")] public string? DefaultValue { get; set; }
    [JsonProperty("multiplierValue")] public string? MultiplierValue { get; set; }
    [JsonProperty("shiftValue")] public string? ShiftValue { get; set; }
    [JsonProperty("taskName")] public string? TaskName { get; set; }
    [JsonProperty("alignment")] public string? Alignment { get; set; }
    [JsonProperty("size")] public string? Size { get; set; }
    [JsonProperty("remoteSignalOffset")] public string? RemoteSignalOffset { get; set; }

    [JsonProperty("primaryReadbackDataSignal")]
    public string? PrimaryReadbackDataSignal { get; set; }

    [JsonProperty("applicableToConfiguration")]
    public string? ApplicableToConfiguration { get; set; }
}