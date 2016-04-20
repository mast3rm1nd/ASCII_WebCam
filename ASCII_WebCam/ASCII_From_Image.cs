using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using System.Drawing;
using System.IO;

namespace ASCII_WebCam
{
    class ASCII_From_Image
    {
        //private static string chars_by_density = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'. ";
        //private static string chars_by_density = new string("$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'. ".Reverse().ToArray());
        //private static string chars_by_density = new string("$oZQzrjft+~i!l\"^. ".Reverse().ToArray());
        //private static string chars_by_density = "$l^. ";
        private static string chars_by_density = new string("$ft~i!l. ".Reverse().ToArray());

        public static string Get(Bitmap image)
        {
            var result = new StringBuilder();
            
            for(var y = 0; y < image.Height; y++)
            {
                for(var x = 0; x < image.Width; x++)
                {
                    var color = image.GetPixel(x, y);
                    var brightness = Calculate_Brightness_from_Colour(color);
                    var index_of_char = brightness / 255 * (chars_by_density.Length - 1);
                    var char_to_place = chars_by_density[(int)Math.Round(index_of_char)];

                    result.Append(char_to_place);
                    result.Append(char_to_place);
                }
                result.Append(Environment.NewLine);
            }            

            return result.ToString();
        }


        private static double Calculate_Brightness_from_Colour(Color c)
        {
            return (int)Math.Sqrt(
                c.R * c.R * .241 +
                c.G * c.G * .641 +
                c.B * c.B * .068);
        }
    }
}
