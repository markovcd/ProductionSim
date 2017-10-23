using System;
using System.Linq;
using System.Xml.Serialization;

namespace ProductionSim
{
	class Program
	{
		public static void Main(string[] args)
		{
			
			var logger = new Logger { Console = true, Debug = true, Timestamp = true };

            var p1 = new Part("A", 0);
            var p2 = new Part("B", 0);
            var p3 = new Part("C", 0);

            var p4 = new Part("D", 4, new [] {p1, p2});
            var p5 = new Part("E", 5, new []{p4, p3});
            
            var p6 = new Part("F", 3, new [] {p5, p4});
			
            var inB4 = new Buffer("inB4", 100);
			var inB5B6 = new Buffer("inB5B6", 100);
			var final = new Buffer("final", 1000);
			
            
            var b1 = new Block("b1", new[] { p1 }, new Buffer("inB1", 1), inB4, logger);
		    var b2 = new Block("b2", new[] { p2 }, new Buffer("inB2", 1), inB4, logger);
		    var b3 = new Block("b3", new[] { p3 }, new Buffer("inB3", 1), inB5B6, logger);
		    var b4 = new Block("b4", new[] { p4 }, inB4, inB5B6, logger);
		    var b5 = new Block("b5", new[] { p5 }, inB5B6, final, logger);
		    var b6 = new Block("b6", new[] { p6 }, inB5B6, final, logger);

           
            var s = new Simulation(new[] {b1, b2, b3, b4, b5, b6}, logger);
		    while (true)
		    {
		        s.Tick();
		        Console.ReadKey();
		    }
            /*var serializer = new XmlSerializer(typeof(Simulation));
		    using (var file = System.IO.File.OpenWrite(@"C:\Users\m25326\Desktop\a.txt"))
		    {
		        serializer.Serialize(file,s);

		    }*/
			//var s = Deserialize(@"C:\Users\m25326\Desktop\a.txt");
		  
		}
		
		private static Simulation Deserialize(string fileName)
		{
			var serializer = new XmlSerializer(typeof(Simulation));
			
			using (var file = System.IO.File.OpenRead(fileName))
		    {
		        return serializer.Deserialize(file) as Simulation;
		    }
		}
	}
}