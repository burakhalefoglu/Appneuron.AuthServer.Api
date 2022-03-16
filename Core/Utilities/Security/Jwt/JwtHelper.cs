using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Entities.ClaimModels;
using Core.Extensions;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encyption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        private readonly string _operationClaimCrypto;
        private readonly TokenOptions _tokenOptions;
        private readonly string _customerAudience;
        private readonly string _clientAudience;
        private readonly string _adminAudience;
        private DateTime _accessTokenExpiration;

        public JwtHelper()
        {
            var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
            _tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();
            _customerAudience = configuration.GetSection("CustomerAudience").Get<string>();
            _clientAudience = configuration.GetSection("ClientAudience").Get<string>();
            _adminAudience = configuration.GetSection("AdminAudience").Get<string>();
            _operationClaimCrypto = configuration.GetSection("OperationClaimCrypto").Get<string>();
        }
        
        public TAccessToken CreateClientToken<TAccessToken>(ClientClaimModel clientClaimModel)
            where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddDays(365);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateClientJwtSecurityToken(_tokenOptions, clientClaimModel, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new TAccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }

        public TAccessToken CreateCustomerToken<TAccessToken>(UserClaimModel userClaimModel)
            where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(Convert.ToDouble(_tokenOptions.AccessTokenExpiration));
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateCustomerJwtSecurityToken(_tokenOptions, userClaimModel, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new TAccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }

        public string DecodeToken(string input)
        {
            var handler = new JwtSecurityTokenHandler();
            if (input.StartsWith("Bearer "))
                input = input.Substring("Bearer ".Length);
            return handler.ReadJwtToken(input).ToString();
        }

        private JwtSecurityToken CreateCustomerJwtSecurityToken(TokenOptions tokenOptions,
            UserClaimModel userClaimModel,
            SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(
                tokenOptions.Issuer,
                _customerAudience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetUserClaims(userClaimModel),
                signingCredentials: signingCredentials
            );
            return jwt;
        }

        private IEnumerable<Claim> SetUserClaims(UserClaimModel userClaimModel)
        {
            for (var i = 0; i < userClaimModel.OperationClaims.Length; i++)
                userClaimModel.OperationClaims[i] =
                    SecurityKeyHelper.EncryptString(_operationClaimCrypto,
                        userClaimModel.OperationClaims[i]);

            var claims = new List<Claim>();
            claims.AddName(userClaimModel.Name);
            claims.AddEmail(userClaimModel.Email);
            claims.AddNameIdentifier(userClaimModel.UserId.ToString());
            claims.AddRoles(userClaimModel.OperationClaims);
            return claims;
        }

        private JwtSecurityToken CreateClientJwtSecurityToken(TokenOptions tokenOptions,
            ClientClaimModel clientClaimModel,
            SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(
                tokenOptions.Issuer,
                _clientAudience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaimsforClient(clientClaimModel),
                signingCredentials: signingCredentials
            );
            return jwt;
        }

        private IEnumerable<Claim> SetClaimsforClient(ClientClaimModel clientClaimModel)
        {
            for (var i = 0; i < clientClaimModel.OperationClaims.Length; i++)
                clientClaimModel.OperationClaims[i] =
                    SecurityKeyHelper.EncryptString(_operationClaimCrypto,
                        clientClaimModel.OperationClaims[i]);

            var claims = new List<Claim>();
            claims.AddNameIdentifier(clientClaimModel.ClientId.ToString());
            claims.AddRoles(clientClaimModel.OperationClaims);
            claims.AddProjectId(clientClaimModel.ProjectId.ToString());

            return claims;
        }
    }
}