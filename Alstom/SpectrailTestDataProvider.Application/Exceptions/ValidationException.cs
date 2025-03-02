#region

using FluentValidation.Results;

#endregion

namespace SpectrailTestDataProvider.Application.Exceptions;

public class ValidationException() : ApplicationException("One or More Validation Failure Happens")
{
    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
}