namespace SilkyEngine.Sources.Graphics
{
    public class ImgData
    {
        private byte[,] data;
        private int width, height;
        public int Width => width;
        public int Height => height;
        public ImgData(byte[,] data)
        {
            this.data = data;
            width = data.GetLength(0);
            height = data.GetLength(1);
        }
        public byte this[int i, int j]
        {
            get
            {
                if (i < 0 || i >= width || j < 0 || j >= height)
                {
                    if (i < 0 && j < 0)
                        return data[0, 0];
                    if (i < 0 && j < height)
                        return data[0, j];
                    if (i < 0)
                        return data[0, height - 1];
                    if (j < 0 && i < width)
                        return data[i, 0];
                    if (j < 0)
                        return data[width - 1, 0];
                    if (i >= width && j < height)
                        return data[width - 1, j];
                    if (i >= width)
                        return data[width - 1, height - 1];
                        
                    return data[i, height - 1];
                }
                return data[i, j];
            }
            set => data[i, j] = value;

        }
    }
}