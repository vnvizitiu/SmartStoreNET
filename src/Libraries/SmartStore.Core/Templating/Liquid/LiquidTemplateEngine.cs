﻿using System;
using System.Web.Hosting;
using DotLiquid;
using DotLiquid.NamingConventions;
using SmartStore.Core;
using SmartStore.Core.Events;
using SmartStore.Core.IO;

namespace SmartStore.Templating.Liquid
{
	public partial class LiquidTemplateEngine : ITemplateEngine
	{
		public LiquidTemplateEngine(IVirtualPathProvider vpp, IEventPublisher eventPublisher)
		{
			Template.NamingConvention = new CSharpNamingConvention();

			if (HostingEnvironment.IsHosted)
			{
				Template.FileSystem = new LiquidFileSystem(vpp);

				// Register tag "T"
				Template.RegisterTag<T>("T");

				// Register tag "zone"
				Template.RegisterTagFactory(new ZoneTagFactory(eventPublisher));
			}
		}

		public ITemplate Compile(string source)
		{
			Guard.NotEmpty(source, nameof(source));

			return new LiquidTemplate(Template.Parse(source), source);
		}

		public string Render(string template, object data, IFormatProvider formatProvider)
		{
			Guard.NotEmpty(template, nameof(template));
			Guard.NotNull(data, nameof(data));
			Guard.NotNull(formatProvider, nameof(formatProvider));

			return Compile(template).Render(data, formatProvider);
		}

		public ITestModel CreateTestModelFor(BaseEntity entity, string modelPrefix)
		{
			Guard.NotNull(entity, nameof(entity));

			return new TestDrop(entity, modelPrefix);
		}
	}
}
