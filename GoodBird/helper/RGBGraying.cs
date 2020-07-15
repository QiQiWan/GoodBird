using System;
using System.Drawing;

namespace GoodBird.Helper
{
    /// <summary>
    /// 图片处理类型
    /// </summary>
    public class RGBGraying
    {
        #region GET the background
        public static Bitmap removeBG(Image image)
        {
            return removeBG(new Bitmap(image));
        }
        public static Bitmap removeBG(Bitmap image)
        {
            image = changeColor(image, getBGArr(image, Direction.DownWard), Direction.DownWard, Color.White);
            return image;
        }
        public static int[] getBGArr(Bitmap image, Direction direction)
        {
            int width = image.Width, height = image.Height;
            int iEnd, jStart, jEnd;
            int trans = (direction == Direction.DownWard || direction == Direction.UpWard) ? 0 : 1;
            int[] BGArr;
            if (trans == 0)
            {
                iEnd = width;
                BGArr = new int[width];
                if (direction == Direction.DownWard)
                {
                    jStart = 1;
                    jEnd = height - 3;
                }
                else
                {
                    jStart = height - 3;
                    jEnd = 1;
                }
            }
            else
            {
                iEnd = height;
                BGArr = new int[height];
                if (direction == Direction.RightWard)
                {
                    jStart = 1;
                    jEnd = width - 3;
                }
                else
                {
                    jStart = width - 3;
                    jEnd = 1;
                }
            }
            for (int i = 0; i < iEnd; i++)
            {
                BGArr[i] = 0;
                int j = jStart;
                while (j == jEnd)
                {
                    int dire = (jEnd - jStart) / Math.Abs(jEnd - jStart);
                    int current = GetGrayLevel(image.GetPixel(i, j));
                    int next = GetGrayLevel(image.GetPixel(i, j + dire));
                    int afterNext = GetGrayLevel(image.GetPixel(i, j + 2 * dire));
                    int last = GetGrayLevel(image.GetPixel(i, j - dire));
                    if ((next - current) > (Math.Abs(current - last) + Math.Abs(afterNext - next)))
                    {
                        BGArr[i] = j;
                        break;
                    }
                    j = dire + j;
                }
            }
            return BGArr;
        }
        public static Bitmap changeColor(Bitmap image, int[] BGArr, Direction direction, Color color)
        {
            int width = image.Width, height = image.Height;
            int iEnd, jEnd;
            int iTrans = (direction == Direction.DownWard || direction == Direction.UpWard) ? 0 : 1;
            int jTrans = (direction == Direction.DownWard || direction == Direction.RightWard) ? 0 : 1;
            if (iTrans == 0)
                iEnd = width;
            else
                iEnd = height;
            if (jTrans == 0)
            {
                for (int i = 0; i < iEnd; i++)
                {
                    int j = 0;
                    while (j < BGArr[i])
                    {
                        if (iTrans == 0)
                            image.SetPixel(i, j, color);
                        else
                            image.SetPixel(j, i, color);
                        j++;
                    }
                }
            }
            else
            {
                jEnd = direction == Direction.UpWard ? height : width;
                for (int i = 0; i < iEnd; i++)
                {
                    int j = BGArr[i];
                    while (j < jEnd)
                    {
                        if (iTrans == 0)
                            image.SetPixel(i, j, color);
                        else
                            image.SetPixel(j, i, color);
                        j++;
                    }
                }
            }
            return image;
        }
        public enum Direction { UpWard, DownWard, LeftWard, RightWard };
        #endregion

        /// <summary>
        /// 获取图片的平均灰度
        /// </summary>
        /// <param name="grayBitmap"></param>
        /// <returns></returns>
        public static int GetAverageGrayLevel(Bitmap grayBitmap)
        {
            //if the image which as argument is set into function is not gray image, return exception;
            if (!isGrayImage(grayBitmap))
                throw new Exception("不是灰度图片!");
            int size = grayBitmap.Width * grayBitmap.Height;
            int s = 0;

            byte[] rgbValues = ImageHelper.GetImgArr(grayBitmap);

            for (int i = 0; i < (rgbValues.Length / 3); i++)
            {
                s += rgbValues[3 * i];
            }
            return s / size;
        }
        /// <summary>
        /// 获取该像素的灰度值
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int GetGrayLevel(Color color)
        {
            if (isGray(color))
                return color.R;
            else
                return -1;
        }
        /// <summary>
        /// to define the gray color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool isGray(Color color)
        {
            if (color.R == color.G && color.R == color.B)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 灰度化像素
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int GetGrayBit(Color color)
        {
            if (isGray(color))
                return color.R;
            return (int)Math.Round((double)((int)color.R * 299 + (int)color.G * 587 + (int)color.B * 114 + 500) / 1000, 0);
        }
        /// <summary>
        /// 灰度化并返回图片
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap GetGrayImage(Bitmap image)
        {
            byte[] rgbValues = ImageHelper.GetImgArr(image);

            for (int i = 0; i < (rgbValues.Length / 3); i++)
            {
                Color temp = Color.FromArgb(rgbValues[3 * i], rgbValues[3 * i + 1], rgbValues[3 * i + 2]);
                int gray = GetGrayBit(temp);
                rgbValues[3 * i] = rgbValues[3 * i + 1] = rgbValues[3 * i + 2] = (byte)gray;
            }
            image = ImageHelper.WriteImg(rgbValues, image.Width, image.Height);
            return image;
        }
        /// <summary>
        /// 根据传入分界点二值化图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Bitmap Get2BWImage(Bitmap image, int level)
        {
            if (!isGrayImage(image))
                image = GetGrayImage(image);

            byte[] rgbValues = ImageHelper.GetImgArr(image);

            for (int i = 0; i < (rgbValues.Length / 3); i++)
            {
                rgbValues[3 * i] = rgbValues[3 * i + 1] = rgbValues[3 * i + 2] = (byte)(rgbValues[3 * i] >= level ? 255 : 0);
            }
            image = ImageHelper.WriteImg(rgbValues, image.Width, image.Height);
            return image;
        }
        /// <summary>
        /// 根据平均灰度二值化图片
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap GetAve2BWImage(Bitmap image)
        {
            int level = GetAverageGrayLevel(image);

            image = Get2BWImage(image, level);

            return image;
        }
        static public bool isGrayImage(Bitmap bitmap) => isGray(bitmap.GetPixel(new Random().Next(0, bitmap.Width), new Random().Next(0, bitmap.Height)));
    }
}