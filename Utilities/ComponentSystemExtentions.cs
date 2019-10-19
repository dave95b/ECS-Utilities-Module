using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace ECS.Utilities
{
    public static class ComponentSystemExtentions
    {
        public static JobHandle SetComponentData<T>(this ComponentSystemBase system, in EntityQuery query, in NativeArray<T> componentData, JobHandle inputDeps = default) where T : struct, IComponentData
        {
            var copyJob = new SetComponentDataFromArrayJob<T>
            {
                ComponentData = componentData,
                ComponentType = system.GetArchetypeChunkComponentType<T>(isReadOnly: false)
            };
            return copyJob.Schedule(query, inputDeps);
        }
        public static JobHandle SetComponentData<T>(this ComponentSystemBase system, in EntityQuery query, in T componentData, JobHandle inputDeps = default) where T : struct, IComponentData
        {
            var copyJob = new SetComponentDataFromValueJob<T>
            {
                ComponentData = componentData,
                ComponentType = system.GetArchetypeChunkComponentType<T>(isReadOnly: false)
            };
            return copyJob.Schedule(query, inputDeps);
        }
    }
}