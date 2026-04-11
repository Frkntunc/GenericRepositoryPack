namespace ApplicationService.Features.Common
{
    public interface ICacheableRequest
    {
        string CacheKey { get; }
        TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(1);
    }

}
