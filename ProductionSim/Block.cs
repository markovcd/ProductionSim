using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ProductionSim
{

	public class Block : Loggable, IBlock
	{
		private readonly string _name;
		private readonly ISet<IPart> _producesParts, _usesParts;
	    private IPart _producedPart;
	    private IEnumerable<IPart> _stockParts;
	    private readonly IBuffer _inputBuffer, _outputBuffer;

		public string Name { get { return _name; } }
    
	    public IEnumerable<IPart> ProducesParts { get { return _producesParts; } }
	    public IEnumerable<IPart> UsesParts { get { return _usesParts; } }

	    public IBuffer InputBuffer { get { return _inputBuffer; } }
	    public IBuffer OutputBuffer { get { return _outputBuffer; } }
	    
	    public int Ticks { get; protected set; }
	    public int IdleTicks { get; protected set; }
	    public int CurrentPartTicksLeft { get; protected set; }
	    
	    public IPart ProducedPart { get { return _producedPart; } }
	    public IEnumerable<IPart> StockParts { get { return _stockParts; } }
			    
	    public IPart NextPart
	    {
	        get { return _producedPart; }
	        set
	        {
	            if (!ProducesParts.Contains(value)) ThrowInvalidOperationException("Block {0} doesn't produce part {1}.", this, value);
	            _producedPart = value;
	        }
	    }    
   
        public Block(string name, IEnumerable<IPart> producesParts, IBuffer inputBuffer, IBuffer outputBuffer, ILogger logger = null)
		{
		    _name = name;
		    _producesParts = producesParts.ToHashSet();
            _usesParts = producesParts.SelectMany(p => p.MadeFrom).ToHashSet();
            _inputBuffer = inputBuffer;
            _outputBuffer = outputBuffer;

			IdleTicks = 0;
		    Ticks = 0;
		    
		    Logger = logger;
		    
		    Log("Created.");
		}
        
        private void ThrowInvalidOperationException(string s, params object[] args)
        {
        	var msg = string.Format(s, args);
        	Log(msg);
        	throw new InvalidOperationException(msg);
        }
        
        public bool CanMakePart(IPart part)
        {
        	if (!ProducesParts.Contains(part)) return false;
        	var inputBuffer = InputBuffer.ToList();
            return part.MadeFrom.All(inputBuffer.Remove);
        }

	    public void ResetState()
	    {
	        IdleTicks = 0;
	        CurrentPartTicksLeft = 0;
            _producedPart = null;
	        NextPart = null;
	        
	        Log("Resetting state.");
	    }
	    
	    private IEnumerable<IPart> TakeStockParts(IPart producedPart)
	    {
	    	foreach (var part in producedPart.MadeFrom)
	    	{
	    		if (!InputBuffer.Remove(part)) ThrowInvalidOperationException("Can't make part {0}. Not all required input parts present in buffer {1}", ProducedPart, InputBuffer);
	    		yield return part;
	    	}
	    }
	    	

		public void Tick()
		{
		    
		    Ticks++;
			Log("Tick {0}.", Ticks);
		    
            if (ProducedPart == null && NextPart == null)
			{
				IdleTicks++;
			    Log("Idle ticks {0}.", IdleTicks);
			}
			else if (ProducedPart == null && NextPart != null)
			{
				if (CanMakePart(NextPart))
				{
					_producedPart = NextPart;
					NextPart = null;
					CurrentPartTicksLeft = ProducedPart.ManufactureTime;
					_stockParts = TakeStockParts(ProducedPart).ToList();
				    Log("Setting current part to {0}.", ProducedPart);
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
				
			    Log("Ticks left to make part {0} is {1}.", ProducedPart, CurrentPartTicksLeft);
			    
			    if (CurrentPartTicksLeft > 0) return;
			    
			    if (OutputBuffer.Full)
			    {
			    	IdleTicks++;
			    	Log("Idle ticks {0}.", IdleTicks);
			    }
			    else
			    {
			    	OutputBuffer.Add(ProducedPart);
			    	_producedPart = null;
			    	_stockParts = null;
			    	Log("Made part {0}", ProducedPart);
			    }
			}
		}
		
		public override string ToString()
		{
			return _name;
		} 
		
		
		
	}
	
}
