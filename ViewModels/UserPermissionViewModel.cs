public class UserPermissionViewModel
{
    public int UserId {get;set;}
    public string Username {get;set;}
    public List<PermissionItemViewModel> Permissions {get;set;} = new();
}