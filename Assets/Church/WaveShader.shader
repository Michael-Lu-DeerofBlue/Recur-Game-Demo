Shader "Custom/WaveShader" {
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _WaveColor ("Wave Color", Color) = (0.9, 0.9, 0.9, 1.0)
        _Amplitude ("Amplitude", Float) = 0.4
        _Wavelength ("Wavelength", Float) = 3.0
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _AlphaClipThreshold ("Alpha Clip Threshold", Range(0.0, 1.0)) = 0.5
        _WaveDirection ("Wave Direction", Vector) = (2.0, 1.0, 0.0, 0.0)
        _StaticHeight ("Static Height", Float) = 0.5
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
       // _Metallic ("Metallic", Range(0,1)) = 0.0
        _Alpha ("Alpha", Range(0.0, 1.0)) = 1.0 
    }
    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Amplitude;
            float _Wavelength;
            float _WaveSpeed;
            float4 _WaveColor;
            float _AlphaClipThreshold;
            float3 _WaveDirection;
            float _StaticHeight;
            half _Glossiness;
            half _Metallic;
            float _Alpha; // Alpha property

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                float3 pos = v.vertex.xyz;
                if (pos.y < _StaticHeight) {
                    float k = 2.0 * UNITY_PI / _Wavelength;
                    float wave = _Amplitude * sin(dot(_WaveDirection.zx, pos.zx) * k + _WaveSpeed * _Time.y);
                    pos.z += wave;
                }
                o.pos = UnityObjectToClipPos(float4(pos, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = texColor * _WaveColor;
                finalColor.a *= _Alpha; // Apply the alpha value
                clip(finalColor.a - _AlphaClipThreshold);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
