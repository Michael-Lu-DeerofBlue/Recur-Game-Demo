using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "ResolutionConnection", menuName = "SettingsGenerator/Connection/ResolutionConnection", order = 4)]
    public class ResolutionConnectionSO : OptionConnectionSO
    {
        /// <summary>
        /// Disable if the resolutions change very often.
        /// </summary>
        [Tooltip("Disable if the resolutions change very often.")]
        public bool CacheResolutions = true;

        /// <summary>
        /// If enabled then only those resolution options which match the
        /// current resolution refresh rate are listed. That list may be
        /// much shorter than the full list.
        /// </summary>
        [Tooltip("If enabled then only those resolution options which match the " +
            "current resolution refresh rate are listed. That list may be " +
            "much shorter than the full list.")]
        public bool LimitToCurrentRefreshRate = false;

        /// <summary>
        /// If enabled then only one resolution per frequency will be listed.<br />
        /// For example there may be two resolutions: 640x480 @60Hz and 640x480 @75Hz<br />
        /// If enabled then only one of these will be in the list. It will choose the one
        /// which has the closest frequency to the currently used frequency.
        /// </summary>
        [Tooltip("If enabled then only one resolution per frequency will be listed. " +
            "For example there may be two resolutions: 640x480 @60Hz and 640x480 @75Hz\n" +
            "If enabled then only one of these will be in the list. It will choose the one " +
            "which has the closest frequency to the currently used frequency.")]
        public bool LimitToUniqueResolutions = true;

        /// <summary>
        /// If enabled then any resolution that is bigger than the width or height of the biggest screen (hradware resolution) will be skipped.<br /><br />
        /// NOTICE: This does nothing in the EDITOR since the API does not return the correct size there. Please test it in a real build.
        /// </summary>
        [Tooltip("If enabled then any resolution that is bigger than the width or height of the biggest screen (hradware resolution) will be skipped.\n\n" +
            "NOTICE: This does nothing in the EDITOR since the API does not return the correct size there. Please test it in a real build.")]
        public bool LimitMaxResolutionToDisplayResolution = false;

        /// <summary>
        /// If enabled then then resolutions with a refresh rate of 59 Hz will be
        /// skipped if (and only if) there is an alternative with 60 Hz.
        /// </summary>
        [Tooltip(
            "If enabled then then resolutions with a refresh rate of 59 Hz will be " +
            "skipped if (and only if) there is an alternative with 60 Hz.")]
        public bool SkipRefreshRatesWith59Hz = true;

        /// <summary>
        /// Should the refresh rate (frequency) be added to the labels.<br />
        /// Example without: 1024x768<br />
        /// Example with: 1024x768 (60Hz)<br />
        /// </summary>
        [Tooltip("Should the refresh rate (frequency) be added to the labels.\n" +
            "Example without: 1024x768\n" + 
            "Example with: 1024x768 (60Hz)")]
        public bool AddRefreshRateToLabels = false;

        /// <summary>
        /// A list of aspect ratios (width, height) to use as a positive filter criteria for the list of resolutions.<br />
        /// If the list is empty then no filtering will occur and all resolutions will be listed.<br />
        /// </summary>
        [Tooltip("A list of aspect ratios (x = width, y = height) to use as a positive filter criteria for the list of resolutions.\n" +
                 "If the list is empty then no filtering will occur and all resolutions will be listed.")]
        public List<Vector2Int> AllowedAspectRatios = new List<Vector2Int>();

        /// <summary>
        /// Threshold of how much a resolution can differ from the defined AllowedAspectRatios.<br />
        /// Like if the allowed aspect is 16:9 (w:h), i.e.: 1.77 and this is 0.02f then all ratios between 1.75 and 1.79 are valid. 
        /// </summary>
        [Tooltip("Threshold of how much a resolution can differ from the defined AllowedAspectRatios.\n" +
                 "Like if the allowed aspect is 16:9 (w:h), i.e.: 1.77 and this is 0.02f then all ratios between 1.75 and 1.79 are valid.")]
        public float AllowedAspectRatioDelta = 0.02f;

        [Tooltip("Resolution format {0} = width in pixels. {1} = height in pixels.")]
        public string ResolutionFormat = "{0}x{1}";

        [Tooltip("Will be appended to the normal resolution string if AddRefreshRateToLabels is enabled. {0} is the refresh rate as an integer.")]
        public string RefreshRateFormat = " ({0}Hz)";


        protected ResolutionConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new ResolutionConnection();
            _connection.CacheResolutions = CacheResolutions;
            _connection.LimitToCurrentRefreshRate = LimitToCurrentRefreshRate;
            _connection.LimitToUniqueResolutions = LimitToUniqueResolutions;
            _connection.LimitMaxResolutionToDisplayResolution = LimitMaxResolutionToDisplayResolution;
            _connection.SkipRefreshRatesWith59Hz = SkipRefreshRatesWith59Hz;
            _connection.AddRefreshRateToLabels = AddRefreshRateToLabels;
            _connection.AllowedAspectRatios = AllowedAspectRatios;
            _connection.AllowedAspectRatioDelta = AllowedAspectRatioDelta;
            _connection.SetRefreshRateFormat(RefreshRateFormat);
            _connection.SetResolutionFormat(ResolutionFormat);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
