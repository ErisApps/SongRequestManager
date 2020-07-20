using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SongRequestManager.Commands;
using SongRequestManager.Services.Interfaces;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.Services
{
	public class CommandManager : ICommandManager
	{
		private readonly ISongQueueService _songQueueService;
		private readonly IBeatSaverService _beatSaverService;
		private readonly IUserRequestTrackerManager _userRequestTrackerManager;

		private List<ICommand> _commands;

		public CommandManager(ISongQueueService songQueueService, IBeatSaverService beatSaverService, IUserRequestTrackerManager userRequestTrackerManager)
		{
			_songQueueService = songQueueService;
			_beatSaverService = beatSaverService;
			_userRequestTrackerManager = userRequestTrackerManager;

			_commands = new List<ICommand>();
		}

		public void Setup()
		{
			_commands.Clear();
			var possibleParameters = new List<object>{_beatSaverService, _songQueueService, _userRequestTrackerManager};

			// TODO: Manually register all commands instead of using reflection, might speed up command loading roughly 9-10 times on my machine.
			// TODO: The reflection init way is still useful during development though
			// _commands.Add(new RequestCommand(_songQueueService, _beatSaverService));

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			_commands.AddRange(Assembly.GetExecutingAssembly().DefinedTypes
				.Where(ti => typeof(ICommand).IsAssignableFrom(ti) && !ti.IsAbstract)
				.Select(t => t.GetConstructors().OrderByDescending(ctor => ctor.GetParameters().Length).First())
				.Where(c => c != null)
				.Select(c =>
				{
					var instantiatedParams = new object[c.GetParameters().Length];
					for (var i = 0; i < c.GetParameters().Length; i++)
					{
						var parameterInfo = c.GetParameters()[i];
						instantiatedParams[i] = possibleParameters.Find(x => parameterInfo.ParameterType.IsInstanceOfType(x));

						if (instantiatedParams[i] == null)
						{
							Logger.Log($"Couldn't find parameter of type {parameterInfo.Name}");
						}
					}

					return c.Invoke(instantiatedParams);
				})
				.Cast<ICommand>());

			stopwatch.Stop();
			
			Logger.Log($"Command initialization took {stopwatch.Elapsed:c}");
		}

		public ICommand FindCommand(string commandName)
		{
			return _commands.FirstOrDefault(c => c.Alias.Contains(commandName));
		}

		public void Dispose()
		{
		}
	}
}