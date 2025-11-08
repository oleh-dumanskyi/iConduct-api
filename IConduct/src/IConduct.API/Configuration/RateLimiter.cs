using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace IConduct.API.Configuration
{
    public static class RateLimiter
    {
        public static IServiceCollection AddGlobalRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("global-limiter-policy", limiterOptions =>
                {
                    limiterOptions.AutoReplenishment = true;
                    limiterOptions.PermitLimit = 100;
                    limiterOptions.Window = TimeSpan.FromHours(24);
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                    options.OnRejected = async (context, token) =>
                    {
                        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                        TimeSpan? retryAfter = null;
                        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var metadata))
                        {
                            retryAfter = (TimeSpan)metadata;
                            context.HttpContext.Response.Headers.RetryAfter =
                                ((int)retryAfter.Value.TotalSeconds).ToString();
                        }

                        var responseBody = new
                        {
                            Title = "Too many requests.",
                            Detail = "Please, try again later.",
                            RetryAfterSeconds = retryAfter?.TotalSeconds
                        };

                        context.HttpContext.Response.ContentType = "application/json";

                        await context.HttpContext.Response.WriteAsync(
                            JsonSerializer.Serialize(responseBody),
                            token
                        );
                    };
                });
            });

            return services;
        }
    }
}
