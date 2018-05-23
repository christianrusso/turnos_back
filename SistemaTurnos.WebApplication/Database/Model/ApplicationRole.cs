using Microsoft.AspNetCore.Identity;

namespace SistemaTurnos.WebApplication.Database.Model
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string name) : this()
        {
            Name = name;
        }
    }
}
