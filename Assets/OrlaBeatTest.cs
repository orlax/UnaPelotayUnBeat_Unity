using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OrlaBeatTest : MonoBehaviour
{
    public AudioClip audioclip;
    float[] audioClipSamples;
    double sampleRate = 0.0f;
    bool running = false;
    double nextTick = 0.0F;
    public float bpm = 120; 

    private void Start()
    {
        audioClipSamples = new float[audioclip.samples * audioclip.channels];
        audioclip.GetData(audioClipSamples, 0);
        sampleRate = AudioSettings.outputSampleRate;
        running = true; 
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running) return;

        int start = (int)(sampleRate * AudioSettings.dspTime);
        int samplesPerTick = (int)(sampleRate * 60.0F / bpm * 4.0F / 4);
        for (int i = 0; i < data.Length; i++)
        {
            var source = (int)Mathf.Min( (start+i)%samplesPerTick, audioClipSamples.Length - 1);


            data[i] = (float)audioClipSamples[source];

            while (start + i >= nextTick)
            {
                nextTick += samplesPerTick;
            }
        }
        
    }

}
