using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.SceneManagement;
using ECS.Utilities;

namespace Gravity.ECS
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class DestroySystem : ComponentSystem
    {
        private EntityQuery instantiatedQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            SceneManager.sceneUnloaded += (scene) =>
            {
                if (scene.isSubScene)
                    return;

                EntityManager.DestroyEntity(instantiatedQuery);
            };

            instantiatedQuery = GetEntityQuery(ComponentType.ReadOnly<InstantiatedTag>());
            Enabled = false;
        }

        protected override void OnUpdate()
        {

        }
    }
}