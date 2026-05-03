public class UserPermission
{
    public int UserId {get;set;}
    public int PermissionId {get;set;}
    public DateTime CreatedAt {get;set;}
    public Permission Permission {get;set;}
}