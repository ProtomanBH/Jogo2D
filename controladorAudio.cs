using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class controladorAudio : MonoBehaviour
{
    public AudioSource musica;
    public AudioSource efeitos_1;
    //public AudioSource efeitos_2;
    //public AudioSource efeitos_3;

    public AudioClip musica_jogo;
    public AudioClip som_ataque;
    public AudioClip som_dano;
    public AudioClip som_andar;

    private float volumeMaximoMusica;
    private float volumeMaximoEfeitos;

    private AudioClip novaMusica;
    private string novaCena;
    private bool trocarCena;



    // Start is called before the first frame update
    void Start()
    {
        volumeMaximoMusica = 0.2f;
        volumeMaximoEfeitos = 0.5f;

        musica.clip = musica_jogo;
        musica.volume = volumeMaximoMusica;

        //efeitos_1.clip = som_ataque;
        //efeitos_1.volume = volumeMaximoEfeitos;

        //efeitos_2.clip = som_dano;
        //efeitos_2.volume = volumeMaximoEfeitos;

        //efeitos_3.clip = som_andar;
        //efeitos_3.volume = volumeMaximoEfeitos;

        musica.Play();
       //efeitos_1.Play();
    }

    public void tocar_efeito(AudioClip som, float volume)
    {
        float tempVolume = volume;
        if(volume > volumeMaximoEfeitos)
        {
            tempVolume = volumeMaximoEfeitos;
        }
        efeitos_1.volume = tempVolume;
        efeitos_1.PlayOneShot(som);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
