using UnityEngine;
using UnityEngine.EventSystems;

public static class EventTriggerUtility
{
    public static void AddEventTrigger(GameObject obj, EventTriggerType type, System.Action<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((data) => action(data));
        trigger.triggers.Add(entry);
    }
}
