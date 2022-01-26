Logger logger = new Logger();

while (true)
{
	logger.Log();
}

public class Logger
{
	public void Log()
	{
		Console.WriteLine($"{DateTime.Now}: Hello Inflection! We are running at {System.Diagnostics.Process.GetCurrentProcess().Id}");
		Thread.Sleep(1000);
	}
}
