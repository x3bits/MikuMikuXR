using TinyTeam.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MikuMikuXR.UI.Page
{
    public class BasePage : TTUIPage
    {
        public BasePage(UIType type, UIMode mod, UICollider col) : base(type, mod, col)
        {
        }

        protected T FindCompoment<T>(string gameObjectName)
        {
            return transform.Find(gameObjectName).GetComponent<T>();
        }
        
        protected void SetButtonListener(string gameObjectName, UnityAction action)
        {
            UiUtils.SetButtonListener(transform.Find(gameObjectName).GetComponent<Button>(), action);
        }

        protected void SetSliderOnChangeListener(string gameObjectName, UnityAction<float> action)
        {
            UiUtils.SetSliderOnChangeListener(transform.Find(gameObjectName).GetComponent<Slider>(), action);
        }

        protected void SetEventTriggerListener(string gameObjectName,
            EventTriggerType eventTriggerType, UnityAction<BaseEventData> action)
        {
            var trigger = transform.Find(gameObjectName).GetComponent<EventTrigger>();
            var entry = new EventTrigger.Entry {eventID = eventTriggerType};
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        protected void InitDeltaSlider(string gameObjectName, UnityAction<float> onDeltaChange)
        {
            var changing = false;
            var lastValue = 0.0f;
            var sliderTransform = transform.Find(gameObjectName);
            var slider =  sliderTransform.GetComponent<Slider>();
            slider.value = lastValue;
            UiUtils.SetSliderOnChangeListener(slider, value =>
            {
                if (!changing)
                {
                    return;
                }
                var delta = value - lastValue;
                onDeltaChange.Invoke(delta);
                lastValue = value;
            });
            var trigger = sliderTransform.GetComponent<EventTrigger>();
            var beginDragEntry = new EventTrigger.Entry {eventID = EventTriggerType.BeginDrag};
            beginDragEntry.callback.AddListener(value =>
            {
                changing = true;
            });
            trigger.triggers.Add(beginDragEntry);
            var endDragEntry = new EventTrigger.Entry {eventID = EventTriggerType.EndDrag};
            endDragEntry.callback.AddListener(value =>
            {
                changing = false;
                slider.value = 0.0f;
                lastValue = 0.0f;            
            });
            trigger.triggers.Add(endDragEntry);
        }

    }
}