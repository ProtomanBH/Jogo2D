using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleDanoInimigo : MonoBehaviour
{
    private     _ControladorDeJogo  controlador;
    private     playerScript        script_do_jogador;
    private     SpriteRenderer      renderizador_de_Sprites;
    private     Animator            animator;
    private     controladorAudio    controladorAudio;

    [Header("Configuração pontos de vida")]
    public      int                 pvInimigo;
    public      int                 pvAtual;
    public      GameObject          barrasPV; // Objeto que contem todas as barras de PV
    public      Transform           barraAtual; // Indica a quantidade atual dos pontos de vida do inimigo
    private     float               porcVida; // controla a porcentagem dos pontos de vida
    public      GameObject          informaTXTDanoPrefab; // objeto que mostra o dano na tela

    [Header("Configuração de resistencias e fraquezas")]
    public      float[]             ajusteDano; //Modificador de dano por tipo de ataque
    public      bool                olhandoEsquerda, jogadorEsquerda;

    [Header("Configuração de repulsão")]
    public      GameObject          Prefab_forca_de_repulsao; // Empurra os inimigos
    public      Transform           origem_da_repulsao; // onde comeca a forca de repulsão
    public      float               repulsao_x; // Valor padrão da posição x
    private     float               kx;
    private     bool                informaDano; //informa se o personagem tomou dano
    public      Color[]             cor_do_personagem;
    private     bool                morreu; // Indica se o inimigo foi derrotado

    [Header("Configuração de Chão")]
    public      Transform           groundCheck;
    public      LayerMask           estarNoChao;
    // Start is called before the first frame update
    void Start()
    {
        controlador = FindObjectOfType(typeof(_ControladorDeJogo)) as _ControladorDeJogo;
        script_do_jogador = FindObjectOfType(typeof(playerScript)) as playerScript;

        renderizador_de_Sprites = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        renderizador_de_Sprites.color = cor_do_personagem[0];
        barrasPV.SetActive(false);
        pvAtual = pvInimigo;
        barraAtual.localScale = new Vector3(1, 1, 1);

        if (olhandoEsquerda == true)
        {            
            float x = transform.localScale.x;
            x *= -1;    //Inverte o sinal do X
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
            barrasPV.transform.localScale = new Vector3(x, barrasPV.transform.localScale.y, barrasPV.transform.localScale.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Verifica a posição do jogador em relação ao inimigo.
        float posicao_x_jogador = script_do_jogador.transform.position.x;

        if(posicao_x_jogador < transform.position.x)
        {
            jogadorEsquerda = true;            
        }
        else if(posicao_x_jogador > transform.position.x)
        {
            jogadorEsquerda = false;          
        }

        if(olhandoEsquerda == true && jogadorEsquerda == true)
        {
            kx = repulsao_x;
        }

        else if(olhandoEsquerda == false && jogadorEsquerda == true)
        {
            kx = repulsao_x * -1;
        }

        else if(olhandoEsquerda == true && jogadorEsquerda == false)
        {
            kx = repulsao_x * -1;
        }

        else if (olhandoEsquerda == false && jogadorEsquerda == false)
        {
            kx = repulsao_x;
        }

        origem_da_repulsao.localPosition = new Vector3(kx, origem_da_repulsao.localPosition.y, 0);

        animator.SetBool("grounded", true);
        //controladorAudio.tocar_efeito(controladorAudio.som_dano, 1);
    }

    private void OnTriggerEnter2D(Collider2D colisao)
    { 
        switch (colisao.gameObject.tag)
        {
            case "arma":

                if (informaDano == false)
                {
                    informaDano = true;
                    barrasPV.SetActive(true);

                    armaInfo caracteristicas_da_arma = colisao.gameObject.GetComponent<armaInfo>();

                    animator.SetTrigger("hit");
                    
                    float danoArma = Random.Range(caracteristicas_da_arma.danoMin, caracteristicas_da_arma.danoMax);
                    int tipoDano = caracteristicas_da_arma.tipoDano;

                    float danoTomado = danoArma + (danoArma * (ajusteDano[tipoDano] / 100));

                    pvAtual -= Mathf.RoundToInt(danoTomado); // Reduz os pontos de vida do inimigo

                    porcVida = (float)pvAtual / (float)pvInimigo; // Calcula a porcentagem de vida

                    if(porcVida < 0)
                    {
                        porcVida = 0;                        
                    }

                    barraAtual.localScale = new Vector3(porcVida, 1, 1);

                    if (pvAtual <= 0)
                    {                        
                        animator.SetTrigger("morreu");
                        Destroy(this.gameObject, 2);
                    }

                    GameObject dano_temporario = Instantiate(informaTXTDanoPrefab, transform.position, transform.localRotation);
                    dano_temporario.GetComponent<TextMesh>().text = Mathf.RoundToInt(danoTomado).ToString();
                    dano_temporario.GetComponent<MeshRenderer>().sortingLayerName = "HUD";

                    GameObject efeito_temporario = Instantiate(controlador.efeitos_dano[tipoDano], transform.position, transform.localRotation);
                    Destroy(efeito_temporario, 1);

                    int forca_eixo_X = 10;
                    if(jogadorEsquerda == false)
                    {
                        forca_eixo_X *= -1;
                    }

                    dano_temporario.GetComponent<Rigidbody2D>().AddForce(new Vector2(forca_eixo_X, 200));
                    Destroy(dano_temporario, 0.4f);

                    GameObject empurrao_temporario = Instantiate(Prefab_forca_de_repulsao, origem_da_repulsao.position, origem_da_repulsao.localRotation);
                    Destroy(empurrao_temporario, 0.02f); // destroi a variavel para que outros objetos do jogo não sejam empurrados por ela)

                    StartCoroutine("invulneravel");                   
                }
                break;
        }
    }

    void flip()
    {
        olhandoEsquerda = !olhandoEsquerda; //inverte o valor da variável booleana.
        float x = transform.localScale.x;
        x *= -1;    //Inverte o sinal do X
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        barrasPV.transform.localScale = new Vector3(x, barrasPV.transform.localScale.y, barrasPV.transform.localScale.z);
    }

    IEnumerator loot()
    {
        yield return new WaitForSeconds(2);
       // Sera feito mais adiante
    }

    IEnumerator invulneravel()
    {
        renderizador_de_Sprites.color = cor_do_personagem[1];
        yield return new WaitForSeconds(0.1f);
        renderizador_de_Sprites.color = cor_do_personagem[0];
        yield return new WaitForSeconds(0.1f);
        renderizador_de_Sprites.color = cor_do_personagem[1];
        yield return new WaitForSeconds(0.1f);
        renderizador_de_Sprites.color = cor_do_personagem[0];
        yield return new WaitForSeconds(0.1f);
        renderizador_de_Sprites.color = cor_do_personagem[1];
        yield return new WaitForSeconds(0.1f);
        renderizador_de_Sprites.color = cor_do_personagem[0];
        yield return new WaitForSeconds(0.1f);
        renderizador_de_Sprites.color = cor_do_personagem[1];
        yield return new WaitForSeconds(0.1f);
        renderizador_de_Sprites.color = cor_do_personagem[0];
        yield return new WaitForSeconds(0.1f);
        renderizador_de_Sprites.color = cor_do_personagem[1];
        yield return new WaitForSeconds(0.1f);
        renderizador_de_Sprites.color = cor_do_personagem[0];
        yield return new WaitForSeconds(0.1f);


        informaDano = false;
        barrasPV.SetActive(false);

    }
}
