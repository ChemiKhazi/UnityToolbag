#ifndef AUB_BLENDS_INCLUDED

#define AUB_BLENDS_INCLUDED


/********************FUNCTIONS********************/

float4 Darken (float4 a, float4 b) { return float4(min(a.rgb, b.rgb), 1); }

float4 Multiply (float4 a, float4 b) { return (a * b); }

float4 ColorBurn (float4 a, float4 b) { return (1-(1-a)/b); }

float4 LinearBurn (float4 a, float4 b) { return (a+b-1); }

float4 Lighten (float4 a, float4 b) { return float4(max(a.rgb, b.rgb), 1); }

float4 Screen (float4 a, float4 b) { return (1-(1-a)*(1-b)); }

float4 ColorDodge (float4 a, float4 b) { return (a/(1-b)); }

float4 LinearDodge (float4 a, float4 b) { return (a+b); }

float4 Overlay (float4 a, float4 b) {

	float3 blend = step(0.5, b);
	float3 high = 1.0-(2.0*(1.0-a)*(1.0-b));
	float3 low = (a*b*2);
	return float4(lerp(low, high, blend),1);
}

float4 SoftLight (float4 a, float4 b)
{
	// Anything .5 and up gets set to 1;
	float3 highPass = step(0.5, b);	

	float3 highContrib = a*(1-(1-a)*(1-2*b));
	float3 lowContrib = 1-(1-a)*(1-(a*(2*b)));

	return float4(lerp(lowContrib, highContrib, highPass), 1);
}

float4 HardLight (float4 a, float4 b) {
	float3 blend = step(0.5, b);
	float3 high = 1 - (1 - a) * (1-2*b);
	float3 low = a * (2 * b);
	return float4(lerp(low, high, blend), 1);
}

float4 VividLight (float4 a, float4 b) {

    float4 r = float4(0,0,0,1);

    if (b.r > 0.5) { r.r = 1-(1-a.r)/(2*(b.r-0.5)); }

    else { r.r = a.r/(1-2*b.r); }

    if (b.g > 0.5) { r.g = 1-(1-a.g)/(2*(b.g-0.5)); }

    else { r.g = a.g/(1-2*b.g); }

    if (b.b > 0.5) { r.b = 1-(1-a.b)/(2*(b.b-0.5)); }

    else { r.b = a.b/(1-2*b.b); }

    return r;

}

float4 LinearLight (float4 a, float4 b) {

    float4 r = float4(0,0,0,1);

    if (b.r > 0.5) { r.r = a.r+2*(b.r-0.5); }

    else { r.r = a.r+2*b.r-1; }

    if (b.g > 0.5) { r.g = a.g+2*(b.g-0.5); }

    else { r.g = a.g+2*b.g-1; }

    if (b.b > 0.5) { r.b = a.b+2*(b.b-0.5); }

    else { r.b = a.b+2*b.b-1; }

    return r;

}

float4 PinLight (float4 a, float4 b) {

    float4 r = float4(0,0,0,1);

    if (b.r > 0.5) { r.r = max(a.r, 2*(b.r-0.5)); }

    else { r.r = min(a.r, 2*b.r); }

    if (b.g > 0.5) { r.g = max(a.g, 2*(b.g-0.5)); }

    else { r.g = min(a.g, 2*b.g); }

    if (b.b > 0.5) { r.b = max(a.b, 2*(b.b-0.5)); }

    else { r.b = min(a.b, 2*b.b); }

    return r;

}

float4 Difference (float4 a, float4 b) { return (abs(a-b)); }

float4 Exclusion (float4 a, float4 b) { return (0.5-2*(a-0.5)*(b-0.5)); }

 

#endif