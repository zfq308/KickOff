using System;
using System.Threading;
using System.Threading.Tasks;

namespace KickOff.Stages
{
	public class AsyncRunnerStage : IStage
	{
		private ServiceArgs _serviceArgs;
		private IStartup _startup;

		private readonly CancellationTokenSource _source;
		private readonly Task _runner;

		public AsyncRunnerStage()
		{
			_source = new CancellationTokenSource();

			_runner = new Task(() =>
			{
				try
				{
					_startup.Execute(_serviceArgs);
				}
				catch (TaskCanceledException)
				{
				}
			}, _source.Token);
		}

		public virtual void OnStart(StageArgs args)
		{
			_startup = args.TryGetInstance<IStartup>();

			if (_startup == null)
				throw new StartupNotFoundException();

			_serviceArgs = new ServiceArgs(args.StartArgs, () => _source.IsCancellationRequested);

			_runner.Start();
		}

		public virtual void OnStop(StageArgs args)
		{
			try
			{
				_source.Cancel();
				_runner.Wait();
			}
			catch (TaskCanceledException)
			{
			}

			(_startup as IDisposable)?.Dispose();
		}
	}
}
