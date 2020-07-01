using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OrlaBeat : MonoBehaviour
{
    //variables para controlar los beats por minuto y el tempo 
    public double bpm = 140.0F;
    public float gain = 0.5F;
    public int signatureHi = 4;
    public int signatureLo = 4;
    private double nextTick = 0.0F;
    private double sampleRate = 0.0F;
    private int accent;
    private bool running = false;

    //variables para los clips de audios y los samples 
    public AudioClip kickClip;
    public AudioClip snareClip;
    float[] KickSamples;
    float[] DoubleKickSamples;
    float[] snareSamples;

    float[] currentSamples; // el sample "selecciuonado"

    void Start()
    {
        //inicializamos los valores 
        accent = signatureHi;
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        nextTick = startTick * sampleRate;
        running = true;

        //populamos el sample del single snare
        snareSamples = new float[snareClip.samples * snareClip.channels];
        snareClip.GetData(snareSamples, 0);

        //populamos el sample del single kick 
        KickSamples = new float[kickClip.samples * kickClip.channels];
        kickClip.GetData(KickSamples, 0);

        //double kick
        //para el double kick, sampleamos el kick dos veces con un espacio de tiempo en silencio (ceros) entre los dos.
        var timeBetweenHits = (int)(sampleRate * 0.18f); // la cantidad de silencio (ceros) entre los dos hits. 
        DoubleKickSamples = new float[KickSamples.Length * 2 + timeBetweenHits];//alocamos el array kick * 2 + silencio.
        var n = 0; //con n voy llevando la cuenta de por donde voy en el array. 
        for(int i = 0; i<KickSamples.Length; i++)
        {
            DoubleKickSamples[i] = KickSamples[i]; 
        }
        n += KickSamples.Length; //ya sampleamos el primer kick
        for(int i = 0; i< timeBetweenHits; i++)
        {
            DoubleKickSamples[n + i] = 0; 
        }
        n += timeBetweenHits; // ya agregamos el silencio
        for (int i = 0; i < KickSamples.Length; i++)
        {
            DoubleKickSamples[n + i] = KickSamples[i];
        }

        currentSamples = KickSamples;//empezamos siempre con el sample kick. 
    }

    //tengo que leer mucho mas sobre esta funcion. 
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running) return;

        //miramos en que parte del sample vamos de acuerdo al sample rate y el tiempo de audio actual. 
        int startSample = (int)(AudioSettings.dspTime * sampleRate);
        //BPM esto lo traje del ejemplo de metronomo de UNITY 
        int samplesPerTick =(int)(sampleRate * 60.0F / bpm * 4.0F  / signatureLo);  
       
        for (int i = 0; i<data.Length; i++)
        {
            //nos aseguramos de que no nos saldremos del array del sample actual. 
            var source = (int)Mathf.Min((startSample + i) % samplesPerTick, currentSamples.Length - 1);
            data[i] = currentSamples[source];

            //con esto vamos "contanto las notas" para cambiar el sample actual pero lo siento full hack
            //TODO investigar una mejor manera de hacer esto. 
            //tuc pa tuctuc pa....
            while (startSample + i >= nextTick)
            {
                nextTick += samplesPerTick;
                if (++accent > signatureHi)
                {
                    accent = 1;
                }
      
                if (accent % 2 == 0)
                {
                    currentSamples = snareSamples;
                }
                else
                {
                    if(accent == 3)
                    {
                        Debug.Log("DOUBLE KICK");
                        currentSamples = DoubleKickSamples;
                    }
                    else
                    {
                        currentSamples = KickSamples;
                    }
                }
            }

        }
    }

    //synthethizer example 
    /*
    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * Mathf.PI / sampling_frequency; 
        for(int i = 0; i<data.Length; i+= channels)
        {
            phase += increment; 
            data[i] = (float)(gain * Mathf.Sin((float)phase)); 

            if(channels == 2)
            {
                data[i + 1] = data[i]; 
            }

            if(phase> (Mathf.PI * 2))
            {
                phase = 0.0;
            }
        }
    }
    */

    //metronome example 
    /*
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;

        int n = 0;
        while (n < dataLen)
        {
            float x = gain * amp * Mathf.Sin(phase);
            int i = 0;
            while (i < channels)
            {
                data[n * channels + i] += x;
                i++;
            }
            while (sample + n >= nextTick)
            {
                nextTick += samplesPerTick;
                amp = 1.0F;
                if (++accent > signatureHi)
                {
                    accent = 1; 
                    amp *= 2.0F;
                }
                Debug.Log("Tick: " + accent + "/" + signatureHi);
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
        }
    }
    */
}
