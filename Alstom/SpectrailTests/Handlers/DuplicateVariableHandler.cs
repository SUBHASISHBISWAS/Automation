using SpectrailTestFramework.Actions;
using SpectrailTestFramework.Attributes;
using SpectrailTestFramework.Interfaces;

using SpectrailTests.Pages;

namespace SpectrailTests.Handlers;

[MapsToPage(typeof(DuplicateVariableCheck))]
public class DuplicateVariableHandler(IPageObject pageObject, IHandlerFactory handlerFactory) : BaseActionHandler
{
    private readonly DuplicateVariableCheck _duplicateVariablePage = pageObject as DuplicateVariableCheck ??
                                                                     throw new ArgumentException(
                                                                         "Invalid PageObject type.");

    private readonly IHandlerFactory _handlerFactory = handlerFactory;

    protected override async Task ExecuteAsync()
    {
        await _duplicateVariablePage.CheckDuplicate();
    }
}