using System.Collections.Generic;
using IGC.CardCore_IG04;
using IGC.RPGCore_IG04;
using UnityEngine;
using Mycelia;
using SUEngine;

public class SimBridge : MonoBehaviour
{
    public List<ISim> listSys = new List<ISim>();
    public static List<Card> cards = new List<Card>();
    public static List<int> card1s = new List<int>();
    public CardBoard cardBoard = new CardBoard();
    public GameContext  context = new GameContext(cards,card1s,1,0);
    BinaryStateSerializer  serializer = new BinaryStateSerializer();
    private void Awake()
    {
        DontDestroyOnLoad(this);
        MC.InitTables(new LubanConfigService());
        cards = new List<Card>();
        for (int i = 0; i < 56; i++)
        {
            var card = new Card();
            card.runtimeId = i;
            card.definition = new CardDefinition(i,$"newCard_{i.ToString()}",CardSuit.Nones, 1, 1, null);
            cards.Add(card);
            card1s.Add(i);
            
            //cardBoard.Deck.cardIds.Add(i);
        }
        context.cardBoard.Deck.cardIds.AddRange(card1s);
        //cardBoard.Deck.cardIds.AddRange(card1s);
        listSys.Add(new CardGame(context));
        listSys.Add(new RPGMode());
        MC.Simulate_Awake(listSys);
    }
    void Start()
    {
        MC.Simulate_Start();
    }
    void Update()
    {
        PlayerInput();
        MC.Simulate_Update();
    }


    void PlayerInput()
    {
        if (true)
        {
            var moveInput=TouchInputManager.Instance.gameInput.moveValue;
            var aimInput=TouchInputManager.Instance.gameInput.aimValue;
            //Debug.Log("控制测试:" + moveInput.x + " " + moveInput.y);
            MC.Input.SetMove(moveInput.x,moveInput.y);
            MC.Input.SetAim(aimInput.x,aimInput.y);
        }
    }
}
