using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private ApplicationDbContext _db;

    public OrderHeaderRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(OrderHeader obj)
    {
        _db.OrderHeaders.Update(obj);
    }

    public void UpdateStatus(int id, string OrderStatus, string? paymentStatus = null)
    {
        var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.OrderHeaderId == id);
        if (orderFromDb != null)
        {
            orderFromDb.OrderStatus = OrderStatus;
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                orderFromDb.PaymentStatus = paymentStatus;
            }
        }
    }

    public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
    {
        var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.OrderHeaderId == id);
        if (!string.IsNullOrEmpty(sessionId))
        {
            orderFromDb.SessionId = sessionId;

        }
        if (!string.IsNullOrEmpty(paymentIntentId))
        {
            orderFromDb.PaymentIntentId = paymentIntentId;
            orderFromDb.PaymentDate = DateTime.Now;

        }
    }

}