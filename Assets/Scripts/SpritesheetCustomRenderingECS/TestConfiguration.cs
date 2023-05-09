using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace SCR1
{
    public class TestConfiguration : MonoBehaviour
    {
        public Texture2D[] spriteSheets;
    }

    public class RuntimeConfiguration : IComponentData
    {
        
    }

    public struct SpritesheetData
    {
        public Texture2D Texture;
        public float4[] uv;
        public float2[] pivot;
    }
    
    public class SpritesheetDefinition : IComponentData
    {
        public SpritesheetData[] Data;
    }
    
    public class TestConfigurationBaker : Baker<TestConfiguration>
    {
        public override void Bake(TestConfiguration conf)
        {
            var e = GetEntity(TransformUsageFlags.Dynamic);
            var sDef = new SpritesheetDefinition
            {
                Data = new SpritesheetData[conf.spriteSheets.Length]
            };
            for (var i = 0; i < conf.spriteSheets.Length; i++)
            {
                var texture = conf.spriteSheets[i];
                var sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture))
                    .OfType<Sprite>().ToArray();
                Debug.Log($"{sprites.Length}");
                sDef.Data[i] = new SpritesheetData()
                {
                    Texture = conf.spriteSheets[i],
                    uv = new float4[sprites.Length],
                    pivot = new float2[sprites.Length]
                };
                for (var sliceIndex = 0; sliceIndex < sprites.Length; sliceIndex++)
                {
                    var sprite = sprites[sliceIndex];
                    float2 uv0 = sprite.uv[1] - sprite.uv[2]; // uv[2] should contain the texcoord with largest x,y
                    float2 uv1 = sprite.uv[2];  // uv[2] should contain the texcoord with smallest x,y

                    sDef.Data[i].uv[sliceIndex] = new float4(uv0, uv1);
                    sDef.Data[i].pivot[sliceIndex] = sprite.pivot / sprite.pixelsPerUnit;
                }                
            }
            
            AddComponentObject(e, sDef);
        }
    }

}