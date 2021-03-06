﻿using UGUIDots.Render;
using UGUIDots.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace UGUIDots.Sample.Systems {

    [UpdateInGroup(typeof(UpdateMaterialGroup))]
    public class PingPongUISystem : SystemBase {

        protected override void OnUpdate() {
            var ltws         = GetComponentDataFromEntity<LocalToWorld>(true);
            var parents      = GetComponentDataFromEntity<Parent>(true);

            Entities.ForEach((Entity e, ref PingPongPositions c0, ref Translation c2, ref LocalToParent c3, 
                in MaterialPropertyIndex c1) => {
                var x = c2.Value.x;

                if (math.distance(x, c0.Target) <= 0.2f) {
                    c0.Target = c0.AdjustedWidth;
                }

                if (math.distance(x, c0.Target) <= 0.2f) {
                    c0.Target = -c0.AdjustedWidth;
                }

                x = math.lerp(x, c0.Target, Time.DeltaTime);

                var matrix = float4x4.TRS(new float3(x, 0, 0), quaternion.identity, new float3(1));
                var localSpace = new LocalToParent { Value = matrix };

                var root     = HierarchyUtils.GetRoot(e, parents);
                var batch    = EntityManager.GetComponentData<MaterialPropertyBatch>(root);
                var property = batch.Value[c1.Value];
                property.SetVector(ShaderIDConstants.Translation, new float4(localSpace.Position, 0));

                c3 = localSpace;
                c2 = new Translation { Value = localSpace.Position };
            }).WithoutBurst().Run();
        }
    }
}
