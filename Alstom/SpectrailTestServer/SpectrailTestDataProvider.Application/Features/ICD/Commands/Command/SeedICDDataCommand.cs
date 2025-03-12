#region

using MediatR;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD.Commands.Command;

/// <summary>
///     ✅ Command to seed ICD Data from Excel
/// </summary>
public class SeedICDDataCommand : IRequest<bool>;