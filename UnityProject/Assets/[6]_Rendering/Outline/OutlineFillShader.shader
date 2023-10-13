Shader "CustomOutline/OutlineFillShader"
{
    Properties
    {
        _Outline ("Outline", Range (0,20)) = 0.0003
        _OutlineColor ("Outline Color", Color) = (0,0,0,0)
        _EmissionIntensity("Emission Intensity",float) = 1
        _cutoffTex ("Cuttoff Alpha Tex",2D) = "white" {}
        [Toggle(ActivateCutoffMap)]
        _activeAlphaMap ("Activate Cutoff Map", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _zTest("Z Test", Float) = 0
    }

    SubShader
    {

        Name "OutlineFill"
        Tags { "RenderType"="Transparent"
        "Queue" = "Transparent"}
        LOD 100
        ZTest [_zTest]
        Cull Off

        
        Stencil{
            Ref 1
            Comp NotEqual
        }

        Pass
        {   

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct v2f
            {
                float2 uv: TEXCOORD0;
                float4 vertex :SV_POSITION;
            };

            float _Outline;
            float4 _OutlineColor;

            sampler2D _cutoffTex;
            float4 _cutoffTex_ST;

            v2f vert (float4 position : POSITION, float3 normal: TEXCOORD5,float2 uv :TEXCOORD0)
            {
                v2f o;
                
                float3 viewPos = UnityObjectToViewPos(position);
                float4 clipPos = UnityObjectToClipPos(position);
                float3 viewNormal = mul(UNITY_MATRIX_IT_MV,normal);
                viewNormal = normalize(viewNormal);
                
                o.vertex = UnityViewToClipPos(viewPos+viewNormal*_Outline/100);
                o.uv = TRANSFORM_TEX (uv,_cutoffTex);

                return  o;
            }

            /*float4 vert (float4 position : POSITION, float3 normal: TEXCOORD3) : SV_POSITION
            {
                float4 clipPos = UnityObjectToClipPos(position);
                float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, normal));
                float2 offset = normalize (clipNormal.xy)/_ScreenParams.xy*_Outline*clipPos.w*2;

                float3 viewPos = UnityObjectToViewPos(position);
                float3 viewNormal = mul(UNITY_MATRIX_IT_MV,normal);
                viewNormal = normalize(viewNormal);

                //clipPos.xy+= offset;
                clipPos = UnityViewToClipPos(viewPos+viewNormal*_Outline);

                return clipPos;
            }*/

            half _activeAlphaMap;
            float _EmissionIntensity;

            fixed4 frag (v2f i)  : SV_Target
            {
                fixed4 color = _OutlineColor;
                fixed3 emission  = 50*_EmissionIntensity*color.rgb;
                if (_activeAlphaMap>0)
                {
                    half cutoffValue = tex2D(_cutoffTex,i.uv).r;
                    clip(color.a-cutoffValue);
                }
                return fixed4(color.rgb+emission,color.a);

            }

            ENDCG
        }
    }

  
}
