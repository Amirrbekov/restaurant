using DataAccess.Data;
using DataAccess.Repository.IRepository.Comment;
using Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Comment;

public class CommentRepository : Repository<Comments>, ICommentRepository
{
	private readonly ApplicationDbContext _db;

	public CommentRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

	public async Task<Comments> AddAsync(Comments comments)
	{
		await _db.Comments.AddAsync(comments);
		await _db.SaveChangesAsync();
		return comments;
	}
}
