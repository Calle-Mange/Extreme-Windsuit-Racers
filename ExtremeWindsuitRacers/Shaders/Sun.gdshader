shader_type spatial;

uniform vec4 scanLine_Color: source_color = vec4(0.0);
uniform float line_Speed: hint_range(0.0, 1.0, 0.01) = 0.5;
uniform float lines: hint_range(0.0,100, 1.0) = 20.0;
uniform float lineWidth: hint_range(0.0, 1.0, 0.01) = 0.5;
uniform float fresnel_power: hint_range(0.0, 100.0, 0.1);
uniform vec3 fresnel_color: source_color = vec3(0.0);
uniform float extra_Lines: hint_range(0.0, 100.0, 0.1);

vec3 fresnel_glow(float amount, float intensity, vec3 color, vec3 normal, vec3 view)
{
	return pow((1.0 - dot(normalize(normal), normalize(view))), amount) * color * intensity;
}

float hash(float n) {
    return fract(sin(n) * 43758.5453123);
}

// 1D noise based on y coordinate
float noise1D(float y) {
    float i = floor(y);
    float f = fract(y);
    float u = f * f * (3.0 - 2.0 * f);
    return mix(hash(i), hash(i + 1.0), u);
}

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Scale the pattern
	float line_Time_konst = TIME*line_Speed;
	vec2 uv_offset = UV-line_Time_konst;

	float noise = noise1D(uv_offset.y * lines);
    // Create repeating pattern using fract()
    float pattern = step(1.0 - lineWidth, fract(uv_offset.y * lines));
	float pattern2 = step(0.5, fract(noise * 10.0));
	pattern2 = noise1D(uv_offset.y * lines + sin(uv_offset.x * 5.0) * extra_Lines);

    // Invert to make lines dark
    vec4 sunLines = vec4(1.0 - pattern2);
	
	vec4 linesAndColor = sunLines*scanLine_Color;
	vec3 sunLinesVec3 = linesAndColor.xyz;
	vec3 fresnel = fresnel_glow(4.0, fresnel_power, fresnel_color, NORMAL, VIEW);
	
	vec3 testvec1 = vec3(-0.1,0.1,0.1);
	vec3 testvec2 = vec3(0.1,0.1,0.1);
	vec3 testvectors = vec3(testvec1+testvec2);
	
	vec4 sunLinesVec4 = vec4(sunLinesVec3,0.1);
	vec4 fresnelVec4 = vec4(fresnel,0.1);
	
	vec3 emissionVar = vec3(sunLinesVec3+fresnel);
	emissionVar = vec3(sunLinesVec3+fresnel);
	ALBEDO = fresnel;
    EMISSION = emissionVar;
	ALPHA = linesAndColor.a;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
