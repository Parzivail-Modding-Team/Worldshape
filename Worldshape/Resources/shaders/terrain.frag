#version 330 core

uniform vec3 lightPos;
uniform int samples;
uniform sampler2D random;
uniform sampler2D atlas;

in vec3 fragPos;
in vec3 fragNormal;
in vec2 fragTexCoord;

out vec4 color;

void main()
{
	vec2 resolution = vec2(512., 512.);

    vec3 norm = normalize(fragNormal);
    vec3 lightDir = normalize(lightPos); // light very far away, use direction only. If light has position: normalize(lightPos - fragPos)  
    float diffuse = max(dot(norm, lightDir), 0.0);
    float ambient = 0.5;
	
    // Look up noise from texture
    vec4 noise = texture(random, gl_FragCoord.xy / resolution.xy);
    
	vec4 samp = texture(atlas, fragTexCoord);
	if(samp.a < 0.5)
		discard;
    color = vec4(samp.rgb * clamp(ambient + diffuse, 0, 1), samp.a);

    color += vec4(vec3((noise.r - 0.5) / 255.0), 0.0);
}