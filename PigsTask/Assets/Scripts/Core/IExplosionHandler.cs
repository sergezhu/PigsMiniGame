namespace Core
{
    public interface IExplosionHandler
    {
        void HandleExplosion(GridCell cell, int distance);
    }
}