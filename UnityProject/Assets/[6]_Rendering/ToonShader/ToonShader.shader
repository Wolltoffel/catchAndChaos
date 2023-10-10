Shader "Custom/ToonShader"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,0)
        [HDR] _AmbientColor ("AmbientColor", Color) = (0.4,0.4,0.4,1)
        _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        _Glossines("Glossines",Float) = 32
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
         Name "ToonShading"
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal:NORMAL;
                float3 viewDir: TEXCOORD1;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir (v.vertex);
                return o;
            }

            float4 _Color;
            float4 _AmbientColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(_Color.rgb,_Color.a);
                float NormalDotLight = dot (_WorldSpaceLightPos0,i.worldNormal);
                float4 lightIntensity = smoothstep (0,0.01,NormalDotLight);
                float4 light = lightIntensity+_LightColor0;
                col*=light+_AmbientColor;
                return col;
            }
            ENDCG
        }
    }
}
