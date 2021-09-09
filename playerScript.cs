using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour { 

    private     Animator            playerAnimator;
    private     Rigidbody2D         playerRb;
    private     controladorAudio    controladorAudio;
    private     _ControladorDeJogo  controlador;
    private     float               h, v;

    public      BarraPV             barraPV;
    public      Transform           groundCheck;  // objeto responsável por detectar se o personagem está sobre uma superfície.
    public      Collider2D          em_pe, abaixado;    // Colisor em pé e abaixado
    public      GameObject          informaTXTDanoPrefab;
    public      LayerMask           estaNoChao;     // Indica o que é superfície para o teste

    public      float               pvMax, pvAtual;
    public      float               speed;        //velocidade de movimento do personagem
    public      float               jumpForce;    //força aplicada para gerar o pulo do personagem    
    public      int                 idAnimation;  //Indica o ID da animação
    public      bool                olhandoEsquerda;    //Indica se o personagem tá virado pra esquerda.
    public      bool                personagem_atacando;     // Indica se o Personagem está executando um ataque
    public      bool                Grounded;     //Indica se o personagem está pisando em alguma superficie

    // Interação com Itens e Objetos
    public      Transform           mao;
    private     Vector3             direcao = Vector3.right;
    public      LayerMask           interacao;
    public      GameObject          objetoInteracao;
    public      GameObject          iconeAlerta;

    //SISTEMAS DE ARMAS
    public      GameObject[]     armas;

    private bool informaDano;
    public bool  jogadorEsquerda;

    // Start is called before the first frame update
    void Start()
    {
        controladorAudio = FindObjectOfType(typeof(controladorAudio)) as controladorAudio;
        playerRb = GetComponent<Rigidbody2D>(); //Associa o componente a variável
        playerAnimator = GetComponent<Animator>();

        pvAtual = pvMax;
        barraPV.definePVAtual(pvAtual);

        foreach(GameObject equipamento in armas)
        {
            equipamento.SetActive(false);
        }        
    }

    void FixedUpdate() // taxa de atualização fixa de 0.02
    {
        Grounded = Physics2D.OverlapCircle(groundCheck.position, 0.02f, estaNoChao);
        playerRb.velocity = new Vector2(h * speed, playerRb.velocity.y);

        interagir();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if(h > 0 && olhandoEsquerda == true && personagem_atacando == false)
        {
            flip();
        }
        else if(h < 0 && olhandoEsquerda == false && personagem_atacando == false)
        {
            flip();
        }

        if (v < 0)
        {
            idAnimation = 2;
            if(Grounded == true)
            {
                h = 0;
            }            
        }

        else if (h != 0)
            {
            idAnimation = 1;
            }
        else
            {
            idAnimation = 0;
            }

        if (Input.GetButtonDown("Fire1") && v >= 0 && personagem_atacando == false && objetoInteracao == null)
        {
            playerAnimator.SetTrigger("atack");
        }

        if (Input.GetButtonDown("Fire1") && v >= 0 && personagem_atacando == false && objetoInteracao != null)
        {
            objetoInteracao.SendMessage("interacao", SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetButtonDown("Jump") && Grounded == true && personagem_atacando == false)
        {
            playerRb.AddForce(new Vector2(0, jumpForce));
            
        }

        if(personagem_atacando == true && Grounded == true)
        {
            h = 0;
        }

        if( v < 0 && Grounded == true)
        {
            abaixado.enabled = true;
            em_pe.enabled = false;
        }
        else if(v >= 0 && Grounded == true)
        {
            abaixado.enabled = false;
            em_pe.enabled = true;
        }
        else if(v != 0 && Grounded == false)
        {
            abaixado.enabled = false;
            em_pe.enabled = true;
        }
        //print(h);
        playerAnimator.SetBool("grounded", Grounded);
        playerAnimator.SetInteger("idAnimation", idAnimation);
        playerAnimator.SetFloat("speedY", playerRb.velocity.y);
        
    }
    
    void flip()
    {
        olhandoEsquerda = !olhandoEsquerda; //inverte o valor da variável booleana.
        float x = transform.localScale.x;
        x *= -1;    //Inverte o sinal do scale X
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);

        direcao.x = x;
    }

    void atacar(int atk)
    {
        switch (atk)
        {
            case 0:
                personagem_atacando = false;
                armas[2].SetActive(false);
                break;
            case 1:
                controladorAudio.tocar_efeito(controladorAudio.som_ataque, 1);
                personagem_atacando = true;                
                break;
        }
    }
        
    void interagir()
    {
        Debug.DrawRay(mao.position, direcao * 0.1f, Color.blue);
        RaycastHit2D hit = Physics2D.Raycast(mao.position, direcao, 0.1f, interacao);
        
        if(hit == true)
        {
            objetoInteracao = hit.collider.gameObject;
            iconeAlerta.SetActive(true);
        }
        else
        {
            objetoInteracao = null;
            iconeAlerta.SetActive(false);
        }
    }

    void controleArma(int id)
    {
        foreach (GameObject equipamento in armas)
        {
            equipamento.SetActive(false);
        }

        armas[id].SetActive(true);
    }
    
    private void OnTriggerEnter2D(Collider2D colisao)
    {
        switch (colisao.gameObject.tag)
        {
            case "arma":

                if (informaDano == false)
                {
                    informaDano = true;

                    armaInfo caracteristicas_da_arma = colisao.gameObject.GetComponent<armaInfo>();
                    float danoArma = Random.Range(caracteristicas_da_arma.danoMin, caracteristicas_da_arma.danoMax);
                    pvAtual -= danoArma;

                    GameObject dano_temporario = Instantiate(informaTXTDanoPrefab, transform.position, transform.localRotation);
                    dano_temporario.GetComponent<TextMesh>().text = Mathf.RoundToInt(danoArma).ToString();
                    dano_temporario.GetComponent<MeshRenderer>().sortingLayerName = "HUD";

                    barraPV.definePVAtual(pvAtual);

                    if (pvAtual <= 0)
                    {
                        playerAnimator.SetTrigger("hit");
                        playerAnimator.SetTrigger("morte");
                        break;
                    }
                    else
                    {
                        playerAnimator.SetTrigger("hit"); // Essa parte será mexida BEM mais pra frente
                        break;
                    }                    
                }
                informaDano = false;
                break;
        }        
    }
}
