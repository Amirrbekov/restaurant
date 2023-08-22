using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataAccess.Repository.IRepository.Blog;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Category obj)
    {
        var objFromDb = _db.Categories.FirstOrDefault(x => x.CategoryId == obj.CategoryId);

        if (objFromDb != null)
        {
            objFromDb.Name = obj.Name;
        }
    }

	public List<Category> ToList()
	{
		return _dbSet.ToList();
	}
}
