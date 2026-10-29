Shader "UI/Glow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowRangeUV ("Glow Range UV", Float) = 0.1
        _AlphaStart ("Alpha Start", Range(0,1)) = 1
        _AlphaEnd ("Alpha End", Range(0,1)) = 0
        _TransitionMid ("Transition Mid", Range(0.01,1)) = 0.5
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _GlowColor;
            float _GlowRangeUV;
            float _AlphaStart;
            float _AlphaEnd;
            float _TransitionMid;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float insideX = step(0, uv.x) * step(uv.x, 1);
                float insideY = step(0, uv.y) * step(uv.y, 1);
                float inside = insideX * insideY;

                float2 glowUV = saturate(uv);
                fixed4 mainColor = tex2D(_MainTex, glowUV);

                float2 edgeDist2 = float2(
                    min(abs(uv.x), abs(uv.x - 1)),
                    min(abs(uv.y), abs(uv.y - 1))
                );
                float edgeDist = max(edgeDist2.x, edgeDist2.y);

                float glowAlpha;
                if (inside > 0.5)
                {
                    glowAlpha = lerp(_AlphaStart, 1.0, mainColor.a);
                }
                else
                {
                    float maxGlow = max(_GlowRangeUV, 0.001);
                    float normDist = saturate(edgeDist / maxGlow);
                    float curve = normDist / max(_TransitionMid, 0.001);
                    curve = saturate(curve);
                    glowAlpha = lerp(_AlphaStart, _AlphaEnd, curve);
                }

                fixed3 glowRGB = _GlowColor.rgb;
                fixed4 result;
                result.rgb = lerp(glowRGB, mainColor.rgb, inside);
                result.a = glowAlpha * _GlowColor.a;
                result *= i.color;

                return result;
            }
            ENDCG
        }
    }
}