using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Threading;


namespace DBManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		compdbEntities Entities = new compdbEntities();

		CollectionViewSource descriptionViewSource = null;
		ObservableCollectionEx<description> descriptionCollection = null;
		bool ChangesSavedSuccessfully = false;

		CollectionViewSource queryResultsViewSource = null;
		ObservableCollectionEx<dynamic> queryResultsCollection = new ObservableCollectionEx<dynamic>();
		
		CollectionViewSource groupsViewSource = null;

		IEnumerable<dynamic> queryResults = null;

		public MainWindow()
		{
			InitializeComponent();

			try
			{
				groupsViewSource = new CollectionViewSource();
				(groupsViewSource as ISupportInitialize).BeginInit();
				groupsViewSource.CollectionViewType = typeof(ListCollectionView);
				groupsViewSource.Source = Entities.groups;
				(groupsViewSource as ISupportInitialize).EndInit();
				
				Binding myBinding = new Binding()
				{
					Source = groupsViewSource
				};
				BindingOperations.SetBinding(groupsListView, ListView.ItemsSourceProperty, myBinding);
			}
			catch (Exception ex)
			{
				ex.ToString();
			}

			DataContext = this;
		}

		void descriptionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (description desc in e.NewItems)
						Entities.description.AddObject(desc);
					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (description desc in e.OldItems)
						Entities.description.DeleteObject(desc);
					break;
			}
				
			ChangesSavedSuccessfully = false;
			try
			{
				Entities.SaveChanges();
				ChangesSavedSuccessfully = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			queryResultsCollection.ReplaceRange(queryResults);
		}
				
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			descriptionViewSource = ((CollectionViewSource)(this.FindResource("descriptionViewSource")));
			descriptionCollection = new ObservableCollectionEx<description>(Entities.description);
			descriptionViewSource.Source = descriptionCollection;

			descriptionCollection.CollectionChanged += descriptionCollection_CollectionChanged;
			
			//groupsViewSource = ((CollectionViewSource)(this.FindResource("groupsViewSource")));
			//groupsViewSource.Source = Entities.groups;

			// Формируем запрос для вывода его в queryResultsDataGrid
			queryResults = from Desc in Entities.description
									  join Group in Entities.groups on Desc.id_desc equals Group.desc into CompGroups
							   from CompGroup in CompGroups.DefaultIfEmpty()
							   select new {
									desc_name = Desc.name,
									Desc.start_date,
									Desc.end_date,
									group_name = CompGroup.name,
									CompGroup.start_year,
									CompGroup.end_year
								};

			queryResultsCollection.AddRange(queryResults);

			queryResultsViewSource = new CollectionViewSource();
			(queryResultsViewSource as ISupportInitialize).BeginInit();
			queryResultsViewSource.CollectionViewType = typeof(ListCollectionView);
			queryResultsViewSource.Source = queryResultsCollection;
			(queryResultsViewSource as ISupportInitialize).EndInit();

			Binding myBinding = new Binding()
			{
				Source = queryResultsViewSource
			};
			BindingOperations.SetBinding(queryResultsDataGrid, DataGrid.ItemsSourceProperty, myBinding);
		}


		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			Entities.SaveChanges();

			groupsViewSource.Source = Entities.groups;

			Thread th = new Thread(() =>
			{
				queryResultsCollection.ReplaceRange(queryResults);
			})
			{
				IsBackground = true
			};
			th.SetApartmentState(ApartmentState.STA);
			th.Start();
		}

		
		private void btnAddDescription_Click(object sender, RoutedEventArgs e)
		{
			description Description = new description()
			{
				name = "Третьи соревнования",
				start_date = new DateTime(2016, 11, 25)
			};

			Thread th = new Thread(() =>
			{
				descriptionCollection.Add(Description);
				if (!ChangesSavedSuccessfully)
				{
					descriptionCollection.CollectionChanged -= descriptionCollection_CollectionChanged;
					descriptionCollection.Remove(Description);
					descriptionCollection.CollectionChanged += descriptionCollection_CollectionChanged;
				}
			})
			{
				IsBackground = true
			};
			th.SetApartmentState(ApartmentState.STA);
			th.Start();
		}
		
		
		private void btnDelDescription_Click(object sender, RoutedEventArgs e)
		{
			description Description = descriptionViewSource.View.CurrentItem as description;
			descriptionCollection.Remove(Description);
			if (!ChangesSavedSuccessfully)
			{
				descriptionCollection.CollectionChanged -= descriptionCollection_CollectionChanged;
				descriptionCollection.Add(Description);
				descriptionCollection.CollectionChanged += descriptionCollection_CollectionChanged;
			}
		}
		
		private void btnPrevDescription_Click(object sender, RoutedEventArgs e)
		{
			groupsViewSource.View.MoveCurrentToPrevious();
		}

		private void btnNextDescription_Click(object sender, RoutedEventArgs e)
		{
			groupsViewSource.View.MoveCurrentToNext();
		}

		ListSortDirection m_sortdir = ListSortDirection.Descending;
		private void btnSortGroups_Click(object sender, RoutedEventArgs e)
		{
			m_sortdir = m_sortdir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
			using (groupsViewSource.DeferRefresh())
			{
				groupsViewSource.SortDescriptions.Clear();
				SortDescription sd = new SortDescription("desc", m_sortdir);
				groupsViewSource.SortDescriptions.Add(sd);
			}

			btnSortGroups.Content = "Сортировать " + (m_sortdir == ListSortDirection.Ascending ? "(asc)" : "(desc)");
		}


		private void btnFilterGroups_Click(object sender, RoutedEventArgs e)
		{
			if (groupsViewSource.View.Filter == null)
				groupsViewSource.View.Filter = GirlsFilter;
			else
				groupsViewSource.View.Filter = null;
		}


		private bool GirlsFilter(object item)
		{
			groups group = item as groups;
			return group.name.Contains("девочки");
		}

		public void OnPropertyChanged(string info)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(info));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
