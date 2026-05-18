using System.Threading;
using System.Threading.Tasks;

namespace TripGenie.API.Application.Interfaces;

/// <summary>
/// Defines the high-level business contract for email delivery requests.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Delivers a responsive verification card requesting new users to confirm registration.
    /// </summary>
    /// <param name="toEmail">The target email address.</param>
    /// <param name="fullName">The recipient's full name.</param>
    /// <param name="verificationLink">The fully qualified verification URL.</param>
    /// <param name="cancellationToken">Cancellation token trace.</param>
    Task SendVerificationEmailAsync(
        string toEmail,
        string fullName,
        string verificationLink,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delivers a security warning reset link containing quick password reset pathways.
    /// </summary>
    /// <param name="toEmail">The target email address.</param>
    /// <param name="fullName">The recipient's full name.</param>
    /// <param name="resetLink">The fully qualified password reset URL.</param>
    /// <param name="cancellationToken">Cancellation token trace.</param>
    Task SendResetPasswordEmailAsync(
        string toEmail,
        string fullName,
        string resetLink,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delivers a welcome notice onboarding users onto the travel generation dashboard.
    /// </summary>
    /// <param name="toEmail">The target email address.</param>
    /// <param name="fullName">The recipient's full name.</param>
    /// <param name="cancellationToken">Cancellation token trace.</param>
    Task SendWelcomeEmailAsync(
        string toEmail,
        string fullName,
        CancellationToken cancellationToken = default);
}
