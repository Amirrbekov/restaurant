using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private ApplicationDbContext _db;

    public ApplicationUserRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(ApplicationUser applicationUser)
    {
        _db.Update(applicationUser);
    }

    //public ApplicationUser GetUserByUsername(string username)
    //{
    //    return _db.ApplicationUsers.FirstOrDefault(u => u.UserName == username);
    //}
}
