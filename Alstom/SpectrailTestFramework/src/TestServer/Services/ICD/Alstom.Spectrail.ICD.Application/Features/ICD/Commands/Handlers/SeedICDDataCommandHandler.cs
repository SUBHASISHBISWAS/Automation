#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;

/// <summary>
///     ✅ Command Handler for Seeding ICD Data
/// </summary>
public class SeedICDDataCommandHandler(IExcelService icdExcelService)
    : IRequestHandler<SeedICDDataCommand, bool>
{
    public async Task<bool> Handle(SeedICDDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await icdExcelService.InitializeAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error seeding ICD Data: {ex.Message}");
            return false;
        }
    }
}