using Business.Handlers.Authorizations.Commands;
using Business.Handlers.Authorizations.Queries;
using Business.Handlers.Clients.Commands;
using Business.Handlers.GroupClaims.Commands;
using Business.Handlers.GroupClaims.Queries;
using Business.Handlers.Groups.Commands;
using Business.Handlers.Groups.Queries;
using Business.Handlers.Languages.Commands;
using Business.Handlers.Languages.Queries;
using Business.Handlers.Logs.Queries;
using Business.Handlers.OperationClaims.Commands;
using Business.Handlers.OperationClaims.Queries;
using Business.Handlers.Translates.Commands;
using Business.Handlers.Translates.Queries;
using Business.Handlers.UserClaims.Commands;
using Business.Handlers.UserClaims.Queries;
using Business.Handlers.UserGroups.Commands;
using Business.Handlers.UserGroups.Queries;
using Business.Handlers.UserProjects.Commands;
using Business.Handlers.UserProjects.Queries;
using Business.Handlers.Users.Commands;
using Business.Handlers.Users.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Extensions;

public static class MediatrExtension
{
    public static void AddMediatRApi(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ForgotPasswordCommand));
        services.AddMediatR(typeof(RegisterUserCommand));
        services.AddMediatR(typeof(ResetPasswordCommand));
        services.AddMediatR(typeof(LoginUserQuery));
        
        services.AddMediatR(typeof(CreateTokenCommand));
        
        services.AddMediatR(typeof(CreateGroupClaimCommand));
        services.AddMediatR(typeof(DeleteGroupClaimCommand));
        services.AddMediatR(typeof(UpdateGroupClaimCommand));
        services.AddMediatR(typeof(GetGroupClaimQuery));
        services.AddMediatR(typeof(GetGroupClaimsQuery));
        
        services.AddMediatR(typeof(CreateGroupCommand));
        services.AddMediatR(typeof(DeleteGroupCommand));
        services.AddMediatR(typeof(UpdateGroupCommand));
        services.AddMediatR(typeof(GetGroupQuery));
        services.AddMediatR(typeof(GetGroupsQuery));
        
        services.AddMediatR(typeof(CreateLanguageCommand));
        services.AddMediatR(typeof(DeleteLanguageCommand));
        services.AddMediatR(typeof(UpdateLanguageCommand));
        services.AddMediatR(typeof(GetLanguageQuery));
        services.AddMediatR(typeof(GetLanguagesQuery));
        
        services.AddMediatR(typeof(GetLogDtoQuery));
        
        services.AddMediatR(typeof(CreateOperationClaimCommand));
        services.AddMediatR(typeof(DeleteOperationClaimCommand));
        services.AddMediatR(typeof(UpdateOperationClaimCommand));
        services.AddMediatR(typeof(GetOperationClaimQuery));
        services.AddMediatR(typeof(GetOperationClaimsQuery));
                
        services.AddMediatR(typeof(CreateTranslateCommand));
        services.AddMediatR(typeof(DeleteTranslateCommand));
        services.AddMediatR(typeof(UpdateTranslateCommand));
        services.AddMediatR(typeof(GetTranslateQuery));
        services.AddMediatR(typeof(GetTranslatesQuery));

        services.AddMediatR(typeof(CreateUserClaimCommand));
        services.AddMediatR(typeof(DeleteUserClaimCommand));
        services.AddMediatR(typeof(UpdateUserClaimCommand));
        services.AddMediatR(typeof(GetUserClaimsQuery));
        
        services.AddMediatR(typeof(CreateUserGroupCommand));
        services.AddMediatR(typeof(DeleteUserGroupCommand));
        services.AddMediatR(typeof(UpdateUserGroupCommand));
        services.AddMediatR(typeof(GetUserGroupQuery));
        services.AddMediatR(typeof(GetUserGroupsQuery));

        services.AddMediatR(typeof(CreateUserProjectCommand));
        services.AddMediatR(typeof(DeleteUserProjectCommand));
        services.AddMediatR(typeof(UpdateUserProjectCommand));
        services.AddMediatR(typeof(GetUserProjectQuery));
        services.AddMediatR(typeof(GetUserProjectsQuery));
        
        services.AddMediatR(typeof(CreateUserCommand));
        services.AddMediatR(typeof(DeleteUserCommand));
        services.AddMediatR(typeof(UpdateUserCommand));
        services.AddMediatR(typeof(UserChangePasswordCommand));
        services.AddMediatR(typeof(GetUserQuery));
        services.AddMediatR(typeof(GetUsersQuery));
        
        
    }
}