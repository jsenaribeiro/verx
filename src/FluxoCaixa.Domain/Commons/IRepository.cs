using System.Linq.Expressions;

namespace FluxoCaixa.Domain.Commons;

public interface IRepository<E> : IReadRepository<E>, IWriteRepository<E> where E : Entity { }

public interface IWriteRepository<E> where E : Entity
{
   Task<E> SaveAsync(E entity);

   Task<bool> DropAsync(Guid id);

   Task<bool> DropAsync(Expression<Func<E, bool>> predicate);
}

public interface IReadRepository<E> where E : Entity
{
   IQueryable<E> Query(Expression<Func<E, bool>> predicate);

   Task<E?> LoadAsync(Guid id);

   Task<E?> LoadAsync(Expression<Func<E, bool>> predicate);

   Task<List<E>> ListAsync();

   Task<List<E>> ListAsync(Expression<Func<E, bool>> predicate);

   Task<long> CountAsync();

   Task<bool> ExistsAsync(Guid id);

   Task<bool> ExistsAsync(Expression<Func<E, bool>> predicate);
}