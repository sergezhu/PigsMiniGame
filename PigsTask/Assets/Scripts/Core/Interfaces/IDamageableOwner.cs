using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IDamageableOwner
    {
        public IEnumerable<IDamageable> GetAllDamageable();
    }
}