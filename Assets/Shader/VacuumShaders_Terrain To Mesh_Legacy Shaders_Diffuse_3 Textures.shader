Shader "Custom/TerrainToMesh/3Textures"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)

        [NoScaleOffset] _V_T2M_Control ("Control Map (RGBA)", 2D) = "white" {}

        _V_T2M_Splat1 ("Layer 1 (R)", 2D) = "white" {}
        _V_T2M_Splat1_uvScale ("Layer1 UV Scale", Float) = 1

        _V_T2M_Splat2 ("Layer 2 (G)", 2D) = "white" {}
        _V_T2M_Splat2_uvScale ("Layer2 UV Scale", Float) = 1

        _V_T2M_Splat3 ("Layer 3 (B)", 2D) = "white" {}
        _V_T2M_Splat3_uvScale ("Layer3 UV Scale", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;

            sampler2D _V_T2M_Control;

            sampler2D _V_T2M_Splat1;
            sampler2D _V_T2M_Splat2;
            sampler2D _V_T2M_Splat3;

            float _V_T2M_Splat1_uvScale;
            float _V_T2M_Splat2_uvScale;
            float _V_T2M_Splat3_uvScale;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 control = tex2D(_V_T2M_Control, i.uv);

                float2 uv1 = i.uv * _V_T2M_Splat1_uvScale;
                float2 uv2 = i.uv * _V_T2M_Splat2_uvScale;
                float2 uv3 = i.uv * _V_T2M_Splat3_uvScale;

                float4 c1 = tex2D(_V_T2M_Splat1, uv1);
                float4 c2 = tex2D(_V_T2M_Splat2, uv2);
                float4 c3 = tex2D(_V_T2M_Splat3, uv3);

                float4 col =
                    c1 * control.r +
                    c2 * control.g +
                    c3 * control.b;

                return col * _Color;
            }

            ENDCG
        }
    }
}