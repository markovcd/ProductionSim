using System;
using System.Linq;
using System.Collections.Generic;

namespace ProductionSim
{
	public class GeneratorBlock : Loggable, IOutputBlock
	{
	    private readonly IEnumerable<IPart> _producesParts;
	    private readonly string _name;
	    private readonly IBuffer _outputBuffer;
	    private readonly IBlockProgram _blockProgram;

	    public GeneratorBlock(string name, IEnumerable<IPart> producesParts, IBuffer outputBuffer, IBlockProgram blockProgram, ILogger logger = null)
			: base(logger)
		{
			_name = name;
			_producesParts = producesParts;
			_outputBuffer = outputBuffer;
			_blockProgram = blockProgram;
		}

	    public string Name { get { return _name; } }
	    public int Ticks { get { throw new NotImplementedException(); } }
	    public int IdleTicks { get { throw new NotImplementedException(); } }
	    public IEnumerable<IPart> ProducesParts { get { return _producesParts; } }
	    public IBuffer OutputBuffer { get { return _outputBuffer; } }

	    public IPart ProducedPart { get; private set; }

	    public IBlockProgram BlockProgram { get { return _blockProgram; } }

        public void Tick()
		{
			throw new NotImplementedException();
		}
	
		public void ResetState()
		{
			throw new NotImplementedException();
		}
		
	}
	
	
	public class EaterBlock : Loggable, IInputBlock
	{
		private readonly string _name;
		private readonly IEnumerable<IPart> _usesParts;
		private readonly IBuffer _inputBuffer;
		
		public EaterBlock(string name, IEnumerable<IPart> usesParts, IBuffer inputBuffer, ILogger logger = null) 
			: base(logger)
		{
			_name = name;
			_usesParts = usesParts;
			_inputBuffer = inputBuffer;
		}

	    public string Name { get { return _name; } }
        public int Ticks { get; private set; }
	    public int IdleTicks { get; private set; }


	    public IEnumerable<IPart> UsesParts { get { return _usesParts; } }
	    public IBuffer InputBuffer { get { return _inputBuffer; } }
	    public IEnumerable<IPart> StockParts { get { throw new NotImplementedException(); } }

        public void Tick()
		{
			throw new NotImplementedException();
		}
		public void ResetState()
		{
			throw new NotImplementedException();
		}
		
		
		
		

		
	}
	
	public class Block : Loggable, IBlock
	{
	    private readonly ISet<IPart> _producesParts, _usesParts;
	    private readonly string _name;
	    private readonly IBuffer _inputBuffer, _outputBuffer;
	    private readonly IBlockProgram _blockProgram;
	   
        public Block(string name, IEnumerable<IPart> producesParts, IBuffer inputBuffer, IBuffer outputBuffer, IBlockProgram blockProgram, ILogger logger = null)
        	: base(logger)
		{
		    _name = name;
		    _producesParts = producesParts.ToHashSet();
            _usesParts = _producesParts.SelectMany(p => p.MadeFrom).ToHashSet();
            _inputBuffer = inputBuffer;
            _outputBuffer = outputBuffer;

		    _blockProgram = blockProgram;

            IdleTicks = 0;
		    Ticks = 0;
		    
		    Log("Created.");
		}

	    public string Name { get { return _name; } }

	    public IEnumerable<IPart> ProducesParts { get { return _producesParts; } }
	    public IEnumerable<IPart> UsesParts { get { return _usesParts; } }

	    public IBuffer InputBuffer { get { return _inputBuffer; } }
	    public IBuffer OutputBuffer { get { return _outputBuffer; } }

	    public int Ticks { get; private set; }
	    public int IdleTicks { get; private set; }
	    public int CurrentPartTicksLeft { get; protected set; }

	    public IPart ProducedPart { get; private set; }
	    public IBlockProgram BlockProgram { get { return _blockProgram; } }

	    public IEnumerable<IPart> StockParts { get; private set; }

        private void ThrowInvalidOperationException(string s, params object[] args)
        {
        	var msg = string.Format(s, args);
        	Log(msg);
        	throw new InvalidOperationException(msg);
        }

	    private bool CanMakePart(IPart part)
        {
        	if (!ProducesParts.Contains(part)) return false;
        	var inputBuffer = InputBuffer.ToList();
            return part.MadeFrom.All(inputBuffer.Remove);
        }

	    public void ResetState()
	    {
	        IdleTicks = 0;
	        CurrentPartTicksLeft = 0;
            ProducedPart = null;
	        
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
		    
            if (ProducedPart == null && BlockProgram.NextPart == null)
			{
				IdleTicks++;
			    Log("Idle ticks {0}.", IdleTicks);
			}
			else if (ProducedPart == null && BlockProgram.NextPart != null)
			{
				if (CanMakePart(BlockProgram.NextPart))
				{
					ProducedPart = BlockProgram.TakePart();
					CurrentPartTicksLeft = ProducedPart.ManufactureTime;
					StockParts = TakeStockParts(ProducedPart).ToList();
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
			    	ProducedPart = null;
			    	StockParts = null;
			    	Log("Made part {0}", ProducedPart);
			    }
			}
		}
		
		public override string ToString()
		{
			return Name;
		} 
		
		
		
	}
	
}
