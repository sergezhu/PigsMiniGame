namespace Core.Interfaces
{
    public interface IExplosionHandler
    {
        void HandleExplosion(GridCell cell, int distance);
    }
}