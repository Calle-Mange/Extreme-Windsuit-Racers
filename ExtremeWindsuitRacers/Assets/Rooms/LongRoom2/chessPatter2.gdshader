shader_type spatial;

varying vec3 world_pos;
varying vec3 world_normal;
uniform float square_amount: hint_range(0.0, 30.0, 1.0) = 5.0;
uniform float frequency: hint_range(0.0, 30.0, 1.0) = 10.0;
uniform float amplitude: hint_range(0.0, 5.0, 0.1) = 0.2;
uniform float speed: hint_range(0.0, 5.0, 0.01) = 1.0;
uniform float room_time_scale: hint_range(0.0, 5.0, 0.01) = 1.0;
uniform float floor_time_scale: hint_range(0.0, 5.0, 0.01) = 1.0;
uniform float scroll_speed = 1.0;
uniform float noise_density: hint_range(50.0, 1000.0, 0.01) = 50.0; // Higher = fewer white dots
uniform float jitter_amount: hint_range(0.01, 10.0, 0.001) = 0.05;
uniform float min_jitter_amplitude : hint_range(-1.0, 1.0, 0.001) = 0.00;  // Minimum amplitude
uniform float max_jitter_amplitude : hint_range(-1.0, 1.0, 0.001) = 0.00;   // Maximum amplitude
uniform float jitter_hz : hint_range(0.0, 100.0, 0.01) = 0.00;   // Maximum amplitude
uniform float jitter_rotation_hz : hint_range(0.0, 100.0, 0.01) = 0.00;   // Maximum amplitude

void vertex() {
	// Compute world-space position
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;

    // Transform normal to world space
    world_normal = normalize((MODEL_MATRIX * vec4(NORMAL, 0.0)).xyz);
}

float hash(vec2 p) {
	float floor_time = TIME * floor_time_scale;
	 vec2 grid = world_pos.xz * square_amount;
        grid.y -= floor_time; // Scroll along Z-axis

        // Create checker pattern
        float checker = mod(floor(grid.x) + floor(grid.y), 2.0);
    return mod(fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453), grid.x);
}

void fragment() {
	
	const vec3 vaporWavePalette[6] = vec3[6](
		vec3(1.0, 0.38039215, 0.7764705882352941),  // #ff61c6ff								Pink
	    vec3(0.3607843137254902, 0.9254901960784314, 1.0),    // #5cecffff 						Light Blue
	    vec3(0.9568627450980393, 1.0, 0.3803921568627451),    // #f4ff61ff 						yellow
	    vec3(1.0, 0.6, 0.0),    // #ff9900ff 													Orange
	    vec3(0.21568627450980393, 0.34901960784313724, 0.44313725490196076),    // #375971ff	Blue
		vec3(0.01, 0.01, 0.1)		// #0a0c37ff												Dark Blue
	);
	
	vec3 color_a = vec3(vaporWavePalette[0]);
	vec3 color_b = vec3(vaporWavePalette[1]);

	float room_time = TIME * room_time_scale;
	float floor_time = TIME * floor_time_scale;

    // Diagonal base pattern
    float base = (UV.x + UV.y) * 0.5;
	
    // Add a sine wave distortion
    float wave = sin((UV.y + room_time * speed) * frequency) * amplitude;
    float factor = clamp(base + wave, 0.0, 1.0);
	vec3 gradient_color = mix(color_a, color_b, factor);
	
	// Small white artifacts using hashed noise
    vec2 noise_coords = world_pos.xz + vec2(1.0, 1.0) * world_pos.y; // Use world position and height for variation
	float noise_density_val = sin(noise_density * 100.0) + fract(sin(noise_density * 100.0) + 1.0);
	float noise_val = hash(noise_coords * noise_density_val);
	
	// Jitter effect on noise threshold
	// Create a random offset to jitter the artifact
	// Fade the jitter value back and forth using a sine wave
	 // Oscillate amplitude between min_jitter_amplitude and max_jitter_amplitude
    float amplitude_range = max_jitter_amplitude - min_jitter_amplitude;
    float jitter = min_jitter_amplitude + 0.5 * amplitude_range * (sin(TIME * jitter_hz) + 1.0);
	float jittered_noise_val = noise_val + jitter;

	
	// Random color for specks based on world position
	vec3 speck_color = vec3(
		0.5 + 0.5 * sin(world_pos.x * 0.1 + TIME),  // Red channel (based on X position and time)
		0.5 + 0.5 * sin(world_pos.y * 0.1 + TIME),  // Green channel (based on Y position and time)
		0.5 + 0.5 * sin(world_pos.z * 0.1 + TIME)   // Blue channel (based on Z position and time)
	);
	
	//speck_color = vec3(
		//sin(vaporWavePalette[5].x),  // Red channel (based on X position and time)
		//sin(vaporWavePalette[5].y),  // Green channel (based on Y position and time)
		//sin(vaporWavePalette[5].z)   // Blue channel (based on Z position and time)
	//);

	// Determine if this pixel is a white speck
	bool is_speck = jittered_noise_val < 0.02;
    
	if (world_normal.y > 0.95) { // Use .y, since Y is "up" in Godot
        // Use XZ plane for pattern (horizontal plane) with Z scrolling
        vec2 grid = world_pos.xz * square_amount;
        grid.y -= floor_time; // Scroll along Z-axis

        // Create checker pattern
        float checker = mod(floor(grid.x) + floor(grid.y), 2.0);
		
		//Lägg till rosa och ljusblå gradient på resetn

        // Two alternating colors
        ALBEDO = mix(vec3(1.0), vec3(0.0), checker);
    } else {
        //ALBEDO = gradient; // default for downward or sideways faces
		ALBEDO = is_speck ? speck_color : gradient_color;
    }
	
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
