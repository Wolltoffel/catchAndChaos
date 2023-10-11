Shader "Custom/ToonShader"
{
    Properties
    {
        _MainTex("AlbedoMap", 2D) = "white"{}
        _Color ("Color", Color) = (0,0,0,0)
        [HDR]_AmbientColor ("AmbientColor", Color) = (0.4,0.4,0.4,1)
        [HDR] _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        _Glossines("Glossines",Float) = 32
        [HDR]_RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0,1)) = 0.716

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
            "LightMode" = "UniversalForward" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
           // #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

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
                SHADOW_COORDS(2)
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

                TRANSFER_SHADOW(0)
                return o;
            }

            float4 _Color;
            float4 _AmbientColor;
            float _Glossines;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimAmount;

            fixed4 frag (v2f i) : SV_Target
            {   
                //Shadow
                float shadow = SHADOW_ATTENUATION(i);
   
                //Hard Shadows
                float3 normal = normalize(i.worldNormal);
                fixed4 col = fixed4(_Color.rgb,_Color.a);
                float NormalDotLight = dot (_WorldSpaceLightPos0,normal);
                float4 lightIntensity = smoothstep (0,0.01,NormalDotLight*shadow);
                float4 light = lightIntensity+_LightColor0;

                //Highlight
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

                col=baseColorTex*_Color*(light+_AmbientColor+specular+rim);
                return col;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
