using System.Collections.Generic;

namespace ProductionSim
{
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

    public interface IBlock : IState, IElement
    {
        IEnumerable<IPart> ProducesParts { get; }
        IEnumerable<IPart> UsesParts { get; }

        IPart ProducedPart { get; }
        IPart NextPart { get; set; }
        IEnumerable<IPart> StockParts { get; }
        
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