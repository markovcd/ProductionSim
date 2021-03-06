﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace ProductionSim
{
	public class Block : Loggable, IBlock
	{
	    private readonly ISet<IPart> _producesParts, _usesParts;
	    
	    public string Name { get; }

	    public IEnumerable<IPart> ProducesParts => _producesParts;
	    public IEnumerable<IPart> UsesParts => _usesParts;

	    public IBuffer InputBuffer { get; }
	    public IBuffer OutputBuffer { get; }

	    public int Ticks { get; private set; }
	    public int IdleTicks { get; private set; }
	    public int CurrentPartTicksLeft { get; protected set; }
	    
	    public IPart ProducedPart { get; private set; }
	    public IPartSequence PartsToMake { get; }

        public IEnumerable<IPart> StockParts { get; private set; }

	       
   
        public Block(string name, IEnumerable<IPart> producesParts, IBuffer inputBuffer, IBuffer outputBuffer, IEnumerable<PartSequenceStep> partsToMake = null, ILogger logger = null)
		{
		    Name = name;
		    _producesParts = producesParts.ToHashSet();
            _usesParts = _producesParts.SelectMany(p => p.MadeFrom).ToHashSet();
            InputBuffer = inputBuffer;
            OutputBuffer = outputBuffer;

		    PartsToMake = new PartSequence(partsToMake);

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
		    
            if (ProducedPart == null && PartsToMake.NextPart == null)
			{
				IdleTicks++;
			    Log("Idle ticks {0}.", IdleTicks);
			}
			else if (ProducedPart == null && PartsToMake.NextPart != null)
			{
				if (CanMakePart(PartsToMake.NextPart))
				{
					ProducedPart = PartsToMake.TakePart();
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
