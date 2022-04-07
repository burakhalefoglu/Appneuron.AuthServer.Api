using Microsoft.AspNetCore.Http;

namespace Core.Extensions;

public static class SignalrExtensions
{
    public static T GetQueryParameterValue<T>(this IQueryCollection httpQuery, string queryParameterName)
    {
        return httpQuery.TryGetValue(queryParameterName, out var value) && value.Any()
            ? (T) Convert.ChangeType(value.FirstOrDefault(), typeof(T))
            : default;
    }
}