using System;

namespace ProductionSim
{
	public interface ILogger
    {
    	void Log(string message, object parent, params object[] args);
    }
	
	public class Logger : ILogger
	{		
		public bool Debug { get; set; }
		public bool Console { get; set; }
		public bool Timestamp { get; set; }
		
		public Logger()
		{
			Console = true;
			Timestamp = true;
		}
		
		public void Log(string message, object parent, params object[] args)
		{
			var msg = string.Format("{0} {1}:\t", parent.GetType().Name, parent) + string.Format(message, args);
			if (Timestamp) msg = DateTime.Now + " " + msg;
			if (Debug) System.Diagnostics.Debug.Print(msg);
			if (Console) System.Console.WriteLine(msg);
		}
	}
	
	public abstract class Loggable
	{
		public ILogger Logger { get; set; }
		
		protected void Log(string message, params object[] args)
        {
        	if (Logger != null) Logger.Log(message, this, args);
        }
	}
}
