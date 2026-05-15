using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text cardCountText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private GameObject turnIndicator;
    [SerializeField] private GameObject unoIndicator;

    private string playerId;

    public string PlayerId => playerId;

    public void Setup(PlayerState state)
    {
        if (state == null) return;

        playerId = state.playerId;

        if (playerNameText != null)
            playerNameText.text = state.playerId;

        if (cardCountText != null)
            cardCountText.text = state.cardCount.ToString();

        if (turnIndicator != null)
            turnIndicator.SetActive(state.isCurrentTurn);

        if (unoIndicator != null)
            unoIndicator.SetActive(state.calledUno);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Clear()
    {
        if (playerNameText != null) playerNameText.text = "";
        if (cardCountText != null) cardCountText.text = "";
        if (turnIndicator != null) turnIndicator.SetActive(false);
        if (unoIndicator != null) unoIndicator.SetActive(false);
    }
}
