using System.Numerics;

namespace SilkyEngine.Sources.Graphics.Structs
{
    public struct LightStruct
    {
        public string name { get; }
        public Vector3 position { get; set; }
        public Vector3 ambient { get; set; }
        public Vector3 diffuse { get; set; }
        public Vector3 specular { get; set; }

        public float constant, linear, quadratic;

        public LightStruct(string name, Vector3 position, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic)
        {
            this.name = name;
            this.position = position;
            this.ambient = ambient;
            this.diffuse = diffuse;
            this.specular = specular;
            this.constant = constant;
            this.linear = linear;
            this.quadratic = quadratic;
        }

        public LightStruct NewNameCopy(string newName)
        {
            return new LightStruct(newName, position, ambient, diffuse, specular, constant, linear, quadratic);
        }
    }
}