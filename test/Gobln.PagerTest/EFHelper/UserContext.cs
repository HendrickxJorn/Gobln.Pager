using Gobln.PagerTest.Model;
using Microsoft.EntityFrameworkCore;

namespace Gobln.PagerTest.EFHelper
{
    public class UserContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
    }
}
