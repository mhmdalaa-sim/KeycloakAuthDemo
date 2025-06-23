namespace KeycloakAuthDemo
{
    using Microsoft.AspNetCore.Authentication;
    using System.Security.Claims;
    using System.Text.Json;

    public class KeycloakRealmRoleClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity;

            var realmAccessClaim = identity.FindFirst("realm_access");
            Console.WriteLine("🔍 TransformAsync called");

            if (realmAccessClaim != null)
            {
                Console.WriteLine("✅ Found realm_access: " + realmAccessClaim.Value);

                using var doc = JsonDocument.Parse(realmAccessClaim.Value);
                if (doc.RootElement.TryGetProperty("roles", out var roles))
                {
                    foreach (var role in roles.EnumerateArray())
                    {
                        var roleValue = role.GetString();
                        Console.WriteLine("➡️ Adding role claim: " + roleValue);
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                    }
                }
            }

            return Task.FromResult(principal);
        }

    }
}