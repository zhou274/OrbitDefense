Shader "Lovatto/2D/Outline_2DSprite"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Width("Thickness", Range(0, 1)) = 0.5
		_Fill("Filling", Range(0 , 1)) = 1
	}

		SubShader
	{
		Lighting Off
		LOD 110

		CGPROGRAM
#pragma surface surf Lambert alpha

	struct Input
	{
		float2 uv_MainTex;
		fixed4 color : COLOR;
	};

	sampler2D _MainTex;
	float _Width;
	float _Fill;

	void surf(Input IN, inout SurfaceOutput o)
	{
		if (IN.uv_MainTex.y > _Fill)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		else
		{
			_Width /= 100;

			fixed4 TempColor = tex2D(_MainTex, IN.uv_MainTex + float2(_Width, _Width))
				+ tex2D(_MainTex, IN.uv_MainTex - float2(_Width, _Width));

			fixed4 OutlineColor = 0;
			OutlineColor.r = (1 - _Fill) / (_Fill * _Fill);
			OutlineColor.g = _Fill;
			OutlineColor.b = 0;
			OutlineColor.a = 1;

			fixed4 AlphaColor = (0, 0, 0, TempColor.a);
			fixed4 MainColor = AlphaColor * OutlineColor.rgba;
			fixed4 TexColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color;

			if (TexColor.a > 0.95)
				MainColor = TexColor;

			o.Albedo = MainColor.rgb;
			o.Alpha = MainColor.a;
		}
	}
	ENDCG
	}

		Fallback "Diffuse"
}