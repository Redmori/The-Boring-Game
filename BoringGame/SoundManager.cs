using SFML.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoringGame
{
    class SoundManager
    {

        public static SoundBuffer error_snd = new SoundBuffer("../../Content/error.wav");
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
