Shader "Custom/XRayShader_Unlit"
{
    Properties
    {
        _Color ("Color", Color) = (1,0,0,1)
        _XRayColor ("Color", Color) = (1,0,0,1)

        _Albedo ("Base Texture", 2D) = "white" {}
    }
    SubShader
    {
        LOD 100
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On

             Stencil
            {
                Ref 1        // Set the reference value for the stencil test
                Comp Greater   // Pass if the stencil value is equal to the reference value
                Pass Replace // Replace the stencil value with the reference value
            }

 
            HLSLPROGRAM
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON

            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;

            }; 

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 texCoord : TEXCOORD2;

            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.worldPos = TransformObjectToWorld(v.vertex);
                o.normal = v.normal;
                o.texCoord = v.texcoord;
                return o;
            }

            float4 _Color;
            
            half3 LightingLambertWithMaterialColor(half3 lightColor, half3 materialColor)
            {
                 return lightColor  * materialColor;
            }
             
            
            float3 Lambert(float3 lightColor, float3 lightDir, float3 normal)
            {
                float NdotL = saturate(dot(normal, lightDir)); // <<< THIS ONE RIGHT HERE
                return lightColor * NdotL;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = _Color;
                
                float3 lightPos = _MainLightPosition.xyz;
                float3 lightCol = LightingLambertWithMaterialColor(_MainLightColor, _Color);
                                
                uint lightsCount = GetAdditionalLightsCount();
                for (int j = 0; j < lightsCount; j++)
                {
                    Light light = GetAdditionalLight(j, i.worldPos);
                    lightCol += LightingLambertWithMaterialColor(light.color * (light.distanceAttenuation * light.shadowAttenuation), _Color);
                }
                color.rgb *= lightCol;
                return color;
            } 
            ENDHLSL
        }

        Pass
        {   
            Name "Silhouette"
             Tags {"Queue" = "Transparent+1" }

            
            ZWrite Off
            ZTest Always
            Cull Off

             Stencil
            {
                Ref 1        // Set the reference value for the stencil test
                Comp NotEqual // Pass if the stencil value is not equal to the reference value
            }
 
            HLSLPROGRAM

            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;

            }; 

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 texCoord : TEXCOORD2;

            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.worldPos = TransformObjectToWorld(v.vertex);
                o.normal = v.normal;
                o.texCoord = v.texcoord;
                return o;
            }

            float4 _XRayColor;

            float4 frag (v2f i) : SV_Target
            {
                float4 color = _XRayColor;
                color.rgb = _XRayColor;
                return color;
            } 
            ENDHLSL
        }              
    }
}