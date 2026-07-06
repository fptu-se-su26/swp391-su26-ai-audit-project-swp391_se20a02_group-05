using CVerify.API.Modules.Jd.DTOs;

namespace CVerify.API.Modules.Jd.Services;

public interface IJdMatchingService
{
    MatchScoreResponse CalculateMatch(JdMatchRequest request);
}
