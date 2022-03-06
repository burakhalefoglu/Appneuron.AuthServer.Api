using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Entities.ClaimModels;
using Core.Extensions;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encyption;
using Core.Utilities.Security.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        private readonly OperationClaimCrypto _operationClaimCrypto;
        private readonly TokenOptions _tokenOptions;

        private DateTime _accessTokenExpiration;

        public JwtHelper()
        {
            var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
         _tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();
            _operationClaimCrypto = configuration.GetSection("OperationClaimCrypto").Get<OperationClaimCrypto>();
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

        public TAccessToken CreateCustomerToken<TAccessToken>(UserClaimModel userClaimModel, List<long> projectIdList)
            where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddHours(Convert.ToDouble(_tokenOptions.AccessTokenExpiration));
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateCustomerJwtSecurityToken(_tokenOptions, userClaimModel, signingCredentials, projectIdList);
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
            SigningCredentials signingCredentials, List<long> projectIdList)
        {
            var jwt = new JwtSecurityToken(
                tokenOptions.Issuer,
                "Appneuron",
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetUserClaims(userClaimModel, projectIdList),
                signingCredentials: signingCredentials
            );
            return jwt;
        }

        private IEnumerable<Claim> SetUserClaims(UserClaimModel userClaimModel, List<long> projectIdList)
        {
            for (var i = 0; i < userClaimModel.OperationClaims.Length; i++)
                userClaimModel.OperationClaims[i] =
                    SecurityKeyHelper.EncryptString(_operationClaimCrypto.Key,
                        userClaimModel.OperationClaims[i]);

            var claims = new List<Claim>();
            claims.AddNameIdentifier(userClaimModel.UserId.ToString());
            claims.AddRoles(userClaimModel.OperationClaims);
            if (projectIdList.Count > 0)
                projectIdList.ForEach(x => { claims.AddProjectId(x.ToString()); });
            return claims;
        }

        private JwtSecurityToken CreateClientJwtSecurityToken(TokenOptions tokenOptions,
            ClientClaimModel clientClaimModel,
            SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(
                tokenOptions.Issuer,
                "Appneuron",
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
                    SecurityKeyHelper.EncryptString(_operationClaimCrypto.Key,
                        clientClaimModel.OperationClaims[i]);

            var claims = new List<Claim>();
            claims.AddNameIdentifier(clientClaimModel.ClientId.ToString());
            claims.AddRoles(clientClaimModel.OperationClaims);
            claims.AddProjectId(clientClaimModel.ProjectId.ToString());

            return claims;
        }
    }
}