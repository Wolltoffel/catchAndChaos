Shader "CustomOutline/OutlineMask_"
{
    Properties
    {
        
    }


    SubShader
    {
        Pass
        {
            Tags {"RenderType" = "Transparent"
            "Queue" = "Transparent+100"}
            LOD 100
            Cull Off
            ZTest Always
            ColorMask 0
            ZWrite Off


            Stencil 
            {
                Ref 1
                Pass Replace
            }
    
        }
    }

  
}
