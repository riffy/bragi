using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRAGI.Util;

public class OdinWaveProvider32 : WaveProvider32
{
    public OdinNative.Odin.Media.PlaybackStream Media { get; private set; }
    public bool ValidFormat { get { return WaveFormat!.SampleRate == 48000 && WaveFormat.Channels == 1; } }

    public OdinWaveProvider32(OdinNative.Odin.Media.PlaybackStream media)
    {
        Media = media;
    }

    public override int Read(float[] buffer, int offset, int sampleCount)
    {
        var result = Media?.AudioReadData(buffer, sampleCount) ?? 0;
        if (result <= 0) return 0; // AudioReadData returns an error code on failure so make it silence
        return (int)result;
    }
}

public abstract class WaveProvider32 : IWaveProvider
{
    private WaveFormat? waveFormat;
    public WaveFormat? WaveFormat
    {
        get { return waveFormat; }
    }
    public WaveProvider32()
        : this(48000, 1)
    {
    }

    public WaveProvider32(int sampleRate, int channels)
    {
        SetWaveFormat(sampleRate, channels);
    }

    public void SetWaveFormat(int sampleRate, int channels)
    {
        waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        WaveBuffer waveBuffer = new WaveBuffer(buffer);
        int samplesRequired = count / 4;
        int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
        return samplesRead * 4;
    }

    public abstract int Read(float[] buffer, int offset, int sampleCount);
}