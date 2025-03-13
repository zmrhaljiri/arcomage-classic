using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static void SetGraphicAlpha(Graphic graphic, float alpha)
    {
        if (graphic != null)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }

    public static Vector2 GetRandomOffset()
    {
        float randomOffset = Random.Range(-10, 10);

        return new Vector2(randomOffset, randomOffset);
    }
}
