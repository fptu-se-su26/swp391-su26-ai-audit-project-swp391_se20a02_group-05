using System;
using System.Threading;
using System.Threading.Tasks;
using CVerify.API.Modules.Jd.DTOs;

namespace CVerify.API.Modules.Jd.Services;

public interface IJdService
{
    Task<JdCreateResponse> CreateJdAsync(Guid userId, JdFormRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JdSummaryResponse>> ListJdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<JdDetailResponse?> GetJdAsync(Guid userId, string jdId, CancellationToken cancellationToken = default);
    Task<JdDetailResponse?> UpdateJdAsync(Guid userId, string jdId, JdUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteJdAsync(Guid userId, string jdId, CancellationToken cancellationToken = default);
}
