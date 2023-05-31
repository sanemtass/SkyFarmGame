Shader "1UP/Magic Snow/FootTex"
{
    Properties
    {
        _FootShape("FootShape", 2D) = "green" {}
    }
    SubShader
    {
        //Tags { "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha One
        //ZWrite Off

        Pass {
        CGPROGRAM

        // Use shader model 3.0 target, to get nicer looking lighting
        // #pragma target 3.0
        #pragma multi_compile_instancing
        #include "UnityCG.cginc"

        #pragma vertex vert
        #pragma fragment frag

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        uniform sampler2D _FootShape;

        v2f vert (appdata v)
        {
            v2f o;
            UNITY_INITIALIZE_OUTPUT(v2f, o);
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            o.vertex = UnityObjectToClipPos(v.vertex);

            o.uv = v.uv;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            UNITY_SETUP_INSTANCE_ID(i)
            fixed4 col = fixed4(0.0, 0.0, 0.0, 1.0);
            float dist = 0.5 * (1.0 - distance(i.uv, fixed2(0.5, 0.5)));
            col.g = dist * tex2D(_FootShape, i.uv).a;
            return col;
        }
        ENDCG
        }
    }
    FallBack "Diffuse"
}
