using System.Linq;
using Boo.Lang;
using MikuMikuXR.SceneController;
using UnityEngine;
using Vuforia;

namespace MikuMikuXR.XR
{
    public class UdtEventHandler : MonoBehaviour, IUserDefinedTargetEventHandler
    {
        public ImageTargetBehaviour ImageTargetTemplate;

        public int LastTargetIndex
        {
            get { return (_targetCounter - 1) % MaxTargets; }
        }

        private const int MaxTargets = 5;
        private UserDefinedTargetBuildingBehaviour _targetBuildingBehaviour;
        private ObjectTracker _objectTracker;

        // DataSet that newly defined targets are added to
        private DataSet _udtDataSet;

        // Currently observed frame quality
        private ImageTargetBuilder.FrameQuality _frameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE;

        private int _targetCounter;


        private void Start()
        {
            _targetBuildingBehaviour = GetComponent<UserDefinedTargetBuildingBehaviour>();

            if (_targetBuildingBehaviour)
            {
                _targetBuildingBehaviour.RegisterEventHandler(this);
                Debug.Log("Registering User Defined Target event handler.");
            }
            VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
            VuforiaARController.Instance.RegisterOnPauseCallback(OnPaused);
        }

        /// <summary>
        /// Called when UserDefinedTargetBuildingBehaviour has been initialized successfully
        /// </summary>
        public void OnInitialized()
        {
            _objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            if (_objectTracker != null)
            {
                // Create a new dataset
                _udtDataSet = _objectTracker.CreateDataSet();
                _objectTracker.ActivateDataSet(_udtDataSet);
            }
        }

        /// <summary>
        /// Updates the current frame quality
        /// </summary>
        public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality)
        {
            Debug.Log("Frame quality changed: " + frameQuality);
            _frameQuality = frameQuality;
            MainSceneController.Instance.OnArFrameQualityChanged.Invoke(frameQuality);

        }

        /// <summary>
        /// Takes a new trackable source and adds it to the dataset
        /// This gets called automatically as soon as you 'BuildNewTarget with UserDefinedTargetBuildingBehaviour
        /// </summary>
        public void OnNewTrackableSource(TrackableSource trackableSource)
        {
            _targetCounter++;

            // Deactivates the dataset first
            _objectTracker.DeactivateDataSet(_udtDataSet);

            // Destroy the oldest target if the dataset is full or the dataset
            // already contains five user-defined targets.
            if (_udtDataSet.HasReachedTrackableLimit() || _udtDataSet.GetTrackables().Count() >= MaxTargets)
            {
                var trackables = _udtDataSet.GetTrackables();
                Trackable oldest = null;
                foreach (var trackable in trackables)
                {
                    if (oldest == null || trackable.ID < oldest.ID)
                        oldest = trackable;
                }

                if (oldest != null)
                {
                    Debug.Log("Destroying oldest trackable in UDT dataset: " + oldest.Name);
                    _udtDataSet.Destroy(oldest, true);
                }
            }

            // Get predefined trackable and instantiate it
            var imageTargetCopy = Instantiate(ImageTargetTemplate);
            imageTargetCopy.gameObject.name = "UserDefinedTarget-" + _targetCounter;

            // Add the duplicated trackable to the data set and activate it
            _udtDataSet.CreateTrackable(trackableSource, imageTargetCopy.gameObject);

            // Activate the dataset again
            _objectTracker.ActivateDataSet(_udtDataSet);

            // Make sure TargetBuildingBehaviour keeps scanning...
            _targetBuildingBehaviour.StartScanning();
        }

        /// <summary>
        /// Instantiates a new user-defined target and is also responsible for dispatching callback to
        /// IUserDefinedTargetEventHandler::OnNewTrackableSource
        /// </summary>
        public bool BuildNewTarget()
        {
            if (_frameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_MEDIUM ||
                _frameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH)
            {
                // create the name of the next target.
                // the TrackableName of the original, linked ImageTargetBehaviour is extended with a continuous number to ensure unique names
                var targetName = string.Format("{0}-{1}", ImageTargetTemplate.TrackableName, _targetCounter);

                // generate a new target:
                _targetBuildingBehaviour.BuildNewTarget(targetName, ImageTargetTemplate.GetSize().x);
                return true;
            }
            Debug.Log("Cannot build new target, due to poor camera image quality");
            return false;
        }

        public void ClearTargets()
        {
            _objectTracker.DeactivateDataSet(_udtDataSet);
            var trackables = _udtDataSet.GetTrackables();
            var trackableList = new List<Trackable>(trackables);
            foreach (var trackable in trackableList)
            {
                _udtDataSet.Destroy(trackable, true);
            }
            _objectTracker.ActivateDataSet(_udtDataSet);
        }
      
        private static void OnVuforiaStarted()
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
      
        private static void OnPaused(bool paused)
        {
            if (!paused) 
            {
                CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
            }
        }

    }
}