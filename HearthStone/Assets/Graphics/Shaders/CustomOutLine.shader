Shader "Custom/CustomOutLine"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[MaterialToggle] _IsOutlineEnabled("Enable Outline", float) = 0
		_OutLineColor("OutLineColor", Color) = (1,1,1,1)
		_OutLineSize("OutLineSize",float) = 0
		_AlphaCutOff("AlphaCutOff", Range(0, 1)) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _ PIXELSNAP_ON
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
				};

				fixed4 _Color;
				fixed4 _OutLineColor;
				float _OutLineSize;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color;
					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				float _AlphaSplitEnabled;
				float _IsOutlineEnabled;
				float _AlphaCutOff;

				fixed4 SampleSpriteTexture(float2 uv)
				{
					fixed4 color = tex2D(_MainTex, uv);

					#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
					if (_AlphaSplitEnabled)
						color.a = tex2D(_AlphaTex, uv).r;
					#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

					if (_IsOutlineEnabled == 1)
					{
						if(tex2D(_MainTex, uv + float2(0, _OutLineSize)).a > _AlphaCutOff || tex2D(_MainTex, uv - float2(0, _OutLineSize)).a > _AlphaCutOff || tex2D(_MainTex, uv - float2(_OutLineSize, 0)).a > _AlphaCutOff || tex2D(_MainTex, uv + float2(_OutLineSize, 0)).a > _AlphaCutOff)
							if (color.a <= _AlphaCutOff)
								color = _OutLineColor;
					}
					else
					{
						if (tex2D(_MainTex, uv + float2(0, _OutLineSize)).a <= _AlphaCutOff || tex2D(_MainTex, uv - float2(0, _OutLineSize)).a <= _AlphaCutOff || tex2D(_MainTex, uv - float2(_OutLineSize, 0)).a <= _AlphaCutOff || tex2D(_MainTex, uv + float2(_OutLineSize, 0)).a <= _AlphaCutOff)
							if (color.a > _AlphaCutOff)
								color = _OutLineColor;
					}

					//if (tex2D(_MainTex, uv + float2(0, _OutLineSize)).a != 0 && tex2D(_MainTex, uv - float2(0, _OutLineSize)).a != 0 && tex2D(_MainTex, uv - float2(_OutLineSize, 0)).a != 0 && tex2D(_MainTex, uv + float2(_OutLineSize, 0)).a != 0)
					//	if (color.a != 0)
					//		color = _OutLineColor;

					return color;
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
					c.rgb *= c.a;
					return c;
				}
			ENDCG
			}
		}
}
