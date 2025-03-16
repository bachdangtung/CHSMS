using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;

namespace CHSMS.API.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public TokenBlacklistMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var securityToken = jwtTokenHandler.ReadToken(token) as JwtSecurityToken;
                var jti = securityToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrEmpty(jti))
                {
                    var cacheKey = $"blacklist:{jti}";
                    var isBlacklisted = await _cache.GetStringAsync(cacheKey);
                    if (!string.IsNullOrEmpty(isBlacklisted))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token has been invalidated.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}

