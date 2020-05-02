using System.Numerics;

namespace SilkyEngine.Sources.Graphics.Structs
{
    public struct LightStruct
    {
        public string Name { get; }
        public Vector3 Position { get; set; }
        public Vector3 Ambient { get; set; }
        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }

        public float constant, linear, quadratic;

        public LightStruct(string name, Vector3 position, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic)
        {
            Name = name;
            Position = position;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            this.constant = constant;
            this.linear = linear;
            this.quadratic = quadratic;
        }

        public LightStruct NewNameCopy(string newName)
        {
            return new LightStruct(newName, Position, Ambient, Diffuse, Specular, constant, linear, quadratic);
        }
    }
}