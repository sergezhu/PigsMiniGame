using System;

namespace Core
{
    public interface IEarnScoresProvider
    {
        public event Action<int> EarnScoresReady;
    }
}