using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radio
{
    class MyWavReader
    {
        const int WAV_HEADER_SIZE = 43;
        const int WAV_NUMCHANNELS_OFFSET = 22;
        const int WAV_NUMCHANNELS_SIZE = 2;
        const int WAV_SAMPLERATE_OFFSET = 24;
        const int WAV_SAMPLERATE_SIZE = 4;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Mp3WaveProviderDebug");

        static public int WavHeaderSize()
        {
            return WAV_HEADER_SIZE;
        }

        static public int GetNumChannelFromHeader(byte[] header)
        {
            if (header.Length < (WAV_NUMCHANNELS_OFFSET + WAV_NUMCHANNELS_SIZE))
            {
                throw new ArgumentException("header", "header is incomplete");
            }

            byte[] numOfChannelsBytes = new byte[WAV_NUMCHANNELS_SIZE];
            Array.Copy(header, WAV_NUMCHANNELS_OFFSET, numOfChannelsBytes, 0, WAV_NUMCHANNELS_SIZE);
            int numOfChannels = BitConverter.ToInt16(numOfChannelsBytes, 0);
            Logger.Debug("num of channels: {0}", numOfChannels);

            return numOfChannels;
        }

        static public int GetSampleRateFromHeader(byte[] header)
        {
            if (header.Length < (WAV_SAMPLERATE_OFFSET + WAV_SAMPLERATE_SIZE))
            {
                throw new ArgumentException("header", "header is incomplete");
            }

            byte[] sampleRateBytes = new byte[WAV_SAMPLERATE_SIZE];
            Array.Copy(header, WAV_SAMPLERATE_OFFSET, sampleRateBytes, 0, WAV_SAMPLERATE_SIZE);
            int sampleRate = BitConverter.ToInt32(sampleRateBytes, 0);
            Logger.Debug("sample rate: {0}", sampleRate);

            return sampleRate;
        }
    }
}
