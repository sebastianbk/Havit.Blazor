﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Havit.Blazor.Components.Web.Infrastructure;
using Havit.Collections;
using Havit.Diagnostics.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Havit.Blazor.Components.Web.Bootstrap
{
	/// <summary>
	/// Grid column base class.
	/// </summary>
	public abstract class HxGridColumnBase<TItem> : ComponentBase, IHxGridColumn<TItem>, IDisposable
	{
		/// <summary>
		/// Cascading parameter to register column to the grid.
		/// </summary>
		[CascadingParameter(Name = HxGrid<TItem>.ColumnsRegistrationCascadingValueName)] protected CollectionRegistration<IHxGridColumn<TItem>> ColumnsRegistration { get; set; }

		RenderedEventHandler IRenderNotificationComponent.Rendered { get; set; }

		/// <inheritdoc />
		protected override void OnInitialized()
		{
			base.OnInitialized();

			Contract.Requires(ColumnsRegistration != null, $"Grid column invalid usage. Must be used in a {typeof(HxGrid<TItem>).FullName}.");
			ColumnsRegistration.Register(this);
		}

		/// <inheritdoc />
		bool IHxGridColumn<TItem>.IsVisible() => IsColumnVisible();

		/// <inheritdoc />
		int IHxGridColumn<TItem>.GetOrder() => this.GetColumnOrder();

		/// <inheritdoc />
		CellTemplate IHxGridColumn<TItem>.GetHeaderCellTemplate() => this.GetHeaderCellTemplate();

		/// <inheritdoc />
		CellTemplate IHxGridColumn<TItem>.GetItemCellTemplate(TItem item) => this.GetItemCellTemplate(item);

		/// <inheritdoc />
		CellTemplate IHxGridColumn<TItem>.GetItemPlaceholderCellTemplate(PlaceholderContext context) => this.GetItemPlaceholderCellTemplate(context);

		/// <inheritdoc />
		CellTemplate IHxGridColumn<TItem>.GetFooterCellTemplate() => this.GetFooterCellTemplate();

		/// <inheritdoc />
		SortingItem<TItem>[] IHxGridColumn<TItem>.GetSorting() => this.GetSorting().ToArray();

		/// <summary>
		/// Indicates whether the column is visible (otherwise the column is hidden).
		/// It is not suitable to conditionaly display the column using @if statement in the markup code.
		/// </summary>
		protected virtual bool IsColumnVisible() => true;

		/// <summary>
		/// Returns the column order.
		/// </summary>
		/// <remarks>
		/// Currently it ensures the correct order of MultiSelectGridColumn when enabled dynamically.		
		/// In future we can implement better in HxGridColumn to enable dynamic column order.
		/// </remarks>
		protected abstract int GetColumnOrder();

		/// <summary>
		/// Returns header cell template.
		/// </summary>
		protected abstract CellTemplate GetHeaderCellTemplate();

		/// <summary>
		/// Returns data cell template for the specific item.
		/// </summary>
		protected abstract CellTemplate GetItemCellTemplate(TItem item);

		/// <summary>
		/// Returns placeholder cell template.
		/// </summary>
		protected abstract CellTemplate GetItemPlaceholderCellTemplate(PlaceholderContext context);

		/// <summary>
		/// Returns footer cell template.
		/// </summary>
		protected abstract CellTemplate GetFooterCellTemplate();

		/// <summary>
		/// Returns column sorting.
		/// </summary>
		protected abstract IEnumerable<SortingItem<TItem>> GetSorting();

		/// <inheritdoc />
		protected override void OnAfterRender(bool firstRender)
		{
			base.OnAfterRender(firstRender);

			((IRenderNotificationComponent)this).Rendered?.Invoke(this, firstRender);
		}

		/// <inheritdoc />
		public virtual void Dispose()
		{
			ColumnsRegistration.Unregister(this);
		}
	}
}
