

using Application.Common.Exceptions;

namespace Application.Orders.Exceptions;

public sealed class OrderNotFoundException(string Message) : AppException(Message) { }
