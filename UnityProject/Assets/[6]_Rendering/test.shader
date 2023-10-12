Shader "Custom/ToonShaderReceiveShadows"
{
    Properties
    {
        _BaseMap("AlbedoMap", 2D) = "white"{}
        _Color ("Color", Color) = (0,0,0,0)
        
        _ShadowColor_1 ("_ShadowColor", Color) = (0,0,0,0)
        _ShadowColor_2 ("_ShadowColor", Color) = (0,0,0,0)
        _ShadowColor_3 ("_ShadowColor", Color) = (0,0,0,0)

        _ShadowThresh_1("_Shadow 1 Thresh",float) = 0.4
        _ShadowThresh_2("_Shadow 2 Thresh",float) = 0.6
        _ShadowThresh_3("_Shadow 3 Thresh",float) = 0.8
        [HDR] _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        _Glossines("Glossines",Float) = 32
        [HDR]_RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0,1)) = 0.716
    }
        SubShader
        {
                Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
                LOD 300
 
            Pass
            {
                Name "Unlit"
                Tags { "LightMode" = "UniversalForward" }
 
                HLSLPROGRAM
 
                #pragma vertex vert
                #pragma fragment frag
 
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
 
                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

 
                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 tangent: TANGENT;
                    float3 normal:NORMAL;
                };
    
                struct v2f
                {
                    float2 uv        : TEXCOORD0;
                    float4 vertex : SV_POSITION;                  
                    float4 shadowCoord : TEXCOORD2;
                    float3 worldNormal:NORMAL;
                    float3 viewDir: TEXCOORD1;
                };


                v2f vert(appdata input)
                {
                    v2f o;

                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
                    o.vertex = vertexInput.positionCS;
                    o.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normal, input.tangent);
                    o.worldNormal = vertexNormalInput.normalWS;
                    o.viewDir = TransformWorldToViewDir(input.vertex);
                    o.shadowCoord = GetShadowCoord(vertexInput);
                    return o;
                }


                float4 _Color;
                float _Glossines;
                float4 _SpecularColor;
                float4 _RimColor;
                float _RimAmount;

                float4 _ShadowColor_1;
                float4 _ShadowColor_2;
                float4 _ShadowColor_3;
                float _ShadowThresh_1;
                float _ShadowThresh_2;
                float _ShadowThresh_3;
 
                half4 frag(v2f i) : SV_Target
                {
                
                    half2 uv = i.uv;
                    half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);

                    Light mainLight = GetMainLight(i.shadowCoord);

                    
                    //Hard Shadows
                    float3 normal = normalize(i.worldNormal);
                    float4 col = float4(_Color.rgb,_Color.a);
                    float NormalDotLight = dot (mainLight.direction,normal);
                    

                    //Highlight
                    float4 lightIntensity = smoothstep (0,0.01,NormalDotLight);
                    float4 light = lightIntensity;
                    float3 viewDir = normalize(i.viewDir);
                    float3 halfVector = normalize(mainLight.direction+mainLight.direction);
                    float normalDotHalf = dot(normal,halfVector);
                    float specularIntensity = pow(normalDotHalf*lightIntensity,_Glossines*_Glossines*100);
                    float specularIntensitySmooth = smoothstep(0.005,0.01,specularIntensity);
                    float4 specular = specularIntensitySmooth*_SpecularColor;
                    

                    
                    half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
                    half3 diffuseColor = LightingLambert(attenuatedLightColor, mainLight.direction, i.worldNormal); 

                    col = _Color*texColor;
                    //col *=shadow;
                    if (diffuseColor.r<_ShadowThresh_1)
                        col *= _ShadowColor_1;
                    else if (diffuseColor.r<_ShadowThresh_2)
                        col *= _ShadowColor_2;
                    else if (diffuseColor.r<_ShadowThresh_3)
                        col *= _ShadowColor_3;
                    
                   //col.rgb = diffuseColor;

                    return col;
                }
                ENDHLSL
            }
            
 
                UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
            }
                FallBack "Hidden/Universal Render Pipeline/FallbackError"
}