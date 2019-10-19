using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;

namespace ECS.Utilities
{
    public struct InstantiatedTag : IComponentData { }
    public struct JustSpawnedTag : IComponentData { }
}