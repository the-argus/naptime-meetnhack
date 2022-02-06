Shader "Unlit/Portal"
{
    Properties
    {
        // no properties necessary
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        // cull off to render the inside of the cube
        // thanks sebastian :)
        Cull off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                // thog dont care about uv
                //float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                // replace float2 uv with float4 screenpos
                float4 screenPos : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            //float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // instead of using TRANSFORM_TEX on v.uv to get uv, we do ComputeScreenPos
                o.screenPos = ComputeScreenPos(o.vertex)
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // uv based on screenposition, divide w so depth is disregarded
                // thanks again sebastian :)
                float2 uv = i.screenPos / i.screenPos.w;
                
                // can't we use float4?? idk and i dont want to find out
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
    Fallback "Standard"
}
