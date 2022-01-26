using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Entities.ClaimModels;
using Core.Extensions;
using Core.Utilities.Security.Encyption;
using Core.Utilities.Security.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        private readonly ClientOptions _clientOptions;
        private readonly CustomerOptions _customerOptions;
        private readonly OperationClaimCrypto _operationClaimCrypto;
        private readonly TokenOptions _tokenOptions;

        private DateTime _accessTokenExpiration;

        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            _customerOptions = Configuration.GetSection("CustomerOptions").Get<CustomerOptions>();
            _clientOptions = Configuration.GetSection("ClientOptions").Get<ClientOptions>();
            _operationClaimCrypto = Configuration.GetSection("OperationClaimCrypto").Get<OperationClaimCrypto>();
        }

        public IConfiguration Configuration { get; }

        public TAccessToken CreateClientToken<TAccessToken>(ClientClaimModel clientClaimModel)
            where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_clientOptions.AccessTokenExpiration);
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

        public TAccessToken CreateCustomerToken<TAccessToken>(UserClaimModel userClaimModel, List<string> ProjectIdList)
            where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(Convert.ToDouble(_tokenOptions.AccessTokenExpiration));
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateCustomerJwtSecurityToken(_tokenOptions, userClaimModel, signingCredentials, ProjectIdList);
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
            SigningCredentials signingCredentials, List<string> ProjectIdList)
        {
            var jwt = new JwtSecurityToken(
                tokenOptions.Issuer,
                _customerOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetUserClaims(userClaimModel, ProjectIdList),
                signingCredentials: signingCredentials
            );
            return jwt;
        }

        private IEnumerable<Claim> SetUserClaims(UserClaimModel userClaimModel, List<string> ProjectIdList)
        {
            for (var i = 0; i < userClaimModel.OperationClaims.Length; i++)
                userClaimModel.OperationClaims[i] =
                    SecurityKeyHelper.EncryptString(_operationClaimCrypto.Key,
                        userClaimModel.OperationClaims[i]);

            var claims = new List<Claim>();
            claims.AddNameIdentifier(userClaimModel.UserId);
            claims.AddRoles(userClaimModel.OperationClaims);
            if (ProjectIdList.Count > 0)
                ProjectIdList.ForEach(x => { claims.AddProjectId(x); });
            return claims;
        }

        private JwtSecurityToken CreateClientJwtSecurityToken(TokenOptions tokenOptions,
            ClientClaimModel clientClaimModel,
            SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(
                tokenOptions.Issuer,
                _clientOptions.Audience,
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
            claims.AddNameIdentifier(clientClaimModel.ClientId);
            claims.AddRoles(clientClaimModel.OperationClaims);
            claims.AddProjectId(clientClaimModel.ProjectId);

            return claims;
        }
    }
}