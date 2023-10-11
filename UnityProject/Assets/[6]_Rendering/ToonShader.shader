Shader "Custom/ToonShader"
{
    Properties
    {
        _MainTex("AlbedoMap", 2D) = "white"{}
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
        LOD 100

        
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        Pass    
        {
            Name "ToonShading"
            Tags { 
            "RenderType"="Opaque" 
            "PassFlags" = "OnlyDirectional" 
            "LightMode" = "UniversalForward" 
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows 


            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent: TANGENT;
                float3 normal:NORMAL;
            };

            struct v2f
            {
                SHADOW_COORDS(1)
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal:NORMAL;
                float3 viewDir: TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir (v.vertex);
                o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                TRANSFER_SHADOW(o)
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

            fixed4 frag (v2f i) : SV_Target
            {   
                //Shadow
                float shadow = SHADOW_ATTENUATION(i);
   
                //Hard Shadows
                float3 normal = normalize(i.worldNormal);
                fixed4 col = fixed4(_Color.rgb,_Color.a);
                float NormalDotLight = dot (_WorldSpaceLightPos0,normal);
                

                //Highlight
                float4 lightIntensity = smoothstep (0,0.01,NormalDotLight*shadow);
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
                float4 baseColorTex = tex2D(_MainTex,i.uv);


                col=baseColorTex*_Color*(_Color+specular+rim);

                float4 shadowColor;
                if (NormalDotLight>_ShadowThresh_1)
                    col *= _ShadowColor_1;
                else if (NormalDotLight>_ShadowThresh_2)
                    col *= _ShadowColor_2;
                else if (NormalDotLight>_ShadowThresh_3)
                    col *= _ShadowColor_3;

                col.a *=shadow;
                return col;
            }
            ENDCG
            
        }


    }


    FallBack "VertexLit"
}
