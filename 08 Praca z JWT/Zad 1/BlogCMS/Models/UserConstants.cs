namespace BlogCMS.Models
{
    public static class UserConstants
    {
        public static readonly List<LoginModel> Users =
        [
            new LoginModel
            {
                Username = "Grzegorz",
                Password = "TajneHaslo_1234",
                Role = "Admin"
            }
        ];
    }
}
