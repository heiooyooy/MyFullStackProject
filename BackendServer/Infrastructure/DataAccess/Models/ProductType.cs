using System.Runtime.Serialization;
using NpgsqlTypes;

namespace Infrastructure.DataAccess.Models;

public enum ProductType
{
    [PgName("EBOOK")]
    EBOOK,
    [PgName("COURSE")]
    COURSE,
    [PgName("SOFTWARE")]
    SOFTWARE
}

public enum OrderStatus
{
    [PgName("PENDING")]
    PENDING,
    [PgName("PAID")]
    PAID,
    [PgName("SHIPPED")]
    SHIPPED,
    [PgName("DELIVERED")]
    DELIVERED,
    [PgName("CANCELLED")]
    CANCELLED
}