using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Diagnostics;

namespace Altaria.Model
{
    public class KeyGen
    {
        int userdata { get; set; }
        int keydata { get; set; }
        Complex[] keydatadata { get; set; }
        int keyframelength { get; set; }
        string keydatainfo { get; set; }

        public KeyGen(int userid, int framelength, int framecount)
        {
            Random random = new Random();
            userdata = userid;
            keydatadata = new Complex[10];
            keyframelength = framelength;
            for (int i = 0; i < keydatadata.Length; i++)
            {
                keydatadata[i] = random.NextDouble() / 100;
            }
        }

        public List<Complex[]> embedWatermark(List<Complex[]> complexframes)
        {

            foreach (var frame in complexframes)
            {
                frame[0] = frame[0] + keydatadata[0];
                frame[1] = frame[1] + keydatadata[1];
                frame[2] = frame[2] + keydatadata[2];
                frame[3] = frame[3] + keydatadata[3];
                frame[4] = frame[4] + keydatadata[4];
                frame[5] = frame[5] + keydatadata[5];
                frame[6] = frame[6] + keydatadata[6];
                frame[7] = frame[7] + keydatadata[7];
                frame[8] = frame[8] + keydatadata[8];
                frame[9] = frame[9] + keydatadata[9];
                Transform.FourierInverse(frame);
            } 
            return complexframes;

        }
    }
}