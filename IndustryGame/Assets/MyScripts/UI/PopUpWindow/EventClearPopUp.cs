using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EventClearPopUp : MonoBehaviour
{
    public class Data : IPopUpWindow
    {
        public readonly MainEvent mainEvent;
        public Data(MainEvent mainEvent)
        {
            this.mainEvent = mainEvent;
        }
        public void Generate()
        {
            GameObject window = Instantiate(Resources.Load<GameObject>("UI/PopUpWindow/EventClearPopUp"));
            EventClearPopUp instance = window.GetComponent<EventClearPopUp>();
            instance.eventFinishDescriptionText.text = mainEvent.descriptionAfterFinish;
            instance.baseRewardText.text = mainEvent.contribution.ToString();
            instance.wildReservedText.text = mainEvent.WildReservated.ToString();
            instance.manMadeEnvReservedText.text = mainEvent.MamMadeEnvReservated.ToString();
            instance.totalReward.text = mainEvent.TotalReward.ToString();
        }
    }
    public Image eventFinishImage;
    public Text eventFinishDescriptionText;
    public Text baseRewardText;
    public Text wildReservedText;
    public Text manMadeEnvReservedText;
    public Text totalReward;
    public void CloseWindow()
    {
        Destroy(gameObject);
        PopUpCanvas.SetWindowExists(false);
        PopUpCanvas.ShowPopUpWindowStack();
    }
}
