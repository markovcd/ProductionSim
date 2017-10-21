using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ProductionSim
{
	public class Simulation : Loggable, ISimulation, IXmlSerializable
	{
	    private ISet<IBlock> _blocks;
	    private ISet<IBuffer> _buffers;
	    private ISet<IPart> _parts;
	    
        public int Ticks { get; protected set; }
	    public int IdleTicks { get; protected set; }
        
	    public IEnumerable<IBlock> Blocks { get { return _blocks; } }
	    
        public IEnumerable<IBuffer> Buffers 
        { 
        	get 
        	{ 
        		return _buffers = _buffers ?? 
        						  _blocks.Select(b => b.InputBuffer)
        				  	  			 .Concat(_blocks.Select(b => b.OutputBuffer))
        				  	  			 .ToHashSet(); 
        	} 
        }
        
		public IEnumerable<IPart> Parts 
		{ 
			get 
			{ 
				return _parts = _parts ?? 
								_blocks.SelectMany(b => b.ProducesParts)
							  		   .Concat(_blocks.SelectMany(b => b.UsesParts))
									   .ToHashSet();
			}
		}

        private Simulation() { }

	    public Simulation(IEnumerable<IBlock> blocks, ILogger logger = null)
		{
	    	_blocks = blocks.ToHashSet();
	    	Logger = logger;
	    	Log("Created.");
		}   

	    public void Tick()
	    {
	        Ticks++;
	        Log("Tick {0}.", Ticks);
	        
	        foreach (var block in Blocks) 
	        {
	        	var idle = block.IdleTicks;
	        	block.Tick();
	        	IdleTicks = IdleTicks + block.IdleTicks - idle;
	        }
	    }

        public void ResetState()
        {
            Ticks = 0;
            IdleTicks = 0;
            
            foreach (var block in Blocks) block.ResetState();
            foreach (var buffer in Buffers) buffer.Clear();
            
            Log("Resetting state.");
            	
        }
        
        #region IXmlSerializable implementation

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			_blocks = SimulationDeserializer.ReadXml(reader);
		}

		public void WriteXml(XmlWriter writer)
		{		
			SimulationSerializer.WriteXml(this, writer);
		}

        #endregion

    }
}
