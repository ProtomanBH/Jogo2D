using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class reSkin : MonoBehaviour
{
    private SpriteRenderer  renderizador_de_sprite;

    public  Sprite[]        vetor_de_sprites;
    public  string          nome_folha_sprites;
    public  string          folha_de_sprite_atual;

    private Dictionary<string, Sprite> folha_de_sprites;


    // Start is called before the first frame update
    void Start()
    {
        renderizador_de_sprite = GetComponent<SpriteRenderer>();
        carregarFolhaSprites();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(folha_de_sprite_atual != nome_folha_sprites)
        {
            carregarFolhaSprites();
        }
        renderizador_de_sprite.sprite = folha_de_sprites[renderizador_de_sprite.sprite.name];
    }

    private void carregarFolhaSprites()
    {
        vetor_de_sprites = Resources.LoadAll<Sprite>(nome_folha_sprites); //carrega todos os sprites com o nome passado para o vetor de sprites
        folha_de_sprites = vetor_de_sprites.ToDictionary(x => x.name, x => x);
        folha_de_sprite_atual = nome_folha_sprites;
    }
}
