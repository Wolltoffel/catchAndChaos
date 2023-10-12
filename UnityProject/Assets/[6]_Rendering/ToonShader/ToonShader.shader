Shader "Custom/ToonShader"
{
    Properties
    {
        _BaseMap("AlbedoMap", 2D) = "white"{}
        _BaseColor ("Color", Color) = (0,0,0,0)
        
        _ShadowColor_1 ("_ShadowColor", Color) = (0,0,0,0)
        _ShadowColor_2 ("_ShadowColor", Color) = (0,0,0,0)
        _ShadowColor_3 ("_ShadowColor", Color) = (0,0,0,0)

        _ShadowThresh_1("_Shadow 1 Thresh",float) = 0.4
        _ShadowThresh_2("_Shadow 2 Thresh",float) = 0.6
        _ShadowThresh_3("_Shadow 3 Thresh",float) = 0.8
        [HDR] _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        _Glossines("Glossines",Float) = 32

    }
    SubShader
    {       
        LOD 100

            Pass    
            {
                Name "ToonShading"
                Tags { 
                "RenderType"="Opaque" 
                "PassFlags" = "OnlyDirectional" 
                "LightMode" = "UniversalForward" 
                "RenderPipeline"="UniversalPipeline"
                }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
    

                #include "UnityCG.cginc"
                #include "Lighting.cginc"
                #include "AutoLight.cginc"



                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal:NORMAL;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldNormal:NORMAL;
                    float3 viewDir: TEXCOORD1;
                    float shadowCoord: TEXCOORD2;
                };

                sampler2D _BaseMap;
                float4 _BaseMap_ST;

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.viewDir = WorldSpaceViewDir (v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv,_BaseMap);
                    return o;
                }

                float4 _BaseColor;
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

                fixed4 frag (v2f i) : SV_Target
                {   
                    //Light mainLight = GetMainLight(input.shadowCoord);
                    float shadow = SHADOW_ATTENUATION(i);
    
                    //Hard Shadows
                    float3 normal = normalize(i.worldNormal);
                    fixed4 col = fixed4(_BaseColor.rgb,_BaseColor.a);
                    float NormalDotLight = dot (_WorldSpaceLightPos0,normal);
                    

                    //Highlight
                    float4 lightIntensity = smoothstep (0,0.01,shadow);
                    float4 light = lightIntensity;
                    float3 viewDir = normalize(i.viewDir);
                    float3 halfVector = normalize(_WorldSpaceLightPos0+viewDir);
                    float normalDotHalf = dot(normal,halfVector);
                    float specularIntensity = pow(normalDotHalf*lightIntensity,_Glossines*_Glossines*100);
                    float specularIntensitySmooth = smoothstep(0.005,0.01,specularIntensity);
                    float4 specular = specularIntensitySmooth*_SpecularColor;
                    

                    //Rim Light
                    float rimDot = 1-dot(viewDir,normal);
                    float rimIntensity = rimDot*NormalDotLight;
                    rimIntensity = smoothstep(_RimAmount-0.01,_RimAmount+0.1,rimIntensity);
                    float4 rim = rimIntensity*_RimColor;
                    
                    //Base Color Tex
                    float4 baseColorTex = tex2D(_BaseMap,i.uv);

                    col=baseColorTex*_BaseColor*(_BaseColor+specular+rim);

                    float4 shadowColor;
                    if (NormalDotLight<_ShadowThresh_1)
                        col *= _ShadowColor_1;
                    else if (NormalDotLight<_ShadowThresh_2)
                        col *= _ShadowColor_2;
                    else if (NormalDotLight<_ShadowThresh_3)
                        col *= _ShadowColor_3;

                    col.a *=shadow;
                    return col;
                }
                ENDHLSL
            }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

    }
    FallBack "VertexLit"
}
