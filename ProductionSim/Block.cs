using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProductionSim
{

	public class Block : IBlock
	{
		private readonly string _name;
		private readonly ISet<IPart> _producesParts, _usesParts;
		private IBuffer _inputBuffer, _outputBuffer;
	    private IPart _nextPart;

		public string Name { get { return _name; } }
    
	    public IEnumerable<IPart> ProducesParts { get { return _producesParts; } }
	    public IEnumerable<IPart> UsesParts { get { return _usesParts; } }

	    public IPart CurrentPart { get; protected set; }

	    public IPart NextPart
	    {
	        get { return _nextPart; }
	        set
	        {
	            if (!ProducesParts.Contains(value)) throw new InvalidOperationException(string.Format("Block {0} doesn't produce part {1}.", this, value));
	            _nextPart = value;
	        }
	    }

	    public int IdleTicks { get; protected set; }
	    public int CurrentPartTicksLeft { get; protected set; }

        public IBuffer InputBuffer
	    {
	        get { return _inputBuffer; }
	        set
	        {
	            if (_inputBuffer != null) _inputBuffer.RemoveTargetBlock(this);
	            _inputBuffer = value;
                value.AddTargetBlock(this);
	        }
	    }

	    public IBuffer OutputBuffer
	    {
	        get { return _outputBuffer; }
	        set
	        {
	            if (_outputBuffer != null) _outputBuffer.RemoveSourceBlock(this);
	            _outputBuffer = value;
	            value.AddSourceBlock(this);
	        }
	    }
        
        public Block(string name, IEnumerable<IPart> producesParts, IBuffer inputBuffer = null, IBuffer outputBuffer = null)
		{
		    _name = name;
		    _producesParts = producesParts.ToHashSet();
            _usesParts = producesParts.SelectMany(p => p.MadeFrom).ToHashSet();

            InputBuffer = inputBuffer;
		    OutputBuffer = outputBuffer;

			IdleTicks = 0;

		    DebugMessage("Created.");
		}
		
		protected void DebugMessage(string message, params object[] args)
		{
		    var msg = string.Format("Block {0}: ", this) + string.Format(message, args);
            Debug.Print(msg);
            Console.WriteLine(msg);
		}

	    public void ResetState()
	    {
	        IdleTicks = 0;
	        CurrentPartTicksLeft = 0;
            CurrentPart = null;
	        NextPart = null;
	    }

		public void Tick()
		{
		    DebugMessage("Tick.");

            if (CurrentPart == null && NextPart == null)
			{
				IdleTicks++;
			    DebugMessage("Idle ticks increased to {0}.", IdleTicks);
			}
			else if (CurrentPart == null && NextPart != null)
			{
				if (InputBuffer.CanMadePart(NextPart))
				{
					CurrentPart = NextPart;
					NextPart = null;
					CurrentPartTicksLeft = CurrentPart.ManufactureTime;
				    DebugMessage("Setting current part to {0}.", CurrentPart);
				}
				else 
				{
					IdleTicks++;
				    DebugMessage("Idle ticks increased to {0}.", IdleTicks);
				}
			}
			else
			{
				CurrentPartTicksLeft--;
			    DebugMessage("Ticks left to make part {0} is {1}.", CurrentPart, CurrentPartTicksLeft);
			    if (CurrentPartTicksLeft > 0) return;
			    OutputBuffer.MakePart(CurrentPart, InputBuffer);
			    CurrentPart = null;
			}
		}
		
		public override string ToString()
		{
			return _name;
		} 
		
	}
	
}
