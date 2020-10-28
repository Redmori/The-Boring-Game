using SFML.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
{
    class SoundManager
    {

        public static string path = "../../../Content/";
        public static SoundBuffer error_snd = new SoundBuffer(path + "Error.wav");
        public static Sound errorSound;


        public static void PlayErrorSound()
        {            
            errorSound.Play();
        }

        public static void LoadSounds()
        {
            errorSound = new Sound(error_snd);
            errorSound.Volume = 30f;
        }

    }
}
