namespace Congratulator.Domain.Errors.Implementations;

public record FileTooLargeError(long MaxSize)
        : DomainError(
            Code: "FILE_TOO_LARGE",
            Description: $"Максимальный размер файла — {MaxSize:N0} байт"
          );

public record InvalidFormatError(string ExpectedFormat)
        : DomainError(
            Code: "INVALID_FORMAT",
            Description: $"Ожидался формат '{ExpectedFormat}'"
          );

public record RecordAlreadyExisted(string RecordType, string Parameter)
        : DomainError(
            Code: "ALREADY_EXISTED",
            Description: $"Запись {RecordType} с параметром {Parameter} уже существует"
          );

public record RecordNotFound(string RecordType, string Parameter)
        : DomainError(
            Code: "NOT_FOUND",
            Description: $"Запись {RecordType} с параметром {Parameter} не найдена"
          );

public record AuthorizationFailed(string Reason)
        : DomainError(
            Code: "AUTH_FAILED",
            Description: $"Не удалось авторизовать пользователя. {Reason}"
          );
public record UnsupportedFileType(string Reason)
        : DomainError(
            Code: "UNSUPPORTED_FILE_TYPE",
            Description: $"{Reason}"
          );

public record EmptyData(string Parameter)
        : DomainError(
            Code: "EMPTY_DATA",
            Description: $"Параметр {Parameter} ожидает данные, которые не были предоставлены"
          );