using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DadosTela : MonoBehaviour
{
    private     playerScript        script_do_jogador;
    public      Transform           barra_energia;
    private     float               porcVida;

    // Start is called before the first frame update
    void Start()
    {
        script_do_jogador = FindObjectOfType(typeof(playerScript)) as playerScript;
        barra_energia.localScale = new Vector3(1, 1, 1);
       
    }

    // Update is called once per frame
    void Update()
    {
        controleBarraPV();
    }

    void controleBarraPV()
    {
        float porcVida = (float)script_do_jogador.pvAtual / (float)script_do_jogador.pvMax;

        barra_energia.localScale = new Vector3(porcVida, 1, 1);
    }

}
