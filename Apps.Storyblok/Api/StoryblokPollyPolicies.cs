using System.Globalization;
using System.Net;
using Polly;
using Polly.Retry;
using RestSharp;

namespace Apps.Storyblok.Api;

public static class StoryblokPollyPolicies
{
    private const int DefaultRetryCount = 5;
    private const double MinimumFallbackDelaySeconds = 5;
    private const double MaximumFallbackDelaySeconds = 15;
    private const double MaximumJitterMilliseconds = 500;

    public static ResiliencePipeline<RestResponse> CreateRateLimitPipeline(
        int retryCount = DefaultRetryCount)
    {
        var options = new RetryStrategyOptions<RestResponse>
        {
            MaxRetryAttempts = retryCount,
            ShouldHandle = new PredicateBuilder<RestResponse>()
                .HandleResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
                .Handle<HttpRequestException>(exception =>
                    exception.StatusCode == HttpStatusCode.TooManyRequests),
            DelayGenerator = args => new ValueTask<TimeSpan?>(GetDelay(args.Outcome.Result))
        };

        return new ResiliencePipelineBuilder<RestResponse>()
            .AddRetry(options)
            .Build();
    }

    private static TimeSpan GetDelay(RestResponse? response)
    {
        if (response is not null && TryGetRetryAfter(response, out var retryAfter))
        {
            return retryAfter + GetJitter();
        }

        var delaySeconds = Random.Shared.NextDouble() *
            (MaximumFallbackDelaySeconds - MinimumFallbackDelaySeconds) +
            MinimumFallbackDelaySeconds;

        return TimeSpan.FromSeconds(delaySeconds);
    }

    private static bool TryGetRetryAfter(RestResponse response, out TimeSpan delay)
    {
        var value = response.Headers?
            .FirstOrDefault(header =>
                header.Name?.Equals("Retry-After", StringComparison.OrdinalIgnoreCase) == true)
            ?.Value?.ToString();

        if (string.IsNullOrWhiteSpace(value))
        {
            delay = default;
            return false;
        }

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) &&
            double.IsFinite(seconds) && seconds >= 0)
        {
            delay = TimeSpan.FromSeconds(seconds);
            return true;
        }

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var retryAt))
        {
            delay = retryAt - DateTimeOffset.UtcNow;
            if (delay < TimeSpan.Zero)
            {
                delay = TimeSpan.Zero;
            }

            return true;
        }

        delay = default;
        return false;
    }

    private static TimeSpan GetJitter() =>
        TimeSpan.FromMilliseconds(Random.Shared.NextDouble() * MaximumJitterMilliseconds);
}
