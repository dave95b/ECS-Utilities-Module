using UnityEngine;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace ECS.Utilities
{

    [BurstCompile]
    unsafe struct SetComponentDataFromArrayJob<T> : IJobChunk where T : struct, IComponentData
    {
        [ReadOnly]
        public NativeArray<T> ComponentData;
        public ArchetypeChunkComponentType<T> ComponentType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityOffset)
        {
            void* destinationPtr = chunk.GetNativeArray(ComponentType).GetUnsafePtr();
            byte* srcPtr = (byte*)ComponentData.GetUnsafeReadOnlyPtr() + UnsafeUtility.SizeOf<T>() * entityOffset;
            int copySizeInBytes = UnsafeUtility.SizeOf<T>() * chunk.Count;

            UnsafeUtility.MemCpy(destinationPtr, srcPtr, copySizeInBytes);
        }
    }

    [BurstCompile]
    unsafe struct SetComponentDataFromValueJob<T> : IJobChunk where T : struct, IComponentData
    {
        [ReadOnly]
        public T ComponentData;
        public ArchetypeChunkComponentType<T> ComponentType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityOffset)
        {
            void* destinationPtr = chunk.GetNativeArray(ComponentType).GetUnsafePtr();
            int size = UnsafeUtility.SizeOf<T>();
            void* sourcePtr = stackalloc byte[size];
            UnsafeUtility.CopyStructureToPtr(ref ComponentData, sourcePtr);

            UnsafeUtility.MemCpyReplicate(destinationPtr, sourcePtr, size, chunk.Count);
        }
    }
}