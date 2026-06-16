using System;
using System.Threading;
using System.Threading.Tasks;
using CVerify.API.Modules.Jd.DTOs;

namespace CVerify.API.Modules.Jd.Services;

public interface IJdService
{
    Task<JdCreateResponse> CreateJdAsync(Guid userId, JdFormRequest request, CancellationToken cancellationToken = default);
}
