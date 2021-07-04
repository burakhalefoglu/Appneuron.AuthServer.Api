using Core.Entities.ClaimModels;
using Core.Extensions;
using Core.Utilities.Security.Encyption;
using Core.Utilities.Security.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Core.Utilities.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }
        private readonly OperationClaimCrypto _operationClaimCrypto;
        private readonly TokenOptions _tokenOptions;
        private readonly CustomerOptions _customerOptions;
        private readonly ClientOptions _clientOptions;

        private DateTime _accessTokenExpiration;

        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            _customerOptions = Configuration.GetSection("CustomerOptions").Get<CustomerOptions>();
            _clientOptions = Configuration.GetSection("ClientOptions").Get<ClientOptions>();
            _operationClaimCrypto = Configuration.GetSection("OperationClaimCrypto").Get<OperationClaimCrypto>();
        }

        public string DecodeToken(string input)
        {
            var handler = new JwtSecurityTokenHandler();
            if (input.StartsWith("Bearer "))
                input = input.Substring("Bearer ".Length);
            return handler.ReadJwtToken(input).ToString();
        }

        public TAccessToken CreateClientToken<TAccessToken>(ClientClaimModel clientClaimModel)
          where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_clientOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateClientJwtSecurityToken(_tokenOptions, clientClaimModel, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new TAccessToken()
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }

        public TAccessToken CreateCustomerToken<TAccessToken>(UserClaimModel userClaimModel,List<string> ProjectIdList)
            where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateCustomerJwtSecurityToken(_tokenOptions, userClaimModel, signingCredentials, ProjectIdList);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new TAccessToken()
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }

        public JwtSecurityToken CreateCustomerJwtSecurityToken(TokenOptions tokenOptions, UserClaimModel userClaimModel,
                SigningCredentials signingCredentials, List<string> ProjectIdList)
        {
            var jwt = new JwtSecurityToken(

                    issuer: tokenOptions.Issuer,
                    audience: _customerOptions.Audience,
                    expires: _accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: SetUserClaims(userClaimModel, ProjectIdList),
                    signingCredentials: signingCredentials
            );
            return jwt;
        }

        private IEnumerable<Claim> SetUserClaims(UserClaimModel userClaimModel, List<string> ProjectIdList)
        {
            for (int i = 0; i < userClaimModel.OperationClaims.Length; i++)
            {
                userClaimModel.OperationClaims[i] =
                    SecurityKeyHelper.EncryptString(_operationClaimCrypto.Key,
                    userClaimModel.OperationClaims[i]);
            }

            var claims = new List<Claim>();
            claims.AddNameIdentifier(userClaimModel.UserId.ToString());
            claims.AddRoles(userClaimModel.OperationClaims);
            claims.AddUniqueKey(userClaimModel.UniqueKey);
            ProjectIdList.ForEach(x =>
            {
                claims.AddProjectId(x);

            });
            return claims;
        }

        private JwtSecurityToken CreateClientJwtSecurityToken(TokenOptions tokenOptions, ClientClaimModel clientClaimModel,
               SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(

                    issuer: tokenOptions.Issuer,
                    audience: _clientOptions.Audience,
                    expires: _accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: SetClaimsforClient(clientClaimModel),
                    signingCredentials: signingCredentials
            );
            return jwt;
        }

        private IEnumerable<Claim> SetClaimsforClient(ClientClaimModel clientClaimModel)
        {
            for (int i = 0; i < clientClaimModel.OperationClaims.Length; i++)
            {
                clientClaimModel.OperationClaims[i] =
                    SecurityKeyHelper.EncryptString(_operationClaimCrypto.Key,
                    clientClaimModel.OperationClaims[i]);
            }

            var claims = new List<Claim>();
            claims.AddNameIdentifier(clientClaimModel.ClientId);
            claims.AddRoles(clientClaimModel.OperationClaims);
            claims.AddCustomerId(clientClaimModel.CustomerId);
            claims.AddProjectId(clientClaimModel.ProjectId);

            return claims;
        }
    }
}