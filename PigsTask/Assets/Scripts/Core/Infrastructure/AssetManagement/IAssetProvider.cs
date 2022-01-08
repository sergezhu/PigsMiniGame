using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Infrastructure.AssetManagement
{
  public interface IAssetProvider
  {
    Task<GameObject> Instantiate(string path, Vector3 at);
    Task<GameObject> Instantiate(string path, Transform parent);
    Task<GameObject> Instantiate(string path, Vector3 at, Transform parent);
    Task<GameObject> Instantiate(string path);
    Task<T> Load<T>(AssetReference prefabReference) where T : class;
    void Cleanup();
    Task<T> Load<T>(string address) where T : class;
    void Initialize();
  }
}