﻿using System.Linq;
using System.Collections.Generic;

namespace ProductionSim
{
    public interface ILogger
    {
        void Log(string message, object parent, params object[] args);
    }

    public interface IElement
    {
        string Name { get; }
    }
	    
    public interface IState 
    {
        int Ticks { get; }
        int IdleTicks { get; }
        
        void Tick();
        void ResetState();
    }
    
    public interface IBlockProgram
    {
    	IPart NextPart { get; }
    	IPart TakePart();
    }

    public interface IPartSequence : IBlockProgram, IList<PartSequenceStep>
    {
        PartSequenceStep NextStep { get; }
        PartSequenceStep CurrentStep { get; }
        int PartsLeft { get; }

        IEnumerable<IPart> Parts { get; }
    }
	
    public interface IInputBlock : IState, IElement
    {
    	IEnumerable<IPart> UsesParts { get; }
    	IBuffer InputBuffer { get; }
    	IEnumerable<IPart> StockParts { get; }
    }
    
    public interface IOutputBlock : IState, IElement
    {
    	IEnumerable<IPart> ProducesParts { get; }
    	IBuffer OutputBuffer { get; }
    	IPart ProducedPart { get; }
    	IBlockProgram BlockProgram { get; }        
    }
    
    public interface IBlock : IInputBlock, IOutputBlock {  }

    public interface IPart : IElement
    {
        IEnumerable<IPart> MadeFrom { get; }
        int ManufactureTime { get; }
    }

    public interface IBuffer : IElement, ICollection<IPart>
    {
        int Capacity { get; }
        bool Full { get; }
    }

    public interface ISimulation : IState
    {
    	IEnumerable<IBuffer> Buffers { get; }
    	IEnumerable<IBlock> Blocks { get; }
    	IEnumerable<IPart> Parts { get; }
    }
}