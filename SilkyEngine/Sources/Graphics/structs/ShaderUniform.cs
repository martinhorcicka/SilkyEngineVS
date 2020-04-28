namespace SilkyEngine.Sources.Graphics.Structs
{
    public struct ShaderUniform
    {
        private object data;
        public ShaderUniform(object data) => this.data = data;
        public void UpdateData(object data) => this.data = data;
        public object GetData() => data;

    }
}