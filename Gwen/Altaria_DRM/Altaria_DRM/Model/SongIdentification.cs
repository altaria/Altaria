using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Altaria_DRM.Model
{
    public class SongIdentification
    {
        private SongRetrieval identifiedSong;
        private int accuracy;
        private int maxConsecutiveHashes;
        private string misc;

        public void setIdentifiedSong(SongRetrieval identifiedSong)
        {
            this.identifiedSong = identifiedSong;
        }

        public SongRetrieval getIdentifiedSong()
        {
            return identifiedSong;
        }

        public void setAccuracy(int accuracy)
        {
            this.accuracy = accuracy;
        }

        public int getAccuracy()
        {
            return accuracy;
        }

        public void setMaxConsecutiveHashes(int maxConsecutiveHashes)
        {
            this.maxConsecutiveHashes = maxConsecutiveHashes;
        }

        public int getMaxConsecutiveHashes()
        {
            return maxConsecutiveHashes;
        }

        public void setMisc(string misc)
        {
            this.misc = misc;
        }

        public string getMisc()
        {
            return misc;
        }

        public SongIdentification(SongRetrieval identifiedSong, int accuracy, int maxConsecutiveHashes, string misc)
        {
            this.identifiedSong = identifiedSong;
            this.accuracy = accuracy;
            this.maxConsecutiveHashes = maxConsecutiveHashes;
            this.misc = misc;
        }
    }
}