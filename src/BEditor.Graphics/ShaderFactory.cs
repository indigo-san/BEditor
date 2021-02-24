﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEditor.Graphics
{
    public class ShaderFactory
    {
        public static readonly ShaderFactory Texture = new TextureShaderFactory();
        public static readonly ShaderFactory Lighting = new LightingShaderFactory();
        public static readonly ShaderFactory Line = new LineShaderFactory();
        public static readonly ShaderFactory Default = new();

        internal const string Frag =
            "#version 330 core\n" +
            "out vec4 FragColor;\n" +

            "uniform vec4 color;\n" +

            "void main()\n" +
            "{\n" +
            "   FragColor = color;\n" +
            "}";

        internal const string Vert =
            "#version 330 core\n" +
            "layout (location = 0) in vec3 aPos;\n" +

            "uniform mat4 model;\n" +
            "uniform mat4 view;\n" +
            "uniform mat4 projection;\n" +

            "void main()\n" +
            "{\n" +
            "   gl_Position = vec4(aPos, 1.0) * model * view * projection;\n" +
            "}";


        public virtual Shader Create()
        {
            return new(Vert, Frag);
        }
    }

    internal sealed class TextureShaderFactory : ShaderFactory
    {
        private const string TextureFrag =
            "#version 330\n" +

            "out vec4 outputColor;\n" +

            "in vec2 texCoord;\n" +

            "uniform vec4 color;\n" +
            "uniform sampler2D texture;\n" +

            "void main()\n" +
            "{\n" +
            // texture2Dは330で非推奨
            "   outputColor = texture2D(texture, texCoord) * color;\n" +
            "}";
        private const string TextureVert =
            "#version 330 core\n" +

            "layout(location = 0) in vec3 aPosition;\n" +
            "layout(location = 1) in vec2 aTexCoord;\n" +

            "out vec2 texCoord;\n" +
            "uniform mat4 model;\n" +
            "uniform mat4 view;\n" +
            "uniform mat4 projection;\n" +

            "void main(void)\n" +
            "{\n" +
            "   texCoord = aTexCoord;\n" +
            "   gl_Position = vec4(aPosition, 1.0) * model * view * projection;\n" +
            "}";

        public override Shader Create()
        {
            return new(TextureVert, TextureFrag);
        }
    }
    internal sealed class LineShaderFactory : ShaderFactory
    {
        private const string LineVert =
              "#version 330 core\n" +
              "layout (location = 0) in vec3 aPos;\n" +

              "uniform mat4 model;\n" +
              "uniform mat4 view;\n" +
              "uniform mat4 projection;\n" +

              "void main()\n" +
              "{\n" +
              "   gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0) * model * view * projection;\n" +
              "}";

        private const string LineFrag =
            "#version 330 core\n" +

            "out vec4 FragColor;\n" +
            "uniform vec4 color;\n" +

            "void main()\n" +
            "{\n" +
            "   FragColor = color;\n" +
            "}";

        public override Shader Create()
        {
            return new(LineVert, LineFrag);
        }
    }
    internal sealed class LightingShaderFactory : ShaderFactory
    {
        private const string LightFrag =
            "#version 330 core\n" +
            "out vec4 FragColor;\n" +

            "uniform vec4 objectColor;\n" +
            "uniform vec4 lightColor;\n" +
            "uniform vec3 lightPos;\n" +
            "uniform vec3 viewPos;\n" +

            "in vec3 Normal;\n" +
            "in vec3 FragPos;\n" +

            "void main()\n" +
            "{\n" +
            "    float ambientStrength = 0.1;\n" +
            "    vec4 ambient = ambientStrength * lightColor;\n" +

            "    vec3 norm = normalize(Normal);\n" +
            "    vec3 lightDir = normalize(lightPos - FragPos);\n" +

            "    float diff = max(dot(norm, lightDir), 0.0);\n" +
            "    vec4 diffuse = diff * lightColor;\n" +


            "    float specularStrength = 0.5;\n" +
            "    vec3 viewDir = normalize(viewPos - FragPos);\n" +
            "    vec3 reflectDir = reflect(-lightDir, norm);\n" +
            "    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);\n" +
            "    vec4 specular = specularStrength * spec * lightColor;\n" +

            "    vec4 result = (ambient + diffuse + specular) * objectColor;\n" +
            "    FragColor = result;\n" +
            "}";
        private const string LightVert =
            "#version 330 core\n" +
            "layout (location = 0) in vec3 aPos;\n" +
            "layout (location = 1) in vec3 aNormal;\n" +

            "uniform mat4 model;\n"+
            "uniform mat4 view;\n"+
            "uniform mat4 projection;\n"+

            "out vec3 Normal;\n"+
            "out vec3 FragPos;\n"+

            "void main()\n"+
            "{\n"+
            "    gl_Position = vec4(aPos, 1.0) * model * view * projection;\n"+
            "    FragPos = vec3(vec4(aPos, 1.0) * model);\n"+
            "    Normal = aNormal * mat3(transpose(inverse(model)));\n"+
            "}";

        public override Shader Create()
        {
            return new(LightVert, LightFrag);
        }
    }
    internal sealed class LampShaderFactory : ShaderFactory
    {
        private const string LampFrag =
            "#version 330 core\n" +
            "out vec4 FragColor;\n" +

            "uniform vec4 color;\n" +

            "void main()\n" +
            "{\n" +
            "   FragColor = color;\n" +
            "}";
        private const string LampVert =
            "#version 330 core\n" +
            "layout (location = 0) in vec3 aPos;\n" +
            "layout (location = 1) in vec3 aNormal;\n" +

            "uniform mat4 model;\n" +
            "uniform mat4 view;\n" +
            "uniform mat4 projection;\n" +

            "out vec3 Normal;\n" +
            "out vec3 FragPos;\n" +

            "void main()\n" +
            "{\n" +
            "    gl_Position = vec4(aPos, 1.0) * model * view * projection;\n" +
            "    FragPos = vec3(vec4(aPos, 1.0) * model);\n" +
            "    Normal = aNormal * mat3(transpose(inverse(model)));\n" +
            "}";

        public override Shader Create()
        {
            return new(LampVert, LampFrag);
        }
    }
}
