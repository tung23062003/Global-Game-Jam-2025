// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Border shader.
// Should be used only "_BorderColor", "_Borders", "_ResolutionX", "_ResolutionX", and "_Transparent" shader properties,
// other properties should have the default value to be compatible with Unity UI.

Shader"Custom/New UI Widgets/UIBorderDrawer"
{
	Properties
	{
		// Sprite texture
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		// Tint
		_Color("Tint", Color) = (1,1,1,1)

		_BorderColor("Border Color", Color) = (1,0,0,1)
		_Borders("Borders", Vector) = (1,1,1,1)
		_ResolutionX("ResolutionX", Float) = 256
		_ResolutionY("ResolutionY", Float) = 256
		_Transparent("Transparent", Float) = 0

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
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

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile_local _ UNITY_UI_CLIP_RECT
			#pragma multi_compile_local _ UNITY_UI_ALPHACLIP

			struct appdata_t
			{
				float4 vertex    : POSITION;
				float4 color     : COLOR;
				float4 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex        : SV_POSITION;
				fixed4 color         : COLOR;
				float4 texcoord      : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				half4  mask : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			float4 _MainTex_ST;
			float _UIMaskSoftnessX;
			float _UIMaskSoftnessY;

			fixed4 _BorderColor;
			float _ResolutionX;
			float _ResolutionY;
			float _Transparent;

			float4 _Borders;

			CBUFFER_END

			float line_alpha(float pos, float line_pos, float width, float resolution)
			{
				float quality = 0.01 / resolution;
				float thickness = width / resolution;
				return smoothstep(thickness + quality, thickness - quality, abs(pos.x - (line_pos / resolution)));
			}

			float lines_alpha(float2 pos, float2 resolution)
			{
				float result = 0.0;

				if (_Borders.x > 0)
				{
					result += line_alpha(pos.x, _Borders.x / 2.0, _Borders.x, _ResolutionX);
			    }

				if (_Borders.y > 0)
				{
					result += line_alpha(pos.x, (_ResolutionX - (_Borders.y / 2.0)), _Borders.y, _ResolutionX);
				}

			    if (_Borders.z > 0)
				{
					result += line_alpha(pos.y, _ResolutionY - _Borders.z / 2.0, _Borders.z, _ResolutionY);
				}

				if (_Borders.w > 0)
				{
					result += line_alpha(pos.y, _Borders.w / 2.0, _Borders.w, _ResolutionY);
				}

				return clamp(result, 0.0, 1.0);
			}

			v2f vert(appdata_t v)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, OUT);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				float4 vPosition = UnityObjectToClipPos(v.vertex);
				OUT.worldPosition = v.vertex;
				OUT.vertex = vPosition;

				float2 pixelSize = vPosition.w;
				pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

				float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
				float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

				float2 texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				OUT.texcoord = float4(texcoord.x, texcoord.y, v.texcoord.z, v.texcoord.w);
				OUT.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

				OUT.color = v.color * _Color;

				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float4 color = (UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.texcoord.xy) + _TextureSampleAdd) * IN.color;

				if (_Transparent > 0)
				{
					color.rgb = _BorderColor.rgb;
					color.a = 0;
				}

			    float2 resolution = float2(_ResolutionX, _ResolutionY);
				float l = lines_alpha(IN.texcoord.zw, resolution);

				float3 border_color = lerp(color.rgb, _BorderColor.rgb, _BorderColor.a);
				color = lerp(color, float4(border_color, _BorderColor.a), l);

				#ifdef UNITY_UI_CLIP_RECT
				float2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
				color.a *= m.x * m.y;
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
}