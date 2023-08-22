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

public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    private readonly ApplicationDbContext _db;
	public ProductImageRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

    public void Update(ProductImage obj)
    {
        
    }
}
