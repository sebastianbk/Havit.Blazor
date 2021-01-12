﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Components;

namespace Havit.Blazor.Components.Web.Bootstrap
{
	public partial class HxMessageBoxHost : ComponentBase
	{
		[Inject] protected IHxMessageBoxService MessageBoxService { get; set; }

		private HxMessageBox messageBox;
		private MessageBoxRequest request;
		private TaskCompletionSource<MessageBoxButtons> resultCompletion;

		protected override void OnInitialized()
		{
			base.OnInitialized();

			MessageBoxService.ShowAsyncFunc = HandleShowRequest;
		}

		private async Task<MessageBoxButtons> HandleShowRequest(MessageBoxRequest request)
		{
			this.request = request;
			this.resultCompletion = new TaskCompletionSource<MessageBoxButtons>();

			StateHasChanged();

			await messageBox.ShowAsync();

			return await this.resultCompletion.Task;
		}

		private void HandleClosed(MessageBoxButtons button)
		{
			_ = resultCompletion.TrySetResult(button);
		}
	}
}
