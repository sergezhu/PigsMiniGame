using System;

namespace Core.Interfaces
{
    public interface IEarnScoresProvider
    {
        public event Action<int> EarnScoresReady;
    }
}