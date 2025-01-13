// Rounded corners shader, each corner has own radius.
// Should be used only "_Size", "_HalfSizeAndOrigin", "_InternalHalfSizeAndOrigin", "_BorderRadius", "_BorderColor", and "_BorderWidth" shader properties,
// other properties should have the default value to be compatible with Unity UI.

Shader "Custom/New UI Widgets/UIRoundedCornersX4"
{
	Properties
	{
		// Sprite texture
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		// Tint
		_Color("Tint", Color) = (1,1,1,1)
		_HalfSizeAndOrigin("Half Size and Origin", Vector) = (10, 10, 10, 10)
		_InternalHalfSizeAndOrigin("Internal Half Size and Origin", Vector) = (10, 10, 10, 10)
		_BorderRadius("Border Radius", Vector) = (10, 10, 10, 10)
		_BorderColor("Border Color", Color) = (1,0,0,1)
		_BorderWidth("Border Width", Float) = 4

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

			float2 _Size = float2(100.0f, 100.0f);
			float4 _HalfSizeAndOrigin;
			float4 _InternalHalfSizeAndOrigin;

			float4 _BorderRadius;
			float4 _BorderColor;
			float _BorderWidth;
			CBUFFER_END
			
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

			inline float Rectangle(float2 position, float2 halfSize)
			{
				float2 distance2edge = abs(position) - halfSize;
				float outside_distance = length(max(distance2edge, 0));
				float inside_distance = min(max(distance2edge.x, distance2edge.y), 0);
				return outside_distance + inside_distance;
			}

			inline float2 RotatePosition(float2 position, float rotation)
			{
				const float PI = 3.14159;
				float angle = rotation * PI * 2 * -1;
				float sin_value, cos_value;
				sincos(angle, sin_value, cos_value);
				return float2(cos_value * position.x + sin_value * position.y, cos_value * position.y - sin_value * position.x);
			}

			inline float CornerAlpha(float2 position, float2 offset, float radius)
			{
				return length(position - offset) - radius;
			}

			float RectangleAlpha(float2 position, float2 size, float4 halfSizeAndOrigin, float4 radius, float border)
			{
				const float angle = 45.0f / 360.0f;

				radius -= border;
				position *= size;

				float2 half_size = size * 0.5f - border;
				float main_alpha = Rectangle(position, half_size);

				float2 rotated_position = RotatePosition(position - halfSizeAndOrigin.zw, angle);
				float rotated_alpha = Rectangle(rotated_position, halfSizeAndOrigin.xy);

				float top_left = length(position - float2(-half_size.x + radius.x, half_size.y - radius.x)) - radius.x;
				float top_right = length(position - (half_size - radius.y)) - radius.y;
				float bottom_right = length(position - float2(half_size.x - radius.z, -half_size.y + radius.z)) - radius.z;
				float bottom_left = length(position - (-half_size + radius.w)) - radius.w;

				float alpha = max(main_alpha, min(min(min(min(rotated_alpha, top_left), top_right), bottom_right), bottom_left));

				float step = fwidth(alpha) * 0.5f;
				return smoothstep(step, -step, alpha);
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float4 color = (UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.texcoord.xy) + _TextureSampleAdd) * IN.color;
				float2 pos = IN.texcoord.zw - float2(0.5f, 0.5f);

				float alpha = RectangleAlpha(pos, _Size, _HalfSizeAndOrigin, _BorderRadius, 0.0f);
				float border_alpha = _BorderWidth > 0 ? RectangleAlpha(pos, _Size, _InternalHalfSizeAndOrigin, _BorderRadius, _BorderWidth) : 1.0f;
				color.a = min(lerp(color.a, _BorderColor.a, 1 - border_alpha), alpha);
				color.rgb = lerp(_BorderColor.rgb, color.rgb, border_alpha);

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