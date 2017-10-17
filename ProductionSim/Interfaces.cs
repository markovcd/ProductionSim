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

        IPart CurrentPart { get; }
        IPart NextPart { get; set; }
        
        IBuffer InputBuffer { get; set; }
        IBuffer OutputBuffer { get; set; }
    }

    public interface IPart : IElement
    {
        IEnumerable<IPart> MadeFrom { get; }
        int ManufactureTime { get; }
    }

    public interface IBuffer : IElement, ICollection<IPart>
    {
        IEnumerable<IBlock> SourceBlocks { get; }
        IEnumerable<IBlock> TargetBlocks { get; }

        int Capacity { get; }
        bool Full { get; }

        void AddSourceBlock(IBlock block);
        void RemoveSourceBlock(IBlock block);

        void AddTargetBlock(IBlock block);
        void RemoveTargetBlock(IBlock block);

        bool CanMadePart(IPart part);
        void MakePart(IPart part, IBuffer input);
    }

    public interface ISimulation : IState
    {
        
    }
}