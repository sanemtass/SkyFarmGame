Shader "1UP/Magic Snow/Snow standard" {
	Properties {
		//[Header(Mesh Subdivision)]
		_Tess("Subdivision Level", Range(1, 20)) = 10
		[Space]
		//[Header(Snow)]
		_SnowColor("Snow Ground Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_PathColor("Snow Path Color", Color) = (0.16, 0.6, 0.94, 1.0)
        _SmoothColor("Snow Side Color", Color) = (1.0, 0.0, 0.0, 0.0)
        _SmoothRatio("Snow Side Intensity", Range(0.0, 1.0)) = 0.2
        _SmoothOffset("Snow Side Offset", Range(0.3, 0.7)) = 0.5
		_SnowDepth("Snow Height", Range(-2,2)) = 0.0
		[HideInInspector]
		_PathTex("Path Texture", 2D) = "black" {}

        //[Header(Diffuse)]
        _DiffuseTex("Detail Texture", 2D) = "white" {}
        _DiffuseRatio("Intensity", Range(0.0, 1.0)) = 0.0
	}

	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
        ZWrite On
		CGPROGRAM

		#pragma surface surf Lambert vertex:vert tessellate:tessDistance nolightmap addshadow
		#pragma target 3.5
		#pragma require tessellation tessHW
		#include "Tessellation.cginc"
		
		uniform sampler2D _PathTex;
		uniform float4 _SnowColor;
		uniform float4 _PathColor;
        uniform float4 _SmoothColor;
        uniform float _SmoothRatio;
		uniform float _SmoothOffset;
        uniform float _SnowDepth;

        uniform sampler2D _DiffuseTex;
        uniform float _DiffuseRatio;

		float _Tess;

		float4 tessDistance(appdata_full v0, 
            appdata_full v1, appdata_full v2) {
			float minDist = 20.0;
			float maxDist = 45.0;
			return UnityDistanceBasedTess(v0.vertex, 
                v1.vertex, v2.vertex, minDist, maxDist, 
                _Tess);
			//return UnityEdgeLengthBasedTess(v0.vertex, v1.vertex, v2.vertex, _Tess);
		}

		struct Input {
			float2 uv_PathTex : TEXCOORD0;
		};
        
		void vert(inout appdata_full v) {	
			float2 uv = v.texcoord.xy;
            float pathMask = 1.0 - tex2Dlod(_PathTex, float4(uv, 0, 0)).g;
            float4 wpos = mul(unity_ObjectToWorld, v.vertex);
            float4 wnormal = mul(unity_ObjectToWorld, v.normal);
            wpos.y += _SnowDepth * normalize(wnormal).y * pathMask;
            v.vertex = mul(unity_WorldToObject, wpos);
		}

		void surf(Input IN, inout SurfaceOutput o) {
			float2 uv = IN.uv_PathTex.xy;
            float pathMask = tex2D(_PathTex, uv).g;
            float smoothMask = 0.2 - abs(pathMask - (1.0 - _SmoothOffset));
            smoothMask = step(0.0, smoothMask);
            fixed4 diffColor = tex2D(_DiffuseTex, uv);

            fixed4 color = _SnowColor;
            color.rgb = lerp(color.rgb, diffColor.rgb, _DiffuseRatio);

            float4 pathColor = _PathColor;
            float4 smoothColor = _SmoothColor;
            smoothMask *= _SmoothRatio;
            pathColor.rgb = lerp(pathColor, smoothColor, smoothMask);
            pathColor.a = pathMask + smoothMask;
            color = lerp(color, pathColor, pathColor.a);

			o.Albedo = color;
		}
		ENDCG
	}

	Fallback "Diffuse"
    CustomEditor "GUIAsset1UP.MagicSnowGUI"
}
