shader_type spatial;
//render_mode unshaded; //Removes global shading

uniform sampler2D SCREEN_TEXTURE: hint_screen_texture, filter_linear_mipmap;
uniform sampler2D NORMAL_TEXTURE: hint_normal_roughness_texture, filter_linear_mipmap;
uniform float threshold: hint_range(0.0, 1.0, 0.01) = 0.5;
uniform vec3 line_color: source_color = vec3(1.0);
uniform vec3 background_color: source_color = vec3(0.0);

const mat3 sobel_x = mat3(
	vec3(1.0,2.0,1.0),
	vec3(0.0,0.0,0.0),
	vec3(-1.0,-2.0,-1.0)
);

const mat3 sobel_y = mat3(
	vec3(1.0,0.0,-1.0),
	vec3(2.0,0.0,-2.0),
	vec3(1.0,0.0,-1.0)
);

void vertex() {
	POSITION = vec4(VERTEX.xy,1.0,1.0);
}

void fragment() {
	vec2 screen_uv = SCREEN_UV;
	vec4 pixel = texture(SCREEN_TEXTURE, screen_uv);
	vec3 normal = texture(NORMAL_TEXTURE, screen_uv).rgb;
	normal = normal * 2.0 - 1.0;
	vec2 pixel_size = 1.0 / VIEWPORT_SIZE;
	mat3 pixels;
	for (int x = -1; x <= 1; x++) {
		vec3 row;
		for (int y = -1; y <= 1; y++){
			vec2 offset = pixel_size * vec2(float(x), float(y));
			vec3 cell = texture(NORMAL_TEXTURE, screen_uv + offset).rgb;
			row[y + 1] = length(cell - normal);
		}
		pixels[x + 1] = row;
	}
	float gx = 0.0;
	for (int i = 0; i <= 2; i++){
		gx += dot(sobel_x[i],pixels[i]);
	}
	float gy = 0.0;
	for (int i = 0; i <= 2; i++){
		gy += dot(sobel_y[i],pixels[i]);
	}


	const vec3 vaporWavePalette[6] = vec3[6](
		vec3(1.0, 0.38039215, 0.7764705882352941),  // #ff61c6ff								Pink
	    vec3(0.3607843137254902, 0.9254901960784314, 1.0),    // #5cecffff 						Light Blue
	    vec3(0.9568627450980393, 1.0, 0.3803921568627451),    // #f4ff61ff 						yellow
	    vec3(1.0, 0.6, 0.0),    // #ff9900ff 													Orange
	    vec3(0.21568627450980393, 0.34901960784313724, 0.44313725490196076),    // #375971ff	Blue
		vec3(0.01, 0.01, 0.1)		// #0a0c37ff												Dark Blue
	);

	float sobel = sqrt(gx * gx + gy * gy);
	//vec3 testLineColor = vaporWavePalette[4];
	//vec3 testBgColor = vaporWavePalette[5];
	ALBEDO = sobel < threshold ? background_color : line_color;


	//ALBEDO = testColor;


	// Få lokala toppar att bli s
}
