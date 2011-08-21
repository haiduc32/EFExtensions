using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Metadata.Edm;
using System.Linq.Expressions;

namespace EFExtensions
{
	public class ObjectSetWrapper<T> : ObjectQueryWrapper<T>
			where T : class
	{
		private ObjectSet<T> source;

		public ObjectSetWrapper(ObjectSet<T> source)
			: base(source)
		{
			this.source = source;
		}

		public ObjectSetWrapper(ObjectSet<T> source, Expression e)
			: base(source, e)
		{
			this.source = source;
		}

		public EntitySet EntitySet { get { return source.EntitySet; } }

		public void AddObject(T entity)
		{
			source.AddObject(entity);
		}

		public T ApplyCurrentValues(T currentEntity)
		{
			return source.ApplyCurrentValues(currentEntity);
		}

		public T ApplyOriginalValues(T originalEntity)
		{
			return source.ApplyOriginalValues(originalEntity);
		}

		public void Attach(T entity)
		{
			source.Attach(entity);
		}

		public TEntity CreateObject<TEntity>() where TEntity : class, T
		{
			return source.CreateObject<TEntity>();
		}

		public T CreateObject()
		{
			return source.CreateObject();
		}

		public void DeleteObject(T entity)
		{
			source.DeleteObject(entity);
		}

		public void Detach(T entity)
		{
			source.Detach(entity);
		}

	}
}
