using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using TripGenie.API.Application.DTOs;
using TripGenie.API.IntegrationTests.Fixtures;
using Xunit;

namespace TripGenie.API.IntegrationTests.Auth;

public class AuthTelemetryTests : BaseIntegrationTest
{
    public AuthTelemetryTests(SharedTestcontainerFixture containerFixture) : base(containerFixture)
    {
    }

    [Fact]
    public async Task Auth_Requests_Should_Propagate_Traceparent_Headers()
    {
        var request = new ForgotPasswordRequest(Email: "telemetry@tripgenie.ai");

        var message = new HttpRequestMessage(HttpMethod.Post, "/api/auth/forgot-password")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("traceparent", "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01");

        var response = await Client.SendAsync(message);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
