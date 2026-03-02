using Gobln.Pager;
using Gobln.Pager.TestHelper.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//todo https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking

namespace Gobln.Pager.TestHelper.EFHelper;

public class UserService
{
    private UserContext _context;

    public UserService(UserContext context)
    {
        _context = context;
    }

    public User AddUser(int id, string name, string surName, DateTime date)
    {
        var blog = _context.Users.Add(new User { Id = id, Name = name, SurName = surName, Date = date });
        _context.SaveChanges();

        return blog.Entity;
    }

    public IEnumerable<User> GetAllUsers()
    {
        var query = _context.Users;

        return query.ToList();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var query = _context.Users;

        return await query.ToListAsync();
    }

    public Page<User> GetPageUsers(IPageFilter pageFilter)
    {
        var query = _context.Users;

        return query.ToPage(pageFilter);
    }

    public async Task<Page<User>> GetPageUsersAsync(IPageFilter pageFilter, CancellationToken cancellationToken)
    {
        var query = _context.Users;

        return await query.ToPageAsync(pageFilter, cancellationToken);
    }


    public PagedList<User> GetPagedListUsers()
    {
        var query = _context.Users;

        return query.ToPagedList();
    }

    public async Task<PagedList<User>> GetPagedListUsersAsync(CancellationToken cancellationToken)
    {
        var query = _context.Users;

        return await query.ToPagedListAsync(cancellationToken);
    }
}
