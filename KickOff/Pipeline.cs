using System;
using System.Collections.Generic;

namespace KickOff
{
	public class Pipeline : IDisposable
	{
		private readonly List<Stage> _stages;

		public Pipeline()
		{
			_stages = new List<Stage>();
		}

		public void Execute(IEnumerable<Stage> stages, string[] startArgs)
		{
			Func<Type, object> factory = type => type.GetConstructor(Type.EmptyTypes).Invoke(null);

			var args = new StageArgs(startArgs);

			foreach (var stage in stages)
			{
				stage.InstanceFactory = factory;
				stage.Execute(args);

				factory = stage.InstanceFactory;
				_stages.Add(stage);
			}
		}

		public void Dispose()
		{
			for (var i = _stages.Count - 1; i >= 0; i--)
				_stages[i].Dispose();
		}
	}
}
