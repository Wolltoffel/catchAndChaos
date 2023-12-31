Shader "Custom/test2"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5
 
            // BlendMode
            [HideInInspector] _Surface("__surface", Float) = 0.0
            [HideInInspector] _Blend("__blend", Float) = 0.0
            [HideInInspector] _AlphaClip("__clip", Float) = 0.0
            [HideInInspector] _SrcBlend("__src", Float) = 1.0
            [HideInInspector] _DstBlend("__dst", Float) = 0.0
            [HideInInspector] _ZWrite("__zw", Float) = 1.0
            [HideInInspector] _Cull("__cull", Float) = 2.0
 
            // Editmode props
            [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
 
            // ObsoleteProperties
            [HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
            [HideInInspector] _Color("Base Color", Color) = (0.5, 0.5, 0.5, 1)
            [HideInInspector] _SampleGI("SampleGI", float) = 0.0 // needed from bakedlit
    }
        SubShader
            {
                Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
                LOD 300
 
                Pass
                {
               
                Name "Unlit"
                Tags { "LightMode" = "UniversalForward" }
 
                // Use same blending / depth states as Standard shader
                Blend[_SrcBlend][_DstBlend]
                ZWrite[_ZWrite]
                Cull[_Cull]
 
                HLSLPROGRAM
                // Required to compile gles 2.0 with standard srp library
                #pragma prefer_hlslcc gles
                #pragma exclude_renderers d3d11_9x
                #pragma target 2.0
 
                #pragma vertex vert
                #pragma fragment frag
                #pragma shader_feature _ALPHATEST_ON
                #pragma shader_feature _ALPHAPREMULTIPLY_ON
 
                // -------------------------------------
                // Unity defined keywords
                #pragma multi_compile_fog
                #pragma multi_compile_instancing

                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
 
                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


 
                struct Attributes
                {
                    float4 positionOS       : POSITION;
                    float2 uv               : TEXCOORD0;
                    float3 normalOS     : NORMAL;
                    float4 tangentOS    : TANGENT;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };
 
                struct Varyings
                {
                    float2 uv        : TEXCOORD0;
                    float fogCoord : TEXCOORD1;
                    float4 vertex : SV_POSITION;                
                    half3  normalWS : TEXCOORD2;
                    float4 shadowCoord : TEXCOORD3;
                    float4 positionOS: TEXCOORD4;
 
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    UNITY_VERTEX_OUTPUT_STEREO
                };

 
                Varyings vert(Attributes input)
                {
                    Varyings output = (Varyings)0;
 
                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_TRANSFER_INSTANCE_ID(input, output);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
 
                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    output.vertex = vertexInput.positionCS;
                    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                    output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                    output.shadowCoord = GetShadowCoord(vertexInput);
                    output.positionOS = input.positionOS;
                   
                    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                    output.normalWS = vertexNormalInput.normalWS;
 
                    return output;
                }
 
                half4 frag(Varyings input) : SV_Target
                {
                            
                    half2 uv = input.uv;
                    half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                    half3 color = texColor.rgb * _BaseColor.rgb;
                    half alpha = texColor.a * _BaseColor.a;


                    Light mainLight = GetMainLight(input.shadowCoord);

                    float3 shadowAttenuation=0;    
                    int kernelSize = 2;
                    float4 positionOS = input.positionOS;
                    float4 positionVarying;
                    for (int x = -kernelSize; x <= kernelSize; x++)
                    {
                        for (int y = -kernelSize; y <= kernelSize; y++)
                        {
                             for (int z = -kernelSize; z <= kernelSize; z++)
                             {
                                    float3 offset = float3(0, 0,0);
                                    VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS.xyz+offset);
                                    Light mainLight = GetMainLight(GetShadowCoord(vertexInput));
                                    shadowAttenuation += mainLight.shadowAttenuation;
                             }
                        }
                    }

                    shadowAttenuation /= ((2 * kernelSize + 1) * (2 * kernelSize + 1) * (2 * kernelSize + 1));


 
                    half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
                    half3 diffuseColor = LightingLambert(attenuatedLightColor, mainLight.direction, input.normalWS);            
 
                    //color *= mainLight.shadowAttenuation; //mainLight.shadowAttenuation;

                    //color.rbg =shadowAttenuation;

                    return half4(color, alpha);
                }


                

                ENDHLSL
            }
    
 
                
 
                UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
            }
                FallBack "Hidden/Universal Render Pipeline/FallbackError"
}