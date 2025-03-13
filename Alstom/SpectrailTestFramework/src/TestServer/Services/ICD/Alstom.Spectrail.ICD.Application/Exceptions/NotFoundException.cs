namespace Alstom.Spectrail.ICD.Application.Exceptions;

public class NotFoundException(string name, object key)
    : ApplicationException($"Entity \"{name}\" ({key}) was not found.");