Shader "Quibli/Light Beam"
{
    Properties
    {
        [Header(Color alpha controls opacity)]
        [HDR]_Color ("Color", Color) = (0.5, 0.5, 0.5, 1.0)

        [Space(15)]
        _Depth ("Depth Fade Distance", Range(1.0, 500.0)) = 100.0

        [Space]
        _CameraDistanceFadeFar("Camera Distance Fade Far", Float) = 10.0
        _CameraDistanceFadeClose("Camera Distance Fade Close", Float) = 0.0

        [Space]
        _UvFadeX("UV Fade X", Range(0, 10)) = 0.1
        _UvFadeY("UV Fade Y", Range(0, 10)) = 0.1
        [ToggleOff]_AllowAlphaOverflow("Allow Alpha Overflow", Float) = 0.0
        
        [Space]
        _NoiseAmount("Noise Amount", Range(0, 1)) = 0.5
        _NoiseScale("Noise Scale", Range(0, 10)) = 1
        _NoiseSpeed("Noise Speed (XY)", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"
        }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ _ALLOWALPHAOVERFLOW_OFF
            #pragma multi_compile_fog;
            #pragma target 3.0

            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Depth;
            float _CameraDistanceFadeFar, _CameraDistanceFadeClose;
            float _UvFadeX, _UvFadeY;
            float _NoiseAmount, _NoiseScale;
            float2 _NoiseSpeed;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 dist :TEXCOORD3;
                float4 projPos : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float2 GradientNoise_Dir(float2 p) {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float GradientNoise(float2 UV, float Scale) {
                const float2 p = UV * Scale;
                const float2 ip = floor(p);
                float2 fp = frac(p);
                const float d00 = dot(GradientNoise_Dir(ip), fp);
                const float d01 = dot(GradientNoise_Dir(ip + float2(0, 1)), fp - float2(0, 1));
                const float d10 = dot(GradientNoise_Dir(ip + float2(1, 0)), fp - float2(1, 0));
                const float d11 = dot(GradientNoise_Dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }

            v2f vert(appdata i) {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = i.texcoord;
                UNITY_TRANSFER_FOG(o, o.vertex);
                o.dist = UnityObjectToViewPos(i.vertex);
                o.projPos = ComputeScreenPos(o.vertex);
                UNITY_TRANSFER_DEPTH(o.projPos);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                fixed4 c = _Color;
                const float scene_depth = LinearEyeDepth(
                    SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                const float object_depth = i.projPos.z + length(i.dist);
                float depth_fade = saturate((scene_depth - object_depth) / _Depth);
                depth_fade *= lerp(1.0, GradientNoise(i.dist.xy + _NoiseSpeed.xy * _Time.y, _NoiseScale), _NoiseAmount);
                c.a *= saturate(
                    (depth_fade * length(i.dist) - _CameraDistanceFadeClose) / (_CameraDistanceFadeFar -
                        _CameraDistanceFadeClose));

                const float fade_uv_x = pow(smoothstep(1, 0, abs(i.uv.x * 2 - 1)), _UvFadeX);
                const float fade_uv_y = pow(smoothstep(1, 0, abs(i.uv.y * 2 - 1)), _UvFadeY);
                c.a *= fade_uv_x * fade_uv_y;

                #ifdef _ALLOWALPHAOVERFLOW_OFF
                c.a = saturate(c.a);
                #endif

                UNITY_APPLY_FOG(i.fogCoord, c);

                return c;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}