using System.IdentityModel.Tokens.Jwt;

namespace api.Options
{
    public class Decoder
    {
        public static int getUserId(string encodedToken)
        {
            encodedToken = encodedToken.Replace("Bearer ", string.Empty);
            var decodedToken = new JwtSecurityTokenHandler().ReadToken(encodedToken) as JwtSecurityToken;
            return int.Parse(decodedToken.Claims.First(c => c.Type == "user_id").Value);
        }
    }
}