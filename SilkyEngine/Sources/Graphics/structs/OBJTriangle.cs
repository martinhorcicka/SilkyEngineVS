namespace SilkyEngine.Sources.Graphics.Structs
{
    public struct OBJTriangle
    {
        public int[] v1, v2, v3;

        public OBJTriangle(string objTriangle)
        {
            string[] verts = objTriangle.Split(' ');
            v1 = manageVert(verts[1]);
            v2 = manageVert(verts[2]);
            v3 = manageVert(verts[3]);
        }

        public OBJTriangle(int[] v1, int[] v2, int[] v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        private static int[] manageVert(string vert)
        {
            string[] nums = vert.Split('/');
            return new int[3] { int.Parse(nums[0]) - 1, int.Parse(nums[1]) - 1, int.Parse(nums[2]) - 1 };
        }
    }
}