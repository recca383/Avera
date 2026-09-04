using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;

namespace Avera.Domain.Identity.Users
{
    public sealed record UserErrors : Error
    {
        private UserErrors(string code, string description, ErrorType errorType)
            : base(code, description, errorType) { }

        // Conflict Errors

        public static UserErrors MemberRequestIsDuplicate => new UserErrors(
            "User.MemberRequestIsDuplicate",
            "Member request is duplicate",
            ErrorType.Conflict
        );

        public static UserErrors MemberIsJoiningMultipleTimes => new UserErrors(
            "User.MemberIsJoiningMultipleTimes",
            "Member is joining multiple times",
            ErrorType.Conflict
        );

        // Unauthorized Errors

        public static UserErrors IsSuspended => new UserErrors(
            "User.Suspended",
            "User is suspended",
            ErrorType.Unauthorized
        );
        // Not Found Errors

        public static UserErrors UserNotFound => new UserErrors(
            "User.NotFound",
            "User not found",
            ErrorType.NotFound
        );

        public static UserErrors EmailNotFound => new UserErrors(
            "User.EmailNotFound",
            "Email not found",
            ErrorType.NotFound
        );

        public static UserErrors TenantNotFound => new UserErrors(
            "User.TenantNotFound",
            "Tenant not found",
            ErrorType.NotFound
        );

        // Validation Errors
        public static UserErrors InvalidPassword => new UserErrors(
            "User.InvalidPassword",
            "Invalid password",
            ErrorType.Validation
        );

        public static UserErrors InvalidEmail => new UserErrors(
            "User.InvalidEmail",
            "Invalid email",
            ErrorType.Validation
        );

        public static UserErrors InvalidToken => new UserErrors(
            "User.InvalidToken",
            "Invalid token",
            ErrorType.Validation
        );

        public static UserErrors InvalidInviteCode => new UserErrors(
            "User.InvalidInviteCode",
            "Invalid invite code",
            ErrorType.Validation
        );

        public static UserErrors InvalidCredentials => new UserErrors(
            "User.InvalidCredentials",
            "Invalid credentials",
            ErrorType.Validation
        );

        // Conflict Errors
        public static UserErrors EmailAlreadyExists => new UserErrors(
            "User.EmailAlreadyExists",
            "Email already exists",
            ErrorType.Conflict
        );

        // Failure Errors

        public static UserErrors PasswordResetFailed => new UserErrors(
            "User.PasswordResetFailed",
            "Password reset failed",
            ErrorType.Failure
        );

        public static UserErrors EmailConfirmationFailed => new UserErrors(
            "User.EmailConfirmationFailed",
            "Email confirmation failed",
            ErrorType.Failure
        );

        public static UserErrors InviteCodeSendFailed => new UserErrors(
            "User.InviteCodeSendFailed",
            "Invite code send failed",
            ErrorType.Failure
        );

        public static UserErrors JoinInviteCodeFailed => new UserErrors(
            "User.JoinInviteCodeFailed",
            "Join invite code failed",
            ErrorType.Failure
        );

        public static UserErrors RegisterFailed => new UserErrors(
            "User.RegisterFailed",
            "Register failed",
            ErrorType.Failure
        );

        public static UserErrors UserCreationFailed => new UserErrors(
            "User.UserCreationFailed",
            "User creation failed",
            ErrorType.Failure
        );

        public static UserErrors RoleAssignmentFailed => new UserErrors(
            "User.RoleAssignmentFailed",
            "Role assignment failed",
            ErrorType.Failure
        );
    }
}
