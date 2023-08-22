using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Blog;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;
	public ProductRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

    public ShopViewModel GetAllPostsPage(int pageNumber, string category, string search)
    {
        Func<Product, bool> InCategory = (product) => { return product.Category.Name.ToLower().Equals(category.ToLower()); };

        int pageSize = 8;
        int skipAmount = pageSize * (pageNumber - 1);

        var query = _db.Products.AsNoTracking().AsQueryable();

        if (!String.IsNullOrEmpty(category))
            query = query.Where(x => InCategory(x));

        if (!String.IsNullOrEmpty(search))
            query = query.Where(x => EF.Functions.Like(x.Name, $"%{search}%")
                                || EF.Functions.Like(x.Description, $"%{search}%"));

        int productsCount = query.Count();
        int pageCount = (int)Math.Ceiling((double)productsCount / pageSize);

        return new ShopViewModel
        {
            PageNumber = pageNumber,
            PageCount = pageCount,
            NextPage = productsCount > skipAmount + pageSize,
            Pages = PageHelper.PageNumbers(pageNumber, pageCount).ToList(),
            Category = category,
            Search = search,
            Products = query
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList()
        };
    }

    public IQueryable<Product> Include(params Expression<Func<Product, object>>[] includeProperties)
	{
		IQueryable<Product> query = _dbSet;

		foreach (var includeProperty in includeProperties)
		{
			query = query.Include(includeProperty);
		}

		return query;
	}

    public void Update(Product obj)
    {
        var objFromDb = _db.Products.FirstOrDefault(x => x.ProductId == obj.ProductId);
        if (objFromDb != null)
        {
            objFromDb.Name = obj.Name;
            objFromDb.Description = obj.Description;
            objFromDb.SKU = obj.SKU;
            objFromDb.Price = obj.Price;
            objFromDb.CategoryId = obj.CategoryId;
            objFromDb.ProductImages = obj.ProductImages;
            //if (obj.Image != null)
            //{
            //    objFromDb.Image = obj.Image;
            //}
        }
    }
}
