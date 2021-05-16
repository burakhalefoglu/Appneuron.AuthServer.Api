﻿using Core.Entities.ClaimModels;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Security.Encyption;
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
        private readonly TokenOptions _tokenOptions;
        private readonly CustomerOptions _customerOptions;
        private readonly UnityClientOptions _unityClientOptions;
        private DateTime _accessTokenExpiration;

        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            _customerOptions = Configuration.GetSection("CustomerOptions").Get<CustomerOptions>();
            _unityClientOptions = Configuration.GetSection("UnityClientOptions").Get<UnityClientOptions>();
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
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
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


        public TAccessToken CreateAdminToken<TAccessToken>(UserClaimModel userClaimModel)
            where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateAdminJwtSecurityToken(_tokenOptions, userClaimModel, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new TAccessToken()
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }




        public TAccessToken CreateCustomerToken<TAccessToken>(UserClaimModel userClaimModel)
            where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateCustomerJwtSecurityToken(_tokenOptions, userClaimModel, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new TAccessToken()
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };
        }

        public JwtSecurityToken CreateAdminJwtSecurityToken(TokenOptions tokenOptions, UserClaimModel userClaimModel,
               SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(

                    issuer: tokenOptions.Issuer,
                    audience: tokenOptions.Audience,
                    expires: _accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: SetUserClaims(userClaimModel),
                    signingCredentials: signingCredentials
            );
            return jwt;
        }



        public JwtSecurityToken CreateCustomerJwtSecurityToken(TokenOptions tokenOptions, UserClaimModel userClaimModel,
                SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(

                    issuer: tokenOptions.Issuer,
                    audience: _customerOptions.Audience,
                    expires: _accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: SetUserClaims(userClaimModel),
                    signingCredentials: signingCredentials
            );
            return jwt;
        }

        private IEnumerable<Claim> SetUserClaims(UserClaimModel userClaimModel)
        {
            var claims = new List<Claim>();
            claims.AddNameIdentifier(userClaimModel.UserId.ToString());
            claims.AddRoles(userClaimModel.OperationClaims);

            return claims;
        }


        private JwtSecurityToken CreateClientJwtSecurityToken(TokenOptions tokenOptions, ClientClaimModel clientClaimModel,
               SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(

                    issuer: tokenOptions.Issuer,
                    audience: _unityClientOptions.Audience,
                    expires: _accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: SetClaimsforClient(clientClaimModel),
                    signingCredentials: signingCredentials
            );
            return jwt;
        }


        private IEnumerable<Claim> SetClaimsforClient(ClientClaimModel clientClaimModel)
        {
            var claims = new List<Claim>();
            claims.AddNameIdentifier(clientClaimModel.ClientId);
            claims.AddRoles(clientClaimModel.OperationClaims);
            claims.AddCustomerId(clientClaimModel.CustomerId);
            claims.AddProjectId(clientClaimModel.ProjectId);

            return claims;
        }


    }
}