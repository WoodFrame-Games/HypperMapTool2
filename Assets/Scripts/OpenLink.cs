using UnityEngine;

public class OpenLink : MonoBehaviour
{
    // Ссылка на ваш Discord
    [SerializeField] private string discordLink = "https://discord.com/users/ваш_ID_или_приглашение";

    // Метод для открытия ссылки
    public void OpenDiscordLink()
    {
        Application.OpenURL(discordLink);
    }

    public void OpenWebLink()
    {
        Application.OpenURL("https://sites.google.com/view/tubmaptool/home");
    }
}