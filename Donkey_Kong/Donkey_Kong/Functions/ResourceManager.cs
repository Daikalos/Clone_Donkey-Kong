using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Donkey_Kong
{
    static class ResourceManager
    {
        static SortedDictionary<string, Texture2D> myTextures;
        static SortedDictionary<string, SoundEffect> mySoundEffects;
        static SortedDictionary<string, SpriteFont> myFonts;

        public static void Initialize()
        {
            myTextures = new SortedDictionary<string, Texture2D>();
            mySoundEffects = new SortedDictionary<string, SoundEffect>();
            myFonts = new SortedDictionary<string, SpriteFont>();
        }

        public static void AddTexture(string aTextureName, Texture2D aTexture)
        {
            myTextures.Add(aTextureName, aTexture);
        }

        public static void AddFont(string aFontName, SpriteFont aFont)
        {
            myFonts.Add(aFontName, aFont);
        }

        public static void RemoveTexture(string aTextureName)
        {
            myTextures.Remove(aTextureName);
        }

        public static void RemoveFont(string aFontName)
        {
            myFonts.Remove(aFontName);
        }

        public static Texture2D RequestTexture(string aTextureName)
        {
            if (myTextures.ContainsKey(aTextureName)) //Check if list contains the texture
            {
                return myTextures[aTextureName]; //Return texture
            }
            return null; //ERROR
        }

        public static SpriteFont RequestFont(string aFontName)
        {
            if (myFonts.ContainsKey(aFontName))
            {
                return myFonts[aFontName];
            }
            return null;
        }
    }
}
