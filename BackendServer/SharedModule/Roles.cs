namespace SharedModule;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Manager = "Manager";
    public const string Support = "Support";

    public static IReadOnlyList<string> AllRoles = new List<string>()
    {
        Admin, User, Manager, Support
    };
}