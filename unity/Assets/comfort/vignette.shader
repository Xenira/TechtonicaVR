Shader "Custom/vignette"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Center ("Center", Vector) = (0.25, 0.25, 0, 0)
        _Radius ("Radius", Range(0, 1)) = 0.5
        _Blur ("Blur", Range(0, 1)) = 0.5
        _VignetteColor ("Vignette Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay+5" "IgnoreProjector"="True" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Center;
        half _Radius;
        half _Blur;
        fixed4 _VignetteColor;

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Calculate the distance from the center
            float2 center = _Center.xy;
            float2 distance = IN.uv_MainTex - center;
            float dist = length(distance) * 3.5;
            _Radius = 1 - _Radius;
            float innerRadius = _Radius - _Blur;

            // Calculate the inverted vignette effect
            float vignette = 1 - smoothstep(innerRadius, _Radius, dist);

            // Apply the inverted vignette effect to the color
            fixed4 c = _VignetteColor;
            // c.rgb *= vignette;
            c.a *= 1 - vignette;

            // Set the color to the vignette color if outside the radius
            if (dist <= innerRadius)
            {
                c.a = 0;
            }

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
