using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace ArarGames.Core.Audio
{
    public static class SoundManager
    {
        private static Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Song> songs = new Dictionary<string, Song>();

        public static float SoundVolume { get; set; } = 1.0f;
        public static float MusicVolume { get; set; } = 1.0f;

        public static void AddSound(string name, SoundEffect sound)
        {
            soundEffects[name] = sound;
        }

        public static void AddSong(string name, Song song)
        {
            songs[name] = song;
        }

        public static void PlaySound(string name)
        {
            if (soundEffects.TryGetValue(name, out var sound))
            {
                sound.Play(SoundVolume, 0f, 0f);
            }
        }

        public static void PlayMusic(string name, bool isRepeating = true)
        {
            if (songs.TryGetValue(name, out var song))
            {
                MediaPlayer.Volume = MusicVolume;
                MediaPlayer.IsRepeating = isRepeating;
                MediaPlayer.Play(song);
            }
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }
    }
}
