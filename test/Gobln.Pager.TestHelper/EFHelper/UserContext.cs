using Gobln.Pager.TestHelper.Model;
using Microsoft.EntityFrameworkCore;

namespace Gobln.Pager.TestHelper.EFHelper
{
    public class UserContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
    }
}
