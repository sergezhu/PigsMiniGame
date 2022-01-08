using System.Collections.Generic;
using UnityEngine;

namespace Core.Extensions
{
    public static class TransformExtensions
    {
        public static void DestroyImmediateAllChildren(this Transform root, bool ignoreSelf = true)
        {
            var children = root.GetComponentsInChildren<Transform>();

            for (var index = children.Length - 1; index >= 0; index--)
            {
                var child = children[index];
                if (child == root && ignoreSelf)
                    continue;

                Object.DestroyImmediate(child.gameObject, true);
            }
        }
        
        public static void DestroyAllChildren(this Transform root)
        {
            var children = root.GetComponentsInChildren<Transform>();

            foreach (var child in children)
            {
                if(child == root)
                    continue;
                
                Object.Destroy(child.gameObject);
            }
        }
        
        public static void DestroyImmediateAllChildren(this Transform root, List<Transform> specifiedChildren, bool ignoreSelf = true)
        {
            var children = root.GetComponentsInChildren<Transform>();

            foreach (var child in children)
            {
                if(child == root && ignoreSelf)
                    continue;
                
                if(specifiedChildren.Contains(child) == false)
                    continue;
                
                Object.DestroyImmediate(child.gameObject, true);
            }
        }
    }
}