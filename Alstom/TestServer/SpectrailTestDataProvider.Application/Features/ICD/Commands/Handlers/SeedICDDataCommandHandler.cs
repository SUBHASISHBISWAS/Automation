#region

using MediatR;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Application.Features.ICD.Commands.Command;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD.Commands.Handlers;

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