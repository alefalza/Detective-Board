Shader "UI/LampBlackout"
{
    // Overlay negro con un "agujero" radial transparente centrado en un inspectable.
    // El color/alpha del overlay sale del Image.color (vertex color).
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Center   ("Center (UV pantalla)", Vector) = (0.266, 0.87, 0, 0)
        _Radius   ("Radius", Float) = 1.8
        _Softness ("Softness (proporción del radio)", Range(0, 1)) = 0.35
        _Dim      ("Dim (oscuridad dentro del círculo)", Range(0, 1)) = 0

        // Boilerplate UI (stencil / máscaras)
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

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv    : TEXCOORD0;
            };

            float4 _Center;
            float  _Radius;
            float  _Softness;
            float  _Dim;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.uv    = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Corrige aspect ratio para que el agujero sea un círculo y no un óvalo
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 delta = i.uv - _Center.xy;
                delta.x *= aspect;

                float dist = length(delta);

                // Borde proporcional al radio: se mantiene suave a cualquier tamaño.
                // El mínimo evita smoothstep(0,0) cuando el radio llega a 0.
                float soft = max(_Radius * _Softness, 0.0001);

                // 0 dentro del círculo (transparente), 1 fuera (oscuridad)
                float darkness = smoothstep(_Radius - soft, _Radius + soft, dist);

                // Segundo smoothstep: transición más gradual en los extremos del borde
                darkness = darkness * darkness * (3.0 - 2.0 * darkness);

                // Flicker: oscurece también el interior del círculo
                darkness = lerp(_Dim, 1.0, darkness);

                // Dithering para evitar banding en el degradado sobre fondo oscuro
                float noise = frac(sin(dot(i.pos.xy, float2(12.9898, 78.233))) * 43758.5453);
                darkness = saturate(darkness + (noise - 0.5) / 255.0);

                fixed4 col = i.color;
                col.a *= darkness;
                return col;
            }
            ENDCG
        }
    }
}
