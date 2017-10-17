using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ProductionSim
{

	public class Block : Loggable, IBlock, IXmlSerializable
	{
		private readonly string _name;
		private readonly ISet<IPart> _producesParts, _usesParts;
	    private IPart _nextPart;

		public string Name { get { return _name; } }
    
	    public IEnumerable<IPart> ProducesParts { get { return _producesParts; } }
	    public IEnumerable<IPart> UsesParts { get { return _usesParts; } }

	    public int Ticks { get; protected set; }
	    public int IdleTicks { get; protected set; }
	    public int CurrentPartTicksLeft { get; protected set; }
	    
	    public IPart CurrentPart { get; protected set; }
		
	    public IPart NextPart
	    {
	        get { return _nextPart; }
	        set
	        {
	            if (!_producesParts.Contains(value)) throw new InvalidOperationException(string.Format("Block {0} doesn't produce part {1}.", this, value));
	            _nextPart = value;
	        }
	    }    

	    public IBuffer InputBuffer { get; set; }
	    public IBuffer OutputBuffer { get; set; }

        
        public Block(string name, IEnumerable<IPart> producesParts, ILogger logger = null)
		{
		    _name = name;
		    _producesParts = producesParts.ToHashSet();
            _usesParts = producesParts.SelectMany(p => p.MadeFrom).ToHashSet();

			IdleTicks = 0;
		    Ticks = 0;
		    
		    Logger = logger;
		    
		    Log("Created.");
		}

	    public void ResetState()
	    {
	        IdleTicks = 0;
	        CurrentPartTicksLeft = 0;
            CurrentPart = null;
	        NextPart = null;
	        
	        Log("Resetting state.");
	    }

		public void Tick()
		{
		    
		    Ticks++;
			Log("Tick {0}.", Ticks);
		    
            if (CurrentPart == null && NextPart == null)
			{
				IdleTicks++;
			    Log("Idle ticks increased to {0}.", IdleTicks);
			}
			else if (CurrentPart == null && NextPart != null)
			{
				if (InputBuffer.CanMadePart(NextPart) && !OutputBuffer.Full)
				{
					CurrentPart = NextPart;
					NextPart = null;
					CurrentPartTicksLeft = CurrentPart.ManufactureTime;
				    Log("Setting current part to {0}.", CurrentPart);
				}
				else 
				{
					IdleTicks++;
				    Log("Idle ticks {0}.", IdleTicks);
				}
			}
			else
			{
				CurrentPartTicksLeft--;
				
			    Log("Ticks left to make part {0} is {1}.", CurrentPart, CurrentPartTicksLeft);
			    
			    if (CurrentPartTicksLeft > 0) return;
			    OutputBuffer.MakePart(CurrentPart, InputBuffer);
			    Log("Made part {0}", CurrentPart);
			    CurrentPart = null;
			}
		}
		
		public override string ToString()
		{
			return _name;
		} 
		
		#region IXmlSerializable implementation

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		public void WriteXml(XmlWriter writer)
		{		
			WriteXml(this, writer);
		}
		
		public static void WriteXml(Block b, XmlWriter writer)
		{
			writer.WriteStartElement("Block");
			
			writer.WriteAttributeString("Name", b.Name);
			
			writer.WriteStartElement("ProducesParts");
			
			foreach (var part in b.ProducesParts) 
			{
				writer.WriteStartElement("Part");
				writer.WriteAttributeString("Name", part.Name);
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
		#endregion
		
	}
	
}
