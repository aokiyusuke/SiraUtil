﻿using System;

namespace SiraUtil.Tools.FPFC
{
    /// <summary>
    /// Represents settings for FPFC. This is subject to changes in the future, so don't be surprised if I update it and you inherit this object.
    /// </summary>
    public interface IFPFCSettings
    {
        /// <summary>
        /// The FOV for the camera. This doesn't work when a VR headset is active.
        /// </summary>
        float FOV { get; }

        /// <summary>
        /// Whether or not the controller is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The arbituary move sensitivity.
        /// </summary>
        float MoveSensitivity { get; }

        /// <summary>
        /// The arbituary mouse sensitivity.
        /// </summary>
        float MouseSensitivity { get; }

        /// <summary>
        /// Ignores and inverts the default state of FPFC when changed.
        /// </summary>
        bool Ignore { get; }

        /// <summary>
        /// Called when the object is changed.
        /// </summary>
        event Action<IFPFCSettings> Changed;
    }
}