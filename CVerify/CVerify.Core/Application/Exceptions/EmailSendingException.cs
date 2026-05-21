using System;

namespace CVerify.API.Application.Exceptions;

/// <summary>
/// Exception thrown when email delivery, rendering, or transport validation fails.
/// </summary>
public class EmailSendingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSendingException"/> class.
    /// </summary>
    public EmailSendingException()
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EmailSendingException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public EmailSendingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
