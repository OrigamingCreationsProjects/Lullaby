#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

// LUZ PRINCIPAL DIRECCIONAL

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, 
	out float DistanceAtten, out float ShadowAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
        Direction = float3(0.5f, 0.5f, 0);
        Color = float3(1.0f, 1.0f, 1.0f);
        DistanceAtten = 1.0f;
        ShadowAtten = 1.0f;
    #else
        float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        Light mainLight = GetMainLight(shadowCoord);
 
        Direction = mainLight.direction;
        Color = mainLight.color;
        DistanceAtten = mainLight.distanceAttenuation;
        //ShadowAtten = mainLight.shadowAttenuation;

        #if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
		    ShadowAtten = 1.0f;
	    #else
            ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
            float shadowStrength = GetMainLightShadowStrength();
            ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
            sampler_MainLightShadowmapTexture),
            shadowSamplingData, shadowStrength, false);
        #endif
    #endif
}

void MainLight_half(half3 WorldPos, out half3 Direction, out half3 Color, 
	out half DistanceAtten, out half ShadowAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
        Direction = half3(0.5h, 0.5h, 0);
        Color = half3(1.0h, 1.0h, 1.0h);
        DistanceAtten = 1.0h;
        ShadowAtten = 1.0h;
    #else
        half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        Light mainLight = GetMainLight(shadowCoord);
 
        Direction = mainLight.direction;
        Color = mainLight.color;
        DistanceAtten = mainLight.distanceAttenuation;
        //ShadowAtten = mainLight.shadowAttenuation;

        #if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
		    ShadowAtten = 1.0h;
	    #else
            ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
            half shadowStrength = GetMainLightShadowStrength();
            ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
            sampler_MainLightShadowmapTexture),
            shadowSamplingData, shadowStrength, false);
        #endif
    #endif
}

// REFLEXIÓN ESPECULAR

void DirectSpecular_float(float3 Specular, float Smoothness, float3 Direction, 
    float3 Color, float3 Normal, float3 View, out float3 OutSpecular)
{
    #ifdef SHADERGRAPH_PREVIEW
        OutSpecular = 0;
    #else
        Smoothness = exp2(10 * Smoothness + 1);
            // Esta expresión amplía el rango de valores de suavidad para enfatizar la diferencia
		    // entre superficies muy suaves y muy rugosas, ya que pequeños cambios en el valor de
		    // suavidad pueden tener un gran impacto en la apariencia de los reflejos especulares
        Normal = normalize(Normal);
        View = SafeNormalize(View);
        OutSpecular = LightingSpecular(Color, Direction, Normal, View, float4(Specular, 0), Smoothness);
    #endif
}

void DirectSpecular_half(half3 Specular, half Smoothness, half3 Direction, 
    half3 Color, half3 Normal, half3 View, out half3 OutSpecular)
{
    #ifdef SHADERGRAPH_PREVIEW
        OutSpecular = 0;
    #else
    Smoothness = exp2(10 * Smoothness + 1);
            // Esta expresión amplía el rango de valores de suavidad para enfatizar la diferencia
		    // entre superficies muy suaves y muy rugosas, ya que pequeños cambios en el valor de
		    // suavidad pueden tener un gran impacto en la apariencia de los reflejos especulares
    Normal = normalize(Normal);
    View = SafeNormalize(View);
    OutSpecular = LightingSpecular(Color, Direction, Normal, View, half4(Specular, 0), Smoothness);
    #endif
}

// LUCES ADICIONALES

void AdditionalLight_float(float3 SpecularColor, float Smoothness, float3 WorldPos,
    float3 Normal, float3 View, out float3 Diffuse, out float3 Specular)
{
    float3 diffuseColor = float3(0.0f, 0.0f, 0.0f);
    float3 specularColor = float3(0.0f, 0.0f, 0.0f);

    #ifndef SHADERGRAPH_PREVIEW
        Smoothness = exp2(10 * Smoothness + 1);
		    // Esta expresión amplía el rango de valores de suavidad para enfatizar la diferencia
		    // entre superficies muy suaves y muy rugosas, ya que pequeños cambios en el valor de
		    // suavidad pueden tener un gran impacto en la apariencia de los reflejos especulares
	
        Normal = normalize(Normal);
        View = SafeNormalize(View);
	
        int pixelLightCount = GetAdditionalLightsCount();
        for (int i = 0; i < pixelLightCount; ++i)
        {
            Light light = GetAdditionalLight(i, WorldPos);
    
			    //float dotProd = dot(light.direction, Normal);
            float3 colorAtt = light.color * (light.distanceAttenuation * light.shadowAttenuation);
			
			    //Diffuse += saturate(dotProd) * colorAtt;
            diffuseColor += LightingLambert(colorAtt, light.direction, Normal);
		
			    //Specular += pow(saturate(dot(normalize(light.direction + View), Normal)) * step(0, dotProd), Smoothness) * colorAtt;
            specularColor += LightingSpecular(colorAtt, light.direction, Normal, View, float4(SpecularColor, 0), Smoothness);
        }
    #endif
	
    Diffuse = diffuseColor;
    Specular = specularColor;
}

void AdditionalLight_half(half3 SpecularColor, half Smoothness, half3 WorldPos, 
    half3 Normal, half3 View, out half3 Diffuse, out half3 Specular)
{
    half3 diffuseColor = half3(0.0h, 0.0h, 0.0h);
    half3 specularColor = half3(0.0h, 0.0h, 0.0h);

    #ifndef SHADERGRAPH_PREVIEW
        Smoothness = exp2(10 * Smoothness + 1);
            // Esta expresión amplía el rango de valores de suavidad para enfatizar la diferencia
		    // entre superficies muy suaves y muy rugosas, ya que pequeños cambios en el valor de
		    // suavidad pueden tener un gran impacto en la apariencia de los reflejos especulares
	
        Normal = normalize(Normal);
        View = SafeNormalize(View);
	
        int pixelLightCount = GetAdditionalLightsCount();
        for (int i = 0; i < pixelLightCount; ++i)
        {
            Light light = GetAdditionalLight(i, WorldPos);
    
			    //half dotProd = dot(light.direction, Normal);
            half3 colorAtt = light.color * (light.distanceAttenuation * light.shadowAttenuation);
			    //Diffuse += saturate(dotProd) * colorAtt;
            diffuseColor += LightingLambert(colorAtt, light.direction, Normal);
		
			    //Specular += pow(saturate(dot(normalize(light.direction + View), Normal)) * step(0, dotProd), Smoothness) * colorAtt;
            specularColor += LightingSpecular(colorAtt, light.direction, Normal, View, half4(SpecularColor, 0), Smoothness);
        }	
    #endif
    
    Diffuse = diffuseColor;
    Specular = specularColor;
}

#endif