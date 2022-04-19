using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

//Controls chat
public class ChatController : NetworkBehaviour
{
    //player name, is clientside
    string playerName;
    //ui elements
    [SerializeField] GameObject chatButton;
    [SerializeField] GameObject chat;
    [SerializeField] TMP_InputField textField;
    [SerializeField] TMP_Text chatField;

    //for client, initialize his name, print help in chat and send join message for everyone
    public override void OnStartClient()
    {
        base.OnStartClient();

        chatButton.SetActive(true);
        playerName = "Player" + Random.Range(100, 999);
        chatField.text = "Use /name *name* if you want a custom name. Please restrain yourself from spurting too much racial slurs and have fun!\n";
        CmdChatMessage($"{playerName} has joined\n");
    }

    //send chat message if player leaves
    //will send error if no server is present, but who cares
    public override void OnStopClient()
    {
        base.OnStopClient();

        CmdChatMessage($"{playerName} has left\n");
    }

    //called when chat button is pressed
    public void SwitchChatVisibility()
    {
        chat.SetActive(!chat.activeInHierarchy);
    }

    //clientside, called whenever we press send button
    //handles name change
    [Client]
    public void SendChatMessage()
    {
        string message = textField.text;
        if (string.IsNullOrWhiteSpace(message)) return;
        if (message.Length > 127) return;
        if(message.StartsWith("/name "))
        {
            string newName = message.Replace("/name ", "");
            message = $"{playerName} changed their name to {newName}";
            playerName = newName;
        }
        CmdChatMessage($"{playerName}: {message}\n");
        textField.text = "";
    }

    //sends request to server to print in chat
    //requiresAuthority is set to false, because by default only server has authority over all of the scene objects
    [Command(requiresAuthority = false)]
    void CmdChatMessage(string message)
    {
        RpcUpdateChat(message);
    }
    
    //server notifies all clients that chat was changed
    [ClientRpc]
    void RpcUpdateChat(string message)
    {
        chatField.text += message;
    }
}
