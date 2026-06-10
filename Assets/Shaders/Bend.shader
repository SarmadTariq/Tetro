Shader "Unlit/Bend"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            cull off
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            sampler2D _MainTex;
			float4 _MainTex_ST;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD1;
            };

            half4 Bend(half4 v)
            {
                half4 wpos = mul (unity_ObjectToWorld, v);
			    half angle = wpos.x/14 * 3.1415926 * 2;
			    wpos.x = (cos(angle) * wpos.z) + 7;
			    wpos.z *= sin(angle);
                wpos = mul (unity_WorldToObject, wpos);
                return wpos;
            }

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex = Bend(v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float3 n = (i.normal + 1)/2;
                float3 dL = dot(n, _WorldSpaceLightPos0.xyz);
                return (col * float4(dL,1));
            }
            ENDCG
        }
    }
}
