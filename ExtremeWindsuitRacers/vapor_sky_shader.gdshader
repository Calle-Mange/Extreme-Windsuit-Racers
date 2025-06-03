shader_type sky;

uniform vec3 skyColor : source_color;
uniform vec3 horizonColor : source_color;
uniform sampler2D skyTexture;

void sky() {
	//COLOR = skyColor;
	float col = clamp(EYEDIR.y / 0.05, 0.0, 1.0);
	vec3 finalColor = mix(horizonColor,skyColor,col);
	COLOR = vec3(finalColor);

	//Testa att sätta alpha
	//vec4 alphaTexture = texture(skyTexture,SKY_COORDS);
	//alphaTexture.a = 0.5;
	//COLOR = alphaTexture.rgb;
}
