using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Donkey_Kong
{
    static class ResourceManager
    {
        static SortedDictionary<string, SpriteFont> myFonts;
        static SortedDictionary<string, SoundEffectInstance> mySEInstances; //SoundEffect
        static SortedDictionary<string, Texture2D> myTextures;

        public static void Initialize()
        {
            myTextures = new SortedDictionary<string, Texture2D>();
            mySEInstances = new SortedDictionary<string, SoundEffectInstance>();
            myFonts = new SortedDictionary<string, SpriteFont>();
        }

        public static void AddFont(string aFontName, SpriteFont aFont)
        {
            myFonts.Add(aFontName, aFont);
        }
        public static void AddTexture(string aTextureName, Texture2D aTexture)
        {
            myTextures.Add(aTextureName, aTexture);
        }
        public static void AddSound(string aSoundName, SoundEffect aSoundEffect)
        {
            mySEInstances.Add(aSoundName, aSoundEffect.CreateInstance());
        }

        public static void RemoveFont(string aFontName)
        {
            myFonts.Remove(aFontName);
        }
        public static void RemoveTexture(string aTextureName)
        {
            myTextures.Remove(aTextureName);
        }
        public static void RemoveSound(string aSoundName)
        {
            mySEInstances.Remove(aSoundName);
        }

        public static SpriteFont RequestFont(string aFontName)
        {
            if (myFonts.ContainsKey(aFontName))
            {
                return myFonts[aFontName];
            }
            return null;
        }
        public static Texture2D RequestTexture(string aTextureName)
        {
            if (myTextures.ContainsKey(aTextureName)) //Check if list contains the texture
            {
                return myTextures[aTextureName]; //Return texture
            }
            return null; //ERROR
        }
        public static SoundEffectInstance RequestSoundEffect(string aSoundName)
        {
            if (mySEInstances.ContainsKey(aSoundName))
            {
                return mySEInstances[aSoundName];
            }
            return null; //ERROR
        }

        public static void PlaySound(string aSoundName)
        {
            if (mySEInstances.ContainsKey(aSoundName))
            {
                if (mySEInstances[aSoundName].State != SoundState.Playing)
                {
                    mySEInstances[aSoundName].Play();
                }
            }
        }
        public static void StopSound(string aSoundName)
        {
            if (mySEInstances.ContainsKey(aSoundName))
            {
                if (mySEInstances[aSoundName].State == SoundState.Playing)
                {
                    mySEInstances[aSoundName].Stop();
                }
            }
        }
    }
}
