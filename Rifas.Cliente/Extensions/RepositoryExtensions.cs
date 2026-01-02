using System;
using System.Linq;
using System.Linq.Expressions;
using GCIT.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GCIT.Core.Extensions
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Permite obtener un IQueryable desde un IRepository y aplicar varios Include de forma cómoda.
        /// Usa AllNoTracking() si está disponible en la implementación del repositorio.
        /// </summary>
        public static IQueryable<T> QueryWithIncludes<T>(this IRepository<T> repository, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));

            // Intentar obtener IQueryable desde AllNoTracking() si el repositorio lo expone
            var repoType = repository.GetType();
            var allNoTrackingMethod = repoType.GetMethod("AllNoTracking");
            IQueryable<T> query;
            if (allNoTrackingMethod != null)
            {
                var res = allNoTrackingMethod.Invoke(repository, null);
                query = res as IQueryable<T> ?? throw new InvalidOperationException("AllNoTracking debe devolver IQueryable<T>");
            }
            else
            {
                // Fallback: intentar exponer un IQueryable mediante propiedad Queryable o método All
                var allMethod = repoType.GetMethod("All");
                if (allMethod != null)
                {
                    var res = allMethod.Invoke(repository, null);
                    query = res as IQueryable<T> ?? throw new InvalidOperationException("All debe devolver IQueryable<T>");
                }
                else
                {
                    throw new InvalidOperationException("El repositorio no expone AllNoTracking ni All para obtener IQueryable<T>.");
                }
            }

            if (includes == null || includes.Length == 0) return query;

            foreach (var inc in includes)
            {
                query = query.Include(inc);
            }

            return query;
        }
    }
}
