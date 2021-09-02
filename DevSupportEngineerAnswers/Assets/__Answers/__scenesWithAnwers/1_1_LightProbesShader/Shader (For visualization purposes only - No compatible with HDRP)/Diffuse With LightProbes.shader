Shader "MyShader/Diffuse With LightProbes" {
	Properties{ [NoScaleOffset] _MainTex("Texture", 2D) = "white" {} }
		SubShader{
		Pass{
			Tags {
			"LightMode" = "ForwardBase"
			"RenderPipeline" = "HDRenderPipeline"
			}
			CGPROGRAM
			#pragma vertex v
			#pragma fragment f

			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
			#include "UnityLightingCommon.cginc" // for _LightColor0
			sampler2D _MainTex;

			struct v2f
			{
				float2 uv : TEXCOORD0;				
				float4 vertex : SV_POSITION;
				fixed4 diff : COLOR0; // diffuse lighting color
			};

			v2f v(appdata_base vertex_data)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(vertex_data.vertex);
				o.uv = vertex_data.texcoord;
				
				// get vertex normal in world space
				half3 worldNormal = UnityObjectToWorldNormal(vertex_data.normal);
				
				// dot product between normal and light direction for
				// standard diffuse (Lambert) lighting
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				
				// factor in the light color
				o.diff = nl * _LightColor0;

				// ShadeSH9 function from UnityCG.cginc evaluates 
				// illumination from ambient or light probes,
				// using world space normal

				//Here we add it to the diffuse lighting from the main light
				o.diff.rgb += ShadeSH9(half4(worldNormal, 1));
				return o;
			}

			fixed4 f(v2f input_fragment) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, input_fragment.uv);

				// multiply by lighting
				col *= input_fragment.diff;
				return col;
			}
			ENDCG
		}
	}
}