using Core.Move;

namespace Core.View
{
    public interface IFourDirectionView
    {
        MoveDirection CurrentDirection { get; }
        void SetDirection(MoveDirection direction);
    }
}