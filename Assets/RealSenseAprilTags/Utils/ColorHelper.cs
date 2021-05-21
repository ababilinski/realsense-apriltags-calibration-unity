using System;
using System.Collections.Generic;
using UnityEngine;

namespace Babilinapps.RealSenseAprilTags.Utils
{
    public static class ColorHelper
    {
        private static readonly Dictionary<int, Color> _colorById = new Dictionary<int, Color>();

        public static Color PickTextColorBasedOnBgColor(Color bgColor, Color lightColor, Color darkColor)
        {
        //https://stackoverflow.com/questions/3942878/how-to-decide-font-color-in-white-or-black-depending-on-background-color

            if ((bgColor.r * 0.299 + bgColor.g * 0.587 + bgColor.b * 0.114) > 186)
            {
                return darkColor;

            }
            else
            {
               return lightColor;
            }
        }

        public static Color PickTextColorBasedOnBgColor(Color bgColor)
        {
            //https://stackoverflow.com/questions/3942878/how-to-decide-font-color-in-white-or-black-depending-on-background-color

            if ((bgColor.r * 0.299 + bgColor.g * 0.587 + bgColor.b * 0.114) > 186)
            {
                return Color.black;

            }
            else
            {
                return Color.white;
            }
        }
        /// <summary>
        /// Helper to get the unique color based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Color GetColorById(int id)
        {
      
            Color returnColorValue;
        
            if (_colorById.TryGetValue(id, out Color idColor))
            {
                returnColorValue = idColor;
            }
            else
            {
                returnColorValue = Color.HSVToRGB(id / 10.0f % 1.0f, 1, 1);
                _colorById.Add(id, returnColorValue);
            }

           
            return returnColorValue;
        }
    }
}
