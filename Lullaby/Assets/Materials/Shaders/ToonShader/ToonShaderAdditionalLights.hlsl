void AdditionalLight_float(float3 WorldPos, float3 Normal, float3 View, float Smoothness, out float3 Diffuse, out float3 Specular)
{
    Diffuse = float3(0.0f, 0.0f, 0.0f);
	Specular = float3(0.0f, 0.0f, 0.0f);

	#ifndef SHADERGRAPH_PREVIEW
		int pixelLightCount = GetAdditionalLightsCount();
		for (int j = 0; j < pixelLightCount; ++j)
		{
			Light light = GetAdditionalLight(j, WorldPos);
    
			float dotProd = dot(light.direction, Normal);
			float3 colorAtt = light.color * (light.distanceAttenuation * light.shadowAttenuation);
			Diffuse += saturate(dotProd) * colorAtt;
			Specular += pow(saturate(dot(normalize(light.direction + View), Normal)) * step(0, dotProd), Smoothness) * colorAtt;
		}
	#endif
}

void AdditionalLight_half(half3 WorldPos, half3 Normal, half3 View, half Smoothness, out half3 Diffuse, out half3 Specular)
{
	Diffuse = float3(0.0f, 0.0f, 0.0f);

	#ifndef SHADERGRAPH_PREVIEW
		int pixelLightCount = GetAdditionalLightsCount();
		for (int j = 0; j < pixelLightCount; ++j)
		{
			Light light = GetAdditionalLight(j, WorldPos);
    
			half dotProd = dot(light.direction, Normal);
			half3 colorAtt = light.color * (light.distanceAttenuation * light.shadowAttenuation);
			Diffuse += saturate(dotProd) * colorAtt;
			Specular += pow(saturate(dot(normalize(light.direction + View), Normal)) * step(0, dotProd), Smoothness) * colorAtt;	
		}
	#endif
}