using AngleSharp.Dom;
using Bunit.Diffing;
using Bunit.Mocking.JSInterop;
using Bunit.Rendering;
using Bunit.Rendering.RenderEvents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using System;

namespace Bunit
{
	/// <summary>
	/// A test context is a factory that makes it possible to create components under tests.
	/// </summary>
	public class TestContext : TestContextBase, ITestContext
	{
		/// <summary>
		/// Creates a new instance of the <see cref="TestContext"/> class.
		/// </summary>
		public TestContext()
		{
			Services.AddSingleton<IJSRuntime>(new PlaceholderJsRuntime());
			Services.AddSingleton<TestHtmlParser>(srv => new TestHtmlParser(srv.GetRequiredService<ITestRenderer>()));
		}

		/// <summary>
		/// Instantiates and performs a first render of a component of type <typeparamref name="TComponent"/>.
		/// </summary>
		/// <typeparam name="TComponent">Type of the component to render</typeparam>
		/// <param name="parameters">Parameters to pass to the component when it is rendered</param>
		/// <returns>The rendered <typeparamref name="TComponent"/></returns>
		public IRenderedComponent<TComponent> RenderComponent<TComponent>(params ComponentParameter[] parameters) where TComponent : IComponent
		{
			var renderResult = Renderer.RenderComponent<TComponent>(parameters).GetAwaiter().GetResult();
			return new RenderedComponent<TComponent>(Services, renderResult.ComponentId, renderResult.Component);
		}


		/// <summary>
		/// Instantiates and performs a first render of a component of type <typeparamref name="TComponent"/>.
		/// </summary>
		/// <typeparam name="TComponent">Type of the component to render</typeparam>
		/// <param name="componentParameterBuilderAction">The ComponentParameterBuilder action to add type safe parameters to pass to the component when it is rendered</param>
		/// <returns>The rendered <typeparamref name="TComponent"/></returns>
		public virtual IRenderedComponent<TComponent> RenderComponent<TComponent>(Action<ComponentParameterBuilder<TComponent>> componentParameterBuilderAction) where TComponent : class, IComponent
		{
			if (componentParameterBuilderAction is null)
			{
				throw new ArgumentNullException(nameof(componentParameterBuilderAction));
			}

			var builder = new ComponentParameterBuilder<TComponent>();
			componentParameterBuilderAction(builder);

			var renderResult = Renderer.RenderComponent<TComponent>(builder.Build()).GetAwaiter().GetResult();
			return new RenderedComponent<TComponent>(Services, renderResult.ComponentId, renderResult.Component);
		}
	}
}