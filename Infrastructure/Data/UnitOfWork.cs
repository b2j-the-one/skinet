using System.Collections.Concurrent;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

/// <summary>
/// Cette classe (UnitOfWork) centralise la gestion des repositories 
/// et garantit que toutes les opérations de persistance sont sauvegardées 
/// en une seule transaction, tout en créant dynamiquement les repositories 
/// génériques et en les mettant en cache.
/// </summary>
/// <param name="context"></param>
public class UnitOfWork(StoreContext context) : IUnitOfWork
{
    /// <summary>
    /// Dictionnaire thread-safe.
    /// Clé = nom du type d’entité ("Product", "Order", etc.)
    /// Valeur = instance du repository correspondant.
    /// Objectif : ne créer qu’une seule fois chaque repository.
    /// </summary>
    private readonly ConcurrentDictionary<string, object> _repositories = new();

    /// <summary>
    /// Sauvegarde toutes les modifications
    /// 
    /// Appelle SaveChangesAsync() sur le StoreContext.
    /// C’est le cœur du pattern Unit of Work : valider toutes les opérations en une seule transaction.
    /// </summary>
    /// <returns>Retourne true si au moins une ligne a été modifiée.</returns>
    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Libère les ressources du StoreContext.
    /// Permet d’utiliser using var uow = new UnitOfWork(context);
    /// </summary>
    public void Dispose()
    {
        // La méthode Dispose() : Libère les ressources allouées à ce contexte.
        context.Dispose();
    }

    /// <summary>
    /// Crée dynamiquement et met en cache les repositories
    /// </summary>
    /// <typeparam name="TEntity">Ex : Product, Order</typeparam>
    /// <returns>Un repository typé correctement</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity).Name;

        return (IGenericRepository<TEntity>)_repositories.GetOrAdd(type, t => /// Si le repository existe déjà → il est retourné, sinon → il est créé via la fonction t => { ... }
        {
            /// Transforme GenericRepository<> en GenericRepository<Product> par exemple
            var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));
            return Activator.CreateInstance(repositoryType, context) // Crée une instance du repository en lui passant le StoreContext
                ?? throw new InvalidOperationException($"Impossible de créer l'instance de référentiel pour {t}");
        });
    }
}
