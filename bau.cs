using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bau : MonoBehaviour
{
    //private _ControladorDeJogo controlador;

    private SpriteRenderer  renderizador;
    public Sprite[]         imagemObjeto;
    public bool             aberto;

    // Start is called before the first frame update
    void Start()
    {
        //controlador = FindObjectOfType(typeof(_ControladorDeJogo)) as _ControladorDeJogo;
        renderizador = GetComponent<SpriteRenderer>();
    }

    public void interacao()
    {
        if(aberto == false)
        {
            aberto = true;
            renderizador.sprite = imagemObjeto[1];
            GetComponent<Collider2D>().enabled = false;
        }        
    }
}
