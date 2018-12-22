using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace DBManager
{
	public partial class compdbEntities : DbContext
	{
		public event SaveChangesEventHandler ChangesSaved;

		private void RaiseChangesSaved(List<OneEntityChanging> changes)
		{
			ChangesSaved?.Invoke(this, new SaveChangesEventArgs(changes));
		}

		//
		// Summary:
		//     Saves all changes made in this context to the underlying database.
		//
		// Returns:
		//     The number of state entries written to the underlying database. This can include
		//     state entries for entities and/or relationships. Relationship state entries are
		//     created for many-to-many relationships and relationships where there is no foreign
		//     key property included in the entity class (often referred to as independent associations).
		//
		// Exceptions:
		//   T:System.Data.Entity.Infrastructure.DbUpdateException:
		//     An error occurred sending updates to the database.
		//
		//   T:System.Data.Entity.Infrastructure.DbUpdateConcurrencyException:
		//     A database command did not affect the expected number of rows. This usually indicates
		//     an optimistic concurrency violation; that is, a row has been changed in the database
		//     since it was queried.
		//
		//   T:System.Data.Entity.Validation.DbEntityValidationException:
		//     The save was aborted because validation of entity property values failed.
		//
		//   T:System.NotSupportedException:
		//     An attempt was made to use unsupported behavior such as executing multiple asynchronous
		//     commands concurrently on the same context instance.
		//
		//   T:System.ObjectDisposedException:
		//     The context or connection have been disposed.
		//
		//   T:System.InvalidOperationException:
		//     Some error occurred attempting to process entities in the context either before
		//     or after sending commands to the database.
		public override int SaveChanges()
		{
			List<OneEntityChanging> changes = new List<OneEntityChanging>();

			var modifiedEntities = ChangeTracker
				.Entries()
				.Where(p => p.State == EntityState.Modified)
				.ToList();

            foreach (var change in modifiedEntities)
			{
				OneEntityChanging entityHasBeenChanged = new OneEntityChanging(change.Entity);
				
				foreach (var propertyName in change.OriginalValues.PropertyNames)
				{
					OnePropertyChanging propValues = new OnePropertyChanging(change.OriginalValues[propertyName], change.OriginalValues[propertyName]);

					if (!(propValues.OldValue == null && propValues.NewValue == null)
						&& ((propValues.OldValue == null && propValues.NewValue != null)
						|| (propValues.OldValue != null && propValues.NewValue == null)
						|| !propValues.OldValue.Equals(propValues.NewValue)))
					{
                        entityHasBeenChanged.PropertiesHasBeenChanged.Add(propertyName, propValues);
                        changes.Add(entityHasBeenChanged);
                    }
				}
			}

            int result = base.SaveChanges();

            RaiseChangesSaved(changes);

            return result;
        }
	}
}
