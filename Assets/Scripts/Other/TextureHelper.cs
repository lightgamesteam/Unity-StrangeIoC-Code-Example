using UnityEngine;

public static class TextureHelper
{
    /// <summary>
    /// Check download image (is error image)
    /// </summary>
    /// <param name="myTexture"></param>
    /// <returns></returns>
    public static bool IsDownloadImageError(this Texture2D myTexture)
    {
        //error image size 8x8
        return myTexture.width == 8 && myTexture.height == 8;
    }
}
