using System.Drawing;

namespace ClothingTracker.Models.Shared
{

    public enum SimpleDiscreteColor
    {
        Black = 1,
        White = 2,
        Grey = 3,
        Blue = 4,
        Green = 5,
        Red = 6,
        Brown = 7,
        Tan = 8,
        Purple = 9,
        Orange = 10,
    }
    public static class SimpleDiscreteColorExtensions
    {
        public static string HexCode(this SimpleDiscreteColor simpleColor)
        {
            switch (simpleColor)
            {
                case SimpleDiscreteColor.Black: return "#000000";
                case SimpleDiscreteColor.White: return "#ffffff";
                case SimpleDiscreteColor.Grey: return "#d3d3d3";
                case SimpleDiscreteColor.Blue: return "#0000ff";
                case SimpleDiscreteColor.Green: return "#00ff00";
                case SimpleDiscreteColor.Red: return "#ff0000";
                case SimpleDiscreteColor.Brown: return "#a52a2a";
                case SimpleDiscreteColor.Tan: return "#d2b48c";
                case SimpleDiscreteColor.Purple: return "#800080";
                case SimpleDiscreteColor.Orange: return "#ffa500";
                default: return "#000000"; // fallback to black if unknown
            }
        }
        public static SimpleDiscreteColor ClosestDiscreteColor(string hexColor)
        {
            Color target = ColorTranslator.FromHtml(hexColor);
            SimpleDiscreteColor closestColor = SimpleDiscreteColor.Black;
            double smallestDistance = double.MaxValue;

            foreach (SimpleDiscreteColor simpleColor in Enum.GetValues(typeof(SimpleDiscreteColor)))
            {
                string hexValue = simpleColor.HexCode();
                Color current = ColorTranslator.FromHtml(hexValue);

                double distance = Math.Pow(current.R - target.R, 2) +
                                  Math.Pow(current.G - target.G, 2) +
                                  Math.Pow(current.B - target.B, 2);

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    closestColor = simpleColor;
                }
            }

            return closestColor;
        }
    }
}
