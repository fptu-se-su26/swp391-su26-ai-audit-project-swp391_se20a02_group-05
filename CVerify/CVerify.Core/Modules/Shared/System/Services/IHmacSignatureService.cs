using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.Modules.Shared.System.Services;

public interface IHmacSignatureService
{
    string ComputeSignature(string method, string url, string body, string timestamp, string nonce);
    (string Signature, string Timestamp, string Nonce) CreateSignatureHeaders(string method, string url, string body);

    /// <summary>
    /// Verifies an inbound HMAC signature (e.g. service-to-service calls from CVerify.AI).
    /// Recomputes the expected signature and compares it in constant time, and rejects
    /// requests whose timestamp is outside the allowed clock-skew window (replay guard).
    /// </summary>
    bool VerifySignature(
        string method,
        string url,
        string body,
        string timestamp,
        string nonce,
        string signature,
        int maxClockSkewSeconds = 300);
}
