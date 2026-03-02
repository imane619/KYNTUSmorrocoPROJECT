namespace ShiftMaster.Absence.API.Application.Services;

/// <summary>
/// Forwards the current request's Authorization header to outgoing Planning API calls.
/// </summary>
public class ForwardAuthHandler(IHttpContextAccessor httpContext) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var auth = httpContext.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(auth))
            request.Headers.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(auth);
        return await base.SendAsync(request, ct);
    }
}
