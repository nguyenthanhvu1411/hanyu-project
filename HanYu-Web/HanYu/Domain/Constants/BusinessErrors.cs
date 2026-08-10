namespace HanYu.Domain.Constants;

public static class BusinessErrors
{
    public const string UserNotFound = "IDENTITY.USER_NOT_FOUND";
    public const string CannotModifySelfSuperAdmin = "IDENTITY.CANNOT_MODIFY_SELF_SUPER_ADMIN";
    public const string LastSuperAdmin = "IDENTITY.LAST_SUPER_ADMIN";
    public const string CannotLockSelf = "IDENTITY.CANNOT_LOCK_SELF";
    public const string CannotDeleteSelf = "IDENTITY.CANNOT_DELETE_SELF";
    public const string CannotDemoteSelf = "IDENTITY.CANNOT_DEMOTE_SELF";
}
