Shader "Custom/oldRef"
{
    Properties
    {
         _Color ("_Color", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Name "ToonShading"
        Cull Off
        ZTest Always
        
         Stencil
        {
            Ref 1
            Comp NotEqual
        }

        Pass
        {   
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {       
                    float4 vertex : SV_POSITION;
                    float3 normal: NORMAL;
                    float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 worldNormal:NORMAL;
            };

            v2f vert(appdata input)
            {       v2f o;
                    o.worldNormal = UnityObjectToWorldNormal(input.normal);
                    return o;
            }

            float4 _Color;
            fixed4 frag(v2f input) : SV_Target
            {
                    float3 normal = normalize(input.worldNormal);
                    float NormalDotLight = dot (_WorldSpaceLightPos0,normal);
                    float4 color = _Color;
                    return fixed4(color.rgb,color.a);
            }
            ENDCG
        }
    }

  
}
