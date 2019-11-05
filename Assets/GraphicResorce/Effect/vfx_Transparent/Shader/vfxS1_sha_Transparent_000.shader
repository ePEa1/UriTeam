Shader "Custom/Transparent"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Trans ("Trans", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 200

		zwrite on
		ColorMask 0

        CGPROGRAM
        #pragma surface surf Change
        #pragma target 3.0

        struct Input
        {
			float4 color:COLOR;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
        }

		float4 LightingChange(SurfaceOutput s, float3 lightDir, float atten)
		{
			return float4 (0, 0, 0, 0);
		}

        ENDCG


		//2pass

		zwrite off
		CGPROGRAM
		#pragma surface surf Standard alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;
		float _Trans;
		float4 _Color;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * c.rgb * c.rgb;
			o.Alpha = _Trans;
		}

		ENDCG
    }
    FallBack "Diffuse"
}
