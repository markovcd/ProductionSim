using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

    

    public interface IPartSequence : IList<PartSequenceStep>
    {
        IPart NextPart { get; }
        PartSequenceStep NextStep { get; }
        PartSequenceStep CurrentStep { get; }
        int PartsLeft { get; }

        IEnumerable<IPart> Parts { get; }
        IPart TakePart();

    }

    public interface IBlock : IState, IElement
    {
        IEnumerable<IPart> ProducesParts { get; }
        IEnumerable<IPart> UsesParts { get; }

        IPart ProducedPart { get; }
        IPart NextPart { get; }
        IEnumerable<IPart> StockParts { get; }
        IPartSequence PartsToMake { get; }
        IBuffer InputBuffer { get; }
        IBuffer OutputBuffer { get; }
    }

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