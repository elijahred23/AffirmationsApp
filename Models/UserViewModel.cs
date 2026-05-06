public class UserViewModel
{
    public int Id {get;set;}
    public string Username {get;set;}
    public string Email {get;set;}
    public bool IsActive {get;set;}
    public List<string> Permissions {get;set;} = new();
}