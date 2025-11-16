Shader "Custom/RadialProgress"
{
    Properties
    {
        _Progress ("Progress", Range(0, 1)) = 0
        _FillColor ("Fill Color", Color) = (0, 1, 0, 1)
        _EmptyColor ("Empty Color", Color) = (0.2, 0.2, 0.2, 0.3)
        _InnerRadius ("Inner Radius", Range(0, 0.5)) = 0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            float _Progress;
            float4 _FillColor;
            float4 _EmptyColor;
            float _InnerRadius;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 delta = i.uv - center;
                float dist = length(delta);
                
                if (dist > 0.5)
                {
                    discard;
                }
                
                if (dist < _InnerRadius)
                {
                    discard;
                }
                
                float angle = atan2(delta.y, delta.x);
                angle = degrees(angle);
                angle = (angle + 90.0) % 360.0;
                if (angle < 0) angle += 360.0;
                
                float progressAngle = _Progress * 360.0;
                
                fixed4 col = angle <= progressAngle ? _FillColor : _EmptyColor;
                
                float edgeSoftness = 0.02;
                float outerEdge = smoothstep(0.5, 0.5 - edgeSoftness, dist);
                float innerEdge = _InnerRadius > 0 ? smoothstep(_InnerRadius, _InnerRadius + edgeSoftness, dist) : 1.0;
                col.a *= outerEdge * innerEdge;
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

