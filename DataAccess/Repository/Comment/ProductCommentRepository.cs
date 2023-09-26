using DataAccess.Data;
using DataAccess.Repository.IRepository.Comment;
using Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Comment;

public class ProductCommentRepository : Repository<ProductComment>, IProductCommentRepository
{
    private readonly ApplicationDbContext _db;

    public ProductCommentRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<ProductComment> AddAsync(ProductComment productComment)
    {
        await _db.ProductComments.AddAsync(productComment);
        await _db.SaveChangesAsync();
        return productComment;
    }
}
