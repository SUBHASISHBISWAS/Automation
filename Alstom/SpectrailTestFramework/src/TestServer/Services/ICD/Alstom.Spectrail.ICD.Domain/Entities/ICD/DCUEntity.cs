#region

using Alstom.Spectrail.ICD.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Domain.Entities.ICD;

public class DCUEntity : EntityBase
{
    public string? VariableAddress { get; set; }
    public string? SignalOffset { get; set; }
    public string? SignalName { get; set; }
    public string? ControlBuildType { get; set; }
    public string? ComplexNetworkType { get; set; }
    public string? SILLevel { get; set; }
    public string? SystemContent { get; set; }
    public string? SignalDescription { get; set; }
    public string? SenderFunction { get; set; }
    public string? SenderEquipment { get; set; }
    public string? ReceiverEquipment { get; set; }
    public string? ReceiverFunction { get; set; }
    public string? DefaultValue { get; set; }
    public string? MultiplierValue { get; set; }
    public string? ShiftValue { get; set; }
    public string? TaskName { get; set; }
    public string? Alignment { get; set; }
    public string? Size { get; set; }
    public string? RemoteSignalOffset { get; set; }
    public string? PrimaryReadbackDataSignal { get; set; }
    public string? ApplicableToConfiguration { get; set; }
}