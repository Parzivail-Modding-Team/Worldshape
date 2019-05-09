using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace Worldshape.Logging
{
	class Lumberjack
	{
		private static Logger _logger;

		public static void Init()
		{
			var config = new LoggingConfiguration();

			var format = "[${longdate}] [${threadname}/${level:uppercase=true}] [${logger}] ${message}";
			var layout = new SimpleLayout(format);

			var logfile = new FileTarget("logfile") { FileName = "output.log", Layout = layout};
			var logconsole = new ConsoleTarget("logconsole"){Layout = layout};

			config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
			config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
			
			LogManager.Configuration = config;

			_logger = LogManager.GetLogger("Worldshape");
		}

		public static void Debug(object message)
		{
			_logger.Debug(message.ToString());
		}

		public static void Info(object message)
		{
			_logger.Info(message.ToString());
		}

		public static void Error(object message)
		{
			_logger.Error(message.ToString());
		}

		public static void Fatal(object message)
		{
			_logger.Fatal(message.ToString());
		}
	}
}
