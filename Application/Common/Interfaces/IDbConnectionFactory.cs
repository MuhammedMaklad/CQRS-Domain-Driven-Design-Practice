

using System.Data;

namespace Application.Common.interfaces;


public interface IDbConnectionFactory
{
  IDbConnection CreateConnection();
}
