﻿using System.Linq;
using System.Threading;
using KickOff.Stages;
using NSubstitute;
using Xunit;

namespace KickOff.Tests.Stages
{
	public class RunnerStageTests
	{
		private readonly RunnerStage _runner;

		public RunnerStageTests()
		{
			_runner = new RunnerStage();
		}

		[Fact]
		public void Execute_returns_immediately()
		{
			var startup = Substitute.For<IStartup>();

			_runner.OnStart(new StageArgs(new string[0])
			{
				InstanceFactory = type => startup
			});

			Thread.Sleep(50);

			startup.Received().Execute(Arg.Any<ServiceArgs>());
		}
	}
}