using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using FluxoCaixa.Domain.Commons;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FluxoCaixa.Infrastructure.Repositories;

/// <summary>
/// Base Repository for basic CRUD operations
/// </summary>
public abstract class AbstractRepository<E> : IRepository<E> where E : Entity
{
   private readonly DbSet<E> dbSet;

   private readonly SqlServerContext context;

   public AbstractRepository(IServiceProvider provider)
   {
      this.context = provider.GetRequiredService<SqlServerContext>();
      this.dbSet = context.Set<E>();
   }

   public IQueryable<E> Query(Expression<Func<E, bool>> predicate) =>
      this.dbSet.Where(predicate).AsQueryable().AsNoTracking();

   public Task<E?> LoadAsync(Guid id) =>
      dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

   public Task<E?> LoadAsync(Expression<Func<E, bool>> predicate) =>
      Query(predicate).AsQueryable().FirstOrDefaultAsync();

   public Task<List<E>> ListAsync() => dbSet.AsNoTracking().ToListAsync();

   public Task<List<E>> ListAsync(Expression<Func<E, bool>> predicate) => Query(predicate).ToListAsync();

   public Task<long> CountAsync() => dbSet.AsNoTracking().LongCountAsync();

   public Task<bool> ExistsAsync(Guid id) => Query(x => x.Id == id).AnyAsync();

   public Task<bool> ExistsAsync(Expression<Func<E, bool>> predicate) => Query(predicate).AnyAsync();

   public async Task<E> SaveAsync(E entity)
   {
      try
      {
         if (entity.Id == Guid.Empty)
         {
            entity.Id = Guid.NewGuid();
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
         }

         var current = await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
         if (current is null) throw new Exception("Entidade não encontrada");
         if (current.Equals(entity)) return current;
         if (current.GetType() != entity.GetType()) throw new Exception("Tipo de entidade diferente");
         if (current.Id != entity.Id) throw new Exception("Id de entidade diferente");

         // Atualiza os campos da entidade atual com os valores da entidade recebida
         foreach (var property in typeof(E).GetProperties())
         {
            if (property.CanWrite)
            {
               var newValue = property.GetValue(entity);
               property.SetValue(current, newValue);
            }
         }

         context.Entry(current).State = EntityState.Modified;
         context.Entry(current).OriginalValues.SetValues(entity);

         dbSet.Update(current);
         await context.SaveChangesAsync();
         return current;
      }
      catch (DbUpdateConcurrencyException ex)
      {
         throw new Exception("Erro de concorrência", ex);
      }
      catch (DbUpdateException ex)
      {
         throw new Exception("Erro ao atualizar entidade", ex);
      }
      catch (Exception ex)
      {
         throw new Exception("Erro inesperado", ex);
      }
   }

   public async Task<bool> DropAsync(Guid id)
   {
      try
      {
         var entidade = await dbSet.FirstOrDefaultAsync(x => x.Id == id);

         if (entidade is null) return false;
         else dbSet.Remove(entidade);
         await context.SaveChangesAsync();

         context.ChangeTracker.Clear();

         return true;
      }
      catch (DbUpdateConcurrencyException) { return false; }
      catch (DbUpdateException) { return false; }
      catch (Exception) { return false; }
   }

   public async Task<bool> DropAsync(Expression<Func<E, bool>> predicate)
   {
      var entities = await this.Query(predicate).ToListAsync();

      foreach (var entity in entities)
         await DropAsync(entity.Id);

      return true;
   }
}