shader_type spatial;

void fragment() {
    // Normalize the normal just to be safe
	vec3 worldNormal = (INV_VIEW_MATRIX * vec4(NORMAL.xyz, 0.0)).xyz;
    vec3 n = normalize(worldNormal);
    
    // Create a mask for faces pointing mostly up (+Z)
    float up_mask = step(0.9, n.y); // 1.0 if normal.z > 0.9, 0.0 otherwise

    // Simple checker pattern based on UV
    vec2 uv = UV * 10.0; // Scale UVs
    float checker = step(0.5, fract(uv.x)) * step(0.5, fract(uv.y));

    // Combine mask and pattern
    vec3 pattern_color = mix(vec3(1.0), vec3(1.0, 0.0, 0.0), checker);

    // Final color: show pattern only on faces pointing up
    ALBEDO = mix(vec3(1.0), pattern_color, up_mask);
}