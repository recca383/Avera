using SharedKernel;

namespace Avera.Domain.Identity.Tenants
{
    public record TenantErrors : Error
    {
        private TenantErrors(string code, string description, ErrorType errorType)
            : base(code, description, errorType) { }

        public static TenantErrors TenantNotFound => new TenantErrors(
            "Tenant.NotFound",
            "No tenant found.",
            ErrorType.NotFound
        );

        public static TenantErrors AlreadyBelongsToTenant => new TenantErrors(
            "Tenant.AlreadyBelongsToTenant",
            "User already belongs to a tenant",
            ErrorType.Conflict
        );

        public static TenantErrors UserNotInTenant => new TenantErrors(
            "Tenant.UserNotInTenant",
            "User not found on Tenant",
            ErrorType.NotFound
        );

        public static TenantErrors NotMember => new TenantErrors(
            "Tenant.NotMember",
            "User not a member of any Tenant",
            ErrorType.NotFound
        );

        public static TenantErrors TenantIsFull => new TenantErrors(
            "Tenant.IsFull",
            "Tenant Is Currently Full",
            ErrorType.Conflict
        );

    }
}