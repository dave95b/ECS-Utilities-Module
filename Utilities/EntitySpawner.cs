using Unity.Entities;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Jobs;

namespace ECS.Utilities
{
    public class EntitySpawner
    {
        private EntitySpawnerQuery spawnerQuery;

        public EntitySpawner(ComponentSystemBase system)
        {
            Assert.IsFalse(system is null);
            spawnerQuery = new EntitySpawnerQuery(system);
        }


        public EntitySpawnerQuery Spawn(in Entity prefab, int count, Allocator allocator)
        {
            Assert.IsTrue(count > 0);
            return spawnerQuery.Spawn(prefab, count, allocator);
        }

        public void Dispose()
        {
            spawnerQuery.Dispose();
        }
    }

    public class EntitySpawnerQuery
    {
        private readonly ComponentSystemBase system;
        private EntityManager EntityManager => system.EntityManager;

        private NativeArray<Entity> entities;
        private NativeList<JobHandle> handles;
        private EntityQuery spawnedQuery;

        public EntitySpawnerQuery(ComponentSystemBase system)
        {
            this.system = system;
            handles = new NativeList<JobHandle>(Allocator.Persistent);
            spawnedQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<JustSpawnedTag>());
        }

        public EntitySpawnerQuery Spawn(in Entity prefab, int count, Allocator allocator)
        {
            bool hasSpawnedTag = EntityManager.HasComponent<JustSpawnedTag>(prefab);
            if (!hasSpawnedTag)
                EntityManager.AddComponent<JustSpawnedTag>(prefab);

            entities = new NativeArray<Entity>(count, allocator, NativeArrayOptions.UninitializedMemory);
            EntityManager.Instantiate(prefab, entities);

            if (!hasSpawnedTag)
                EntityManager.RemoveComponent<JustSpawnedTag>(prefab);

            return this;
        }
        public EntitySpawnerQuery Spawn(in EntityArchetype archetype, int count, Allocator allocator)
        {
            entities = new NativeArray<Entity>(count, allocator, NativeArrayOptions.UninitializedMemory);
            EntityManager.CreateEntity(archetype, entities);
            EntityManager.AddComponent(entities, ComponentType.ReadWrite<JustSpawnedTag>());

            return this;
        }

        public EntitySpawnerQuery AddData<T>(in T data) where T : struct, IComponentData
        {
            EntityManager.AddComponent<T>(entities);
            SetData(data);
            return this;
        }

        public EntitySpawnerQuery AddData<T>(in NativeArray<T> data) where T : struct, IComponentData
        {
            EntityManager.AddComponent<T>(entities);
            SetData(data);
            return this;
        }

        public EntitySpawnerQuery SetData<T>(in T data) where T : struct, IComponentData
        {
            var handle = system.SetComponentData(spawnedQuery, data);
            handles.Add(handle);
            return this;
        }

        public EntitySpawnerQuery SetData<T>(in NativeArray<T> data) where T : struct, IComponentData
        {
            var handle = system.SetComponentData(spawnedQuery, data);
            handles.Add(handle);
            return this;
        }

        public NativeArray<Entity> Complete()
        {
            JobHandle.CompleteAll(handles);
            EntityManager.RemoveComponent(spawnedQuery, ComponentType.ReadWrite<JustSpawnedTag>());
            return entities;
        }

        public void Dispose()
        {
            handles.Dispose();
            spawnedQuery.Dispose();
        }
    }
}