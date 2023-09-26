using Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository.Comment;

public interface IProductCommentRepository : IRepository<ProductComment>
{
    Task<ProductComment> AddAsync(ProductComment productComment);
}
