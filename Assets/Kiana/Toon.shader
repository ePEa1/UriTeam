Shader "Custom/Toon"
{
    Properties
    {
		_Color ("Color", color) = (1,1,1,1)
		_Color2("Color2", color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_LitTex ("LightMap", 2D) = "white" {}
		
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		cull front

		CGPROGRAM
		#pragma surface surf Unlit vertex:vert 
		#pragma target 3.0

		float4 _Color;

		void vert(inout appdata_full v)
		{
			v.vertex.xyz = v.vertex.xyz + v.normal.xyz * 0.00001;
		}

		struct Input
		{
			float4 color:COLOR;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{

		}

		float4 LightingUnlit(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			return float4 (0,0,0,1);
		}

		ENDCG

		//2pass

		cull back

		CGPROGRAM
		#pragma surface surf Toon
		#pragma target 3.0

		float4 _Color;
		float4 _Color2;
        sampler2D _MainTex;
		sampler2D _LitTex;
		

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_LitTex;
			float3 lightDir;
			float3 worldPos;
			float3 viewDir;
        };



        void surf (Input IN, inout SurfaceOutput o)
        {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 d = tex2D(_LitTex, IN.uv_LitTex);

			//ToonShader term
			//float diffColor;
			/*float ndotl = dot (IN.lightDir , o.Normal);

			float3 toon = step ( ndotl * (1 - _Color2), d.g ) * 0.5 + 0.5;*/

			//diffColor = c.rgb * ndotl * _LightColor0.rgb;// * atten

			o.Albedo = c.rgb;
			o.Alpha = c.a;
        }

		float4 LightingToon (SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			//ToonShader term
			float diffColor;
			float ndotl = dot(lightDir, s.Normal);

			float3 toon = step (ndotl * (1 - _Color2), 0.5) * 0.5 + 0.5;

			diffColor = s.Albedo * ndotl * _LightColor0.rgb * atten ;
			return float4(s.Albedo, s.Alpha);
		}

        ENDCG

		
	}

    FallBack "Diffuse"
}
