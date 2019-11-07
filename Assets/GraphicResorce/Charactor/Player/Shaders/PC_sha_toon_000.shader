Shader "Custom/PC_sha_toon_000"
{
    Properties
    {
		_Color ("Color", color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_TestTex ("LightTex", 2D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 color : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

			float4 _Color;
            sampler2D _MainTex;
			sampler2D _TestTex;
            float4 _MainTex_ST;
			float4 _TestTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _TestTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D (_MainTex, i.uv);
				float4 col2 = tex2D(_TestTex, i.uv2);

                UNITY_APPLY_FOG(i.fogCoord, col);
                
				return col * col2;
            }
            ENDCG
        }
    }
}
