﻿using Havit.Blazor.Components.Web.Bootstrap.Internal;

namespace Havit.Blazor.Components.Web.Bootstrap;

/// <summary>
/// MultiSelect. Unlike a normal select, multiselect allows the user to select multiple options at once.<br />
/// Full documentation and demos: <see href="https://havit.blazor.eu/components/HxMultiSelect">https://havit.blazor.eu/components/HxMultiSelect</see>
/// </summary>
/// <typeparam name="TValue">Type of values.</typeparam>
/// <typeparam name="TItem">Type of items.</typeparam>
public class HxMultiSelect<TValue, TItem> : HxInputBase<List<TValue>>, IInputWithSize
{
	/// <summary>
	/// Return <see cref="HxMultiSelect{TValue, TItem}"/> defaults.
	/// Enables to not share defaults in descendants with base classes.
	/// Enables to have multiple descendants which differs in the default values.
	/// </summary>
	protected override MultiSelectSettings GetDefaults() => HxMultiSelect.Defaults;

	/// <summary>
	/// Set of settings to be applied to the component instance (overrides <see cref="HxMultiSelect.Defaults"/>, overridden by individual parameters).
	/// </summary>
	[Parameter] public MultiSelectSettings Settings { get; set; }

	/// <summary>
	/// Returns optional set of component settings.
	/// </summary>
	/// <remarks>
	/// Similar to <see cref="GetDefaults"/>, enables defining wider <see cref="Settings"/> in components descendants (by returning a derived settings class).
	/// </remarks>
	protected override MultiSelectSettings GetSettings() => this.Settings;

	/// <summary>
	/// Size of the input.
	/// </summary>
	[Parameter] public InputSize? InputSize { get; set; }
	protected InputSize InputSizeEffective => this.InputSize ?? GetSettings()?.InputSize ?? GetDefaults()?.InputSize ?? throw new InvalidOperationException(nameof(InputSize) + " default for " + nameof(HxMultiSelect) + " has to be set.");
	InputSize IInputWithSize.InputSizeEffective => this.InputSizeEffective;
	string IInputWithSize.GetInputSizeCssClass() => this.InputSizeEffective.AsFormSelectCssClass();

	private protected override string CoreInputCssClass => "hx-multi-select-input form-select user-select-none";

	/// <summary>
	/// Items to display. 
	/// </summary>
	[Parameter] public IEnumerable<TItem> Data { get; set; }

	/// <summary>
	/// Selects text to display from item.<br/>
	/// When not set, <c>ToString()</c> is used.
	/// </summary>
	[Parameter] public Func<TItem, string> TextSelector { get; set; }

	/// <summary>
	/// Selects value from item.<br/>
	/// Not required when <c>TValue</c> is same as <c>TItem</c>.
	/// </summary>
	[Parameter] public Func<TItem, TValue> ValueSelector { get; set; }

	/// <summary>
	/// Selects value for items sorting. When not set, <see cref="TextSelector"/> property will be used.<br/>
	/// If you need complex sorting, pre-sort data manually or create a custom comparable property.
	/// </summary>
	[Parameter] public Func<TItem, IComparable> SortKeySelector { get; set; }

	/// <summary>
	/// When set to <c>false</c>, items will no longer be sorted.<br/>
	/// Default value is <c>true</c>.
	/// </summary>
	[Parameter] public bool AutoSort { get; set; } = true;

	/// <summary>
	/// Text to display when the selection is empty (the <c>Value</c> property is <c>null</c> or empty).
	/// </summary>
	[Parameter] public string EmptyText { get; set; }

	/// <summary>
	/// Text to display when <see cref="Data"/> is <c>null</c>.
	/// </summary>
	[Parameter] public string NullDataText { get; set; }

	/// <summary>
	/// Input-group at the beginning of the input.
	/// </summary>
	[Parameter] public string InputGroupStartText { get; set; }

	/// <summary>
	/// Input-group at the beginning of the input.
	/// </summary>
	[Parameter] public RenderFragment InputGroupStartTemplate { get; set; }

	/// <summary>
	/// Input-group at the end of the input.
	/// </summary>
	[Parameter] public string InputGroupEndText { get; set; }

	/// <summary>
	/// Text to display in the input (default is a list of selected values).
	/// </summary>
	[Parameter] public string InputText { get; set; }

	/// <summary>
	/// Function to build the text to be displayed in the input from selected items (default is a list of selected values).
	/// </summary>
	/// <remarks>Currently does not affect the chip being generated. Override <c>RenderChipValue()</c> method to influence the chip.</remarks>
	[Parameter] public Func<IEnumerable<TItem>, string> InputTextSelector { get; set; }

	/// <summary>
	/// Input-group at the end of the input.
	/// </summary>
	[Parameter] public RenderFragment InputGroupEndTemplate { get; set; }

	/// <summary>
	/// Enables filtering capabilities.
	/// </summary>
	[Parameter] public bool EnableFiltering { get; set; }

	[Parameter] public Func<TItem, string, bool> FilterPredicate { get; set; } = (_, _) => true;

	[Parameter] public bool ClearFilterOnHide { get; set; }

	/// <summary>
	/// This event is fired when a dropdown element has been made visible to the user.
	/// </summary>
	[Parameter] public EventCallback<string> OnShown { get; set; }

	/// <summary>
	/// This event is fired when a dropdown element has been hidden from the user.
	/// </summary>
	[Parameter] public EventCallback<string> OnHidden { get; set; }

	/// <summary>
	/// Template that defines what should be rendered in case of empty items.
	/// </summary>
	[Parameter] public RenderFragment EmptyTemplate { get; set; }

	private List<TItem> itemsToRender;
	private HxMultiSelectInternal<TValue, TItem> hxMultiSelectInternalComponent;

	private void RefreshState()
	{
		itemsToRender = Data?.ToList();

		// AutoSort
		if (AutoSort && (itemsToRender?.Count > 1))
		{
			if (SortKeySelector != null)
			{
				itemsToRender = itemsToRender.OrderBy(this.SortKeySelector).ToList();
			}
			else if (TextSelector != null)
			{
				itemsToRender = itemsToRender.OrderBy(this.TextSelector).ToList();
			}
			else
			{
				itemsToRender = itemsToRender.OrderBy(i => i.ToString()).ToList();
			}
		}
	}

	private void HandleItemSelectionChanged(bool @checked, TItem item)
	{
		var newValue = Value == null ? new List<TValue>() : new List<TValue>(Value);
		TValue value = SelectorHelpers.GetValue<TItem, TValue>(ValueSelector, item);
		if (@checked)
		{
			newValue.Add(value);
		}
		else
		{
			newValue.Remove(value);
		}

		CurrentValue = newValue; // setter includes ValueChanged + NotifyFieldChanged
	}

	protected override bool TryParseValueFromString(string value, out List<TValue> result, out string validationErrorMessage)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc cref="HxInputBase{TValue}.FocusAsync"/>
	public override async ValueTask FocusAsync()
	{
		if (hxMultiSelectInternalComponent == null)
		{
			throw new InvalidOperationException($"Cannot focus {this.GetType()}. The method must be called after first render.");
		}

		await hxMultiSelectInternalComponent.FocusAsync();
	}

	/// <inheritdoc/>
	protected override void BuildRenderInput(RenderTreeBuilder builder)
	{
		RefreshState();

		builder.OpenComponent<HxMultiSelectInternal<TValue, TItem>>(100);
		builder.AddAttribute(101, nameof(HxMultiSelectInternal<TValue, TItem>.InputId), InputId);
		builder.AddAttribute(102, nameof(HxMultiSelectInternal<TValue, TItem>.InputCssClass), GetInputCssClassToRender());
		builder.AddAttribute(103, nameof(HxMultiSelectInternal<TValue, TItem>.InputText), GetInputText());
		builder.AddAttribute(104, nameof(HxMultiSelectInternal<TValue, TItem>.EnabledEffective), EnabledEffective);
		builder.AddAttribute(105, nameof(HxMultiSelectInternal<TValue, TItem>.ItemsToRender), itemsToRender);
		builder.AddAttribute(106, nameof(HxMultiSelectInternal<TValue, TItem>.TextSelector), TextSelector);
		builder.AddAttribute(107, nameof(HxMultiSelectInternal<TValue, TItem>.ValueSelector), ValueSelector);
		builder.AddAttribute(108, nameof(HxMultiSelectInternal<TValue, TItem>.SelectedValues), Value);
		builder.AddAttribute(109, nameof(HxMultiSelectInternal<TValue, TItem>.NullDataText), NullDataText);
		builder.AddAttribute(110, nameof(HxMultiSelectInternal<TValue, TItem>.ItemSelectionChanged), EventCallback.Factory.Create<HxMultiSelectInternal<TValue, TItem>.SelectionChangedArgs>(this, args => HandleItemSelectionChanged(args.Checked, args.Item)));
		builder.AddAttribute(111, nameof(HxMultiSelectInternal<TValue, TItem>.InputGroupStartText), InputGroupStartText);
		builder.AddAttribute(112, nameof(HxMultiSelectInternal<TValue, TItem>.InputGroupStartTemplate), InputGroupStartTemplate);
		builder.AddAttribute(113, nameof(HxMultiSelectInternal<TValue, TItem>.InputGroupEndText), InputGroupEndText);
		builder.AddAttribute(114, nameof(HxMultiSelectInternal<TValue, TItem>.InputGroupEndTemplate), InputGroupEndTemplate);
		builder.AddAttribute(115, nameof(HxMultiSelectInternal<TValue, TItem>.EnableFiltering), EnableFiltering);
		builder.AddAttribute(116, nameof(HxMultiSelectInternal<TValue, TItem>.FilterPredicate), FilterPredicate);
		builder.AddAttribute(117, nameof(HxMultiSelectInternal<TValue, TItem>.OnHidden), OnHidden);
		builder.AddAttribute(118, nameof(HxMultiSelectInternal<TValue, TItem>.OnShown), OnShown);
		builder.AddAttribute(119, nameof(HxMultiSelectInternal<TValue, TItem>.ClearFilterOnHide), ClearFilterOnHide);
		builder.AddAttribute(120, nameof(HxMultiSelectInternal<TValue, TItem>.EmptyTemplate), EmptyTemplate);

		builder.AddMultipleAttributes(200, this.AdditionalAttributes);

		builder.AddComponentReferenceCapture(300, r => hxMultiSelectInternalComponent = (HxMultiSelectInternal<TValue, TItem>)r);

		builder.CloseComponent();
	}

	private string GetInputText()
	{
		if (!string.IsNullOrEmpty(InputText))
		{
			return InputText;
		}

		if ((InputTextSelector is null) || (Data is null) || (CurrentValue is null))
		{
			return CurrentValueAsString;
		}

		var currentItems = Data.Where(i => CurrentValue.Contains(SelectorHelpers.GetValue(ValueSelector, i)));
		return SelectorHelpers.GetValue(InputTextSelector, currentItems);
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(List<TValue> value)
	{
		// Used for CurrentValueAsString (which is used for the input element and for the chip generator).
		// That's why we do not use NullDataText here.

		if ((!value?.Any() ?? true) || (Data == null))
		{
			// don't care about chip generator, it does not call this method for null/empty value
			return EmptyText;
		}

		// Take itemsToRender because they are sorted.
		List<TItem> selectedItems = itemsToRender.Where(item => value.Contains(SelectorHelpers.GetValue<TItem, TValue>(ValueSelector, item))).ToList();
		return String.Join(", ", selectedItems.Select(TextSelector));
	}

	/// <inheritdoc />
	protected override bool ShouldRenderChipGenerator()
	{
		return CurrentValue?.Any() ?? false;
	}

	/// <inheritdoc />
	protected override List<TValue> GetChipRemoveValue()
	{
		return new List<TValue>();
	}
}
